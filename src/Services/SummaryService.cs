using CashTrack.Data.Entities;
using CashTrack.Models.BudgetModels;
using CashTrack.Models.ExpenseModels;
using CashTrack.Models.SummaryModels;
using CashTrack.Repositories.BudgetRepository;
using CashTrack.Repositories.ExpenseRepository;
using CashTrack.Repositories.IncomeRepository;
using CashTrack.Services.BudgetService;
using System;
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
            var budgets = await _budgetRepo.Find(x => x.Year == request.Year && x.Month == request.Month);

            return new MonthlySummaryResponse()
            {
                MonthlySummary = GetMonthlySummary(expenses, income, budgets, request.Year, request.Month)
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
                budgetedSavings : dippingIntoBudgetedSavings ? (realizedExpenses + budgetedSavings) - incomeToCompareBy : 0;

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
                EstimatedSavings = estimatedSavings //change ui to say Actual Savings if in the past
            };
        }
    }
}
