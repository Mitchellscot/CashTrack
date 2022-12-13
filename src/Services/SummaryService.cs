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
using System.Data;
using System.Globalization;
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
            var expensesYTD = await _expenseRepo.Find(x => x.Date.Year == request.Year && x.Date.Month <= request.Month && !x.ExcludeFromStatistics);
            var incomeYTD = await _incomeRepo.Find(x => x.Date.Year == request.Year && x.Date.Month <= request.Month && !x.IsRefund);
            var annualBudgets = await _budgetRepo.FindWithMainCategories(x => x.Year == request.Year);

            var monthlyIncome = incomeYTD.Where(x => x.Date.Month == request.Month).ToArray();
            var monthlyExpenses = expensesYTD.Where(x => x.Date.Month == request.Month).ToArray();
            var monthlyBudgets = annualBudgets.Where(x => x.Month == request.Month).ToArray();

            var incomeToCompare = request.Month == DateTime.Now.Month && request.Year == DateTime.Now.Year ?
                monthlyBudgets.Where(x => x.BudgetType == BudgetType.Income).Sum(x => x.Amount) :
                (int)decimal.Round(monthlyIncome.Sum(x => x.Amount), 0);

            var monthlySummary = GetMonthlySummary(monthlyExpenses, monthlyIncome, monthlyBudgets, request.Year, request.Month);

            return new MonthlySummaryResponse()
            {
                MonthlySummary = monthlySummary,
                ExpenseSummaryChart = GetExpenseSummaryChartData(monthlyExpenses, monthlyBudgets),
                OverallSummaryChart = GetOverallSummaryChart(monthlyExpenses, monthlyIncome, monthlyBudgets),
                SubCategoryPercentages = GetSubCategoryPercentages(monthlyExpenses, incomeToCompare),
                MainCategoryPercentages = GetMainCategoryPercentages(monthlyExpenses, incomeToCompare),
                MonthlyProgress = GetMonthlyProgress(monthlySummary, request.Year, request.Month),
                AnnualSavingsProgress = GetAnnualSavingsProgress(annualBudgets, expensesYTD, incomeYTD, request.Year, request.Month),
                DailyExpenseLineChart = GetDailyExpenseLineChart(request.Month, request.Year, monthlyExpenses, monthlyBudgets, monthlyIncome),
                YearToDate = GetMonthlyYearToDate(expensesYTD, incomeYTD, request.Month)
            };
        }

        private MonthlyYearToDate GetMonthlyYearToDate(ExpenseEntity[] expenses, IncomeEntity[] incomes, int month)
        {
            var labels = Enumerable.Range(1, month).Select(i => @CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i)).ToArray();

            var incomeData = incomes.GroupBy(x => x.Date.Month).Select(x => (int)decimal.Round(x.Sum(x => x.Amount), 0)).ToArray();
            var expenseData = expenses.GroupBy(x => x.Date.Month).Select(x => (int)decimal.Round(x.Sum(x => x.Amount), 0)).ToArray();
            var savings = incomeData.Zip(expenseData, (a, b) => (a - b)).ToArray();
            return new MonthlyYearToDate()
            {
                IncomeDataset = incomeData,
                ExpenseDataset = expenseData,
                SavingsDataset = savings,
                Labels = labels
            };
        }

        private DailyExpenseChart GetDailyExpenseLineChart(int month, int year, ExpenseEntity[] expenses, BudgetEntity[] budgets, IncomeEntity[] income)
        {
            var date = new DateTime(year, month, 1);
            var isCurrentMonth = DateTime.Now.Year == year && DateTime.Now.Month == month;
            var lastDayOfChart = isCurrentMonth ? DateTime.Now.Day : date.AddMonths(1).AddMinutes(-1).Date.Day;

            decimal monthlyAmount = 0;
            var listOfDailyAmounts = new List<decimal>();
            for (int i = 1; i <= lastDayOfChart; i++)
            {
                var dailyAmount = expenses.Where(x => x.Date.Day == i).Sum(x => x.Amount);
                if (dailyAmount > 0)
                {
                    monthlyAmount += dailyAmount;
                    listOfDailyAmounts.Add(decimal.Round(monthlyAmount, 2));
                }
                else
                {
                    listOfDailyAmounts.Add(monthlyAmount);
                }
            }
            var max = 0;
            var expenseMax = expenses.Sum(x => x.Amount);
            var expenseBudgetMax = budgets.Where(x => x.BudgetType == BudgetType.Need || x.BudgetType == BudgetType.Want && x.Amount > 0 && x.SubCategoryId != null).Sum(x => x.Amount);
            var budgetedSavings = budgets.Where(x => x.BudgetType == BudgetType.Savings).Sum(x => x.Amount);

            var budgetedIncomeMax = budgets.Where(x => x.BudgetType == BudgetType.Income).Sum(x => x.Amount);

            var realizedIncomeMax = income.Sum(x => x.Amount);
            var incomeToCompareBy = DateTime.Now.Year == year && DateTime.Now.Month == month && realizedIncomeMax > 0 ? budgetedIncomeMax : realizedIncomeMax;
            var discretionaryMax = (incomeToCompareBy - budgetedSavings);
            if (expenseMax <= expenseBudgetMax)
            {
                max = expenseBudgetMax;
            }
            else if (expenseMax <= discretionaryMax && expenseMax > expenseBudgetMax)
            {
                max = (int)decimal.Round(discretionaryMax, 0);
            }
            else if (expenseMax <= incomeToCompareBy && expenseMax > discretionaryMax)
            {
                max = (int)decimal.Round(incomeToCompareBy, 0);
            }
            else
            {
                max = (int)decimal.Round(expenseMax, 0);
            }
            return new DailyExpenseChart()
            {
                Dataset = listOfDailyAmounts.ToArray(),
                Labels = Enumerable.Range(1, lastDayOfChart).ToArray(),
                ExpenseBudgetMax = expenseBudgetMax,
                DiscretionarySpendingMax = (int)decimal.Round(discretionaryMax, 0),
                IncomeMax = (int)decimal.Round(incomeToCompareBy, 0),
                ExpenseMax = (int)decimal.Round(expenseMax, 0),
                Max = max
            };
        }

        private AnnualSavingsProgress GetAnnualSavingsProgress(BudgetEntity[] budgets, ExpenseEntity[] expenses, IncomeEntity[] income, int year, int month)
        {
            var annualSavingsGoal = budgets.Where(x => x.BudgetType == BudgetType.Savings).Sum(x => x.Amount);
            var previousExpenses = expenses.Where(x => x.Date.Year == year && x.Date.Month < month).Sum(x => x.Amount);
            var previousIncome = income.Where(x => x.Date.Year == year && x.Date.Month < month).Sum(x => x.Amount);
            var amountSaved = (previousIncome - previousExpenses) > 0 ? (previousIncome - previousExpenses) : 0;
            var percentSaved = amountSaved.ToPercentage(annualSavingsGoal);

            var displaySavingsMessage = DateTime.Now.Month == month && DateTime.Now.Year == year;
            var message = string.Empty;
            if (displaySavingsMessage)
            {
                var savingsGoalForTheCurrentMonth = budgets.Where(x => x.BudgetType == BudgetType.Savings && x.Month <= DateTime.Now.Month).Sum(x => x.Amount);
                var currentSavings = (int)decimal.Round(amountSaved, 0);
                var savingsGoalThisMonth = budgets.Where(x => x.BudgetType == BudgetType.Savings && x.Month == DateTime.Now.Month && x.Year == DateTime.Now.Year).Sum(x => x.Amount);
                if ((currentSavings - savingsGoalForTheCurrentMonth) < 0)
                {
                    var extraMonthlySavings = (savingsGoalForTheCurrentMonth - currentSavings) / (13 - month);
                    var extraOneTimeSavings = (savingsGoalForTheCurrentMonth - currentSavings) - savingsGoalThisMonth;
                    if (DateTime.Now.Month == month)
                    {
                        message = $"Save an extra ${extraOneTimeSavings} this month to meet your goal";
                    }
                    else
                        message = $"Save an extra ${extraOneTimeSavings} this month or an extra {extraMonthlySavings} each month in order to meet your goal";
                }
                else
                {
                    message = "You are on track to meet your savings goal for the year.";
                }
            }

            return new AnnualSavingsProgress()
            {
                AnnualSavingsPercentDone = percentSaved,
                AnnualSavingsAmount = amountSaved,
                AnnualSavingsMessage = message
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
