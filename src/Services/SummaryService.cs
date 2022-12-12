using CashTrack.Common;
using CashTrack.Data.Entities;
using CashTrack.Models.BudgetModels;
using CashTrack.Models.SummaryModels;
using CashTrack.Repositories.BudgetRepository;
using CashTrack.Repositories.ExpenseRepository;
using CashTrack.Repositories.IncomeRepository;
using CashTrack.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashTrack.Services.SummaryService
{
    public interface ISummaryService
    {
        Task<MonthlySummaryResponse> GetMonthlySummaryAsync(MonthlySummaryRequest request);

    }
    public class SummaryService : ISummaryService
    {
        private readonly IBudgetRepository _budgetRepo;
        private readonly IExpenseRepository _expenseRepo;
        private readonly IIncomeRepository _incomeRepo;

        public SummaryService(IBudgetRepository budgetRepository, IExpenseRepository expenseRepository, IIncomeRepository incomeRepository)
        {
            _budgetRepo = budgetRepository;
            _expenseRepo = expenseRepository;
            _incomeRepo = incomeRepository;
        }

        public async Task<MonthlySummaryResponse> GetMonthlySummaryAsync(MonthlySummaryRequest request)
        {
            var expenses = await _expenseRepo.Find(x => x.Date.Year == request.Year && x.Date.Month == request.Month && !x.ExcludeFromStatistics);
            var income = await _incomeRepo.Find(x => x.Date.Year == request.Year && x.Date.Month == request.Month && !x.IsRefund);
            var budgets = await _budgetRepo.FindWithMainCategories(x => x.Year == request.Year && x.Month == request.Month);

            var incomeToCompare = request.Month == DateTime.Now.Month && request.Year == DateTime.Now.Year ?
                budgets.Where(x => x.BudgetType == BudgetType.Income).Sum(x => x.Amount) :
                (int)decimal.Round(income.Sum(x => x.Amount), 0);

            var monthlySummary = GetMonthlySummary(expenses, income, budgets, request.Year, request.Month);
            var progress = GetMonthlyProgress(monthlySummary, request.Year, request.Month);

            return new MonthlySummaryResponse()
            {
                MonthlySummary = monthlySummary,
                ExpenseSummaryChart = GetExpenseSummaryChartData(expenses, budgets),
                OverallSummaryChart = GetOverallSummaryChart(expenses, income, budgets),
                SubCategoryPercentages = GetSubCategoryPercentages(expenses, incomeToCompare),
                MainCategoryPercentages = GetMainCategoryPercentages(expenses, incomeToCompare),
                MonthlyProgress = progress
            };
        }

        private MonthlyProgress GetMonthlyProgress(MonthlySummary summary, int year, int month)
        {
            var incomeToCompareBy = year == DateTime.Now.Year && month == DateTime.Now.Month ? summary.BudgetedIncome : summary.RealizedIncome;

            var discretionarySpendingWindow = incomeToCompareBy - (summary.BudgetedExpenses + summary.BudgetedSavings);
            return new MonthlyProgress()
            {
                RealizedIncome = summary.RealizedIncome.ToPercentage(summary.BudgetedIncome) > 100 ? 100 : summary.RealizedIncome.ToPercentage(summary.BudgetedIncome),
                BudgetedExpenses = summary.RealizedExpenses.ToPercentage(summary.BudgetedExpenses) > 100 ? 100 : summary.RealizedExpenses.ToPercentage(summary.BudgetedExpenses),
                BudgetedSavings = summary.RealizedSavings.ToPercentage(summary.BudgetedSavings),
                DiscretionarySpendingPercent = discretionarySpendingWindow > 0 ? summary.Unspent.ToPercentage(discretionarySpendingWindow) : 0,
                DiscretionarySpendingAmount = summary.Unspent

            };
        }

        private Dictionary<string, int> GetMainCategoryPercentages(ExpenseEntity[] expenses, int income)
        {
            if (expenses.Length == 0)
                return new Dictionary<string, int>();

            var expenseAmount = expenses.Sum(x => x.Amount);
            var amountToPercentBy = expenseAmount > income ? (int)decimal.Round(expenseAmount, 0) : income;

            var expensePercentages = expenses.Where(x => !x.ExcludeFromStatistics).OrderBy(x => x.Category.MainCategoryId).GroupBy(x => x.Category.MainCategory.Name).Select(x => (Name: x.Key, Amount: (int)decimal.Round(x.Sum(x => x.Amount)))).Select(x =>
                (x.Name, Percentage: x.Amount.ToPercentage(amountToPercentBy)))
                .Where(x => x.Percentage > 0).ToDictionary(k => k.Name, v => v.Percentage);


            var savingsPercentage = income > expenseAmount ? (income - expenseAmount).ToPercentage(income) : 0;

            if (savingsPercentage > 0)
                expensePercentages.Add("Savings", savingsPercentage);
            return expensePercentages;
        }

        private Dictionary<string, int> GetSubCategoryPercentages(ExpenseEntity[] expenses, int income)
        {
            if (expenses.Length == 0)
                return new Dictionary<string, int>();

            var expenseAmount = expenses.Sum(x => x.Amount);
            var amountToPercentBy = expenseAmount > income ? (int)decimal.Round(expenseAmount, 0) : income;
            var expensePercentages = expenses.OrderBy(x => x.Category.MainCategoryId).Where(x => !x.ExcludeFromStatistics).GroupBy(x => x.Category.Name).Select(x => (Name: x.Key, Amount: (int)decimal.Round(x.Sum(x => x.Amount))))
                .Select(x => (x.Name, Percentage: x.Amount.ToPercentage(amountToPercentBy)))
                .Where(x => x.Percentage > 0).ToDictionary(k => k.Name, v => v.Percentage);

            var savingsPercentage = income > expenseAmount ? (income - expenseAmount).ToPercentage(income) : 0;

            if (savingsPercentage > 0)
                expensePercentages.Add("Savings", savingsPercentage);

            return expensePercentages;
        }

        private OverallSummaryChart GetOverallSummaryChart(ExpenseEntity[] expenses, IncomeEntity[] income, BudgetEntity[] budgets)
        {
            var budgetedIncome = budgets.Where(x => x.BudgetType == BudgetType.Income).Sum(x => x.Amount);
            var realizedIncome = income.Sum(x => x.Amount);
            var budgetedExpenses = budgets.Where(x => x.BudgetType == BudgetType.Need || x.BudgetType == BudgetType.Want).Sum(x => x.Amount);
            var realizedExpenses = expenses.Sum(x => x.Amount);
            var budgetedSavings = budgets.Where(x => x.BudgetType == BudgetType.Savings).Sum(x => x.Amount);
            var realizedSavings = realizedIncome - realizedExpenses;
            return new OverallSummaryChart()
            {
                BudgetedIncome = new[] { budgetedIncome, 0, 0 },
                RealizedIncome = new[] { realizedIncome, 0, 0 },
                BudgetedExpenses = new[] { 0, budgetedExpenses, 0 },
                RealizedExpenses = new[] { 0, realizedExpenses, 0 },
                BudgetedSavings = new[] { 0, 0, budgetedSavings },
                RealizedSavings = new[] { 0, 0, realizedSavings },
            };
        }

        internal ExpenseSummaryChartData GetExpenseSummaryChartData(ExpenseEntity[] expenses, BudgetEntity[] budgets)
        {
            var expenseMainLabels = expenses.Select(x => x.Category.MainCategory.Name).Distinct().ToList();
            var budgetMainLabels = budgets.Where(x => x.SubCategoryId != null && x.Amount > 0 && x.BudgetType == BudgetType.Need || x.BudgetType == BudgetType.Want).Select(x => x.SubCategory.MainCategory.Name).Distinct().ToList();
            expenseMainLabels.AddRange(budgetMainLabels);
            var categoryLabels = expenseMainLabels.Distinct().OrderBy(x => x).ToArray();

            var budgetedExpenses = budgets.Where(x => x.BudgetType == BudgetType.Want || x.BudgetType == BudgetType.Need && x.SubCategoryId != null && x.Amount > 0).ToArray();

            return new ExpenseSummaryChartData()
            {
                Labels = ChartUtilities.GenerateMonthlyChartLabels(false, categoryLabels, false),
                BudgetedExpenses = ChartUtilities.GetMonthlyBudgetExpenseData(budgetedExpenses, false, false, categoryLabels),
                RealizedExpenses = ChartUtilities.GetMonthlySummaryExpenseData(expenses, false, false, categoryLabels)
            };
        }

        internal MonthlySummary GetMonthlySummary(ExpenseEntity[] expenses, IncomeEntity[] income, BudgetEntity[] budgets, int Year, int Month)
        {
            var realizedIncome = income.Sum(x => x.Amount);
            var budgetedIncome = budgets.Where(x => x.BudgetType == BudgetType.Income).Sum(x => x.Amount);
            //if the summary is generated in the current month, calculate based on projected income
            //if the summary is generated for a previous month, calculate based on realized income
            var incomeToCompareBy = DateTime.Now.Year == Year && DateTime.Now.Month == Month ? budgetedIncome : realizedIncome;
            var realizedExpenses = expenses.Sum(x => x.Amount);
            var budgetedNeeds = budgets.Where(x => x.BudgetType == BudgetType.Need).Sum(x => x.Amount);
            var budgetedWants = budgets.Where(x => x.BudgetType == BudgetType.Want).Sum(x => x.Amount);
            var budgetedExpenses = budgetedNeeds + budgetedWants;
            var budgetedSavings = budgets.Where(x => x.BudgetType == BudgetType.Savings).Sum(x => x.Amount);
            bool dippingIntoBudgetedSavings = (realizedExpenses + budgetedSavings) > incomeToCompareBy && ((realizedExpenses + budgetedSavings) - incomeToCompareBy) < budgetedSavings;
            bool dippingIntoRealSavings = realizedExpenses > incomeToCompareBy;

            var realizedSavings = (realizedExpenses + budgetedSavings) <= incomeToCompareBy ?
                budgetedSavings : dippingIntoBudgetedSavings ? incomeToCompareBy - realizedExpenses : 0;

            var unspent = (realizedExpenses + budgetedSavings) <= incomeToCompareBy ? incomeToCompareBy - (realizedExpenses + budgetedSavings) : 0;

            var estimatedSavings = dippingIntoRealSavings ? (incomeToCompareBy - realizedExpenses) : dippingIntoBudgetedSavings ? realizedSavings : budgetedSavings + unspent;
            return new MonthlySummary()
            {
                RealizedIncome = realizedIncome,
                BudgetedIncome = budgetedIncome,
                RealizedExpenses = realizedExpenses,
                BudgetedExpenses = budgetedExpenses,
                RealizedSavings = realizedSavings,
                BudgetedSavings = budgetedSavings,
                Unspent = unspent,
                EstimatedSavings = estimatedSavings,
                BudgetVariance = budgetedExpenses > 0 ? (realizedExpenses - budgetedExpenses) / budgetedExpenses : 0
            };
        }
    }
}
