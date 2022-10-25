using CashTrack.Models.BudgetModels;
using CashTrack.Repositories.BudgetRepository;
using CashTrack.Repositories.ExpenseRepository;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CashTrack.Services.BudgetService
{
    public interface IBudgetService
    {
        Task<CategoryAveragesAndTotals> GetCategoryAveragesAndTotals(int subCategoryId);
    }
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository _budgetRepo;
        private readonly IExpenseRepository _expenseRepo;

        public BudgetService(IBudgetRepository budgetRepo, IExpenseRepository expenseRepo) => (_budgetRepo, _expenseRepo) = (budgetRepo, expenseRepo);

        public async Task<CategoryAveragesAndTotals> GetCategoryAveragesAndTotals(int subCategoryId)
        {
            var expenses = await _expenseRepo.Find(x => x.CategoryId == subCategoryId && x.Date.Year > DateTime.Now.AddYears(-3).Year);
            var sixMonthTotal = decimal.Round(expenses.Where(x => x.Date > DateTime.Now.AddMonths(-6)).Sum(x => x.Amount));
            var thisYearTotal = decimal.Round(expenses.Where(x => x.Date.Year == DateTime.Now.Year).Sum(x => x.Amount));
            var lastYearTotal = decimal.Round(expenses.Where(x => x.Date.Year == (DateTime.Now.Year - 1)).Sum(x => x.Amount));
            var twoYearsAgoTotal = decimal.Round(expenses.Where(x => x.Date.Year == (DateTime.Now.Year - 2)).Sum(x => x.Amount));

            return new CategoryAveragesAndTotals()
            {
                SixMonthAverages = decimal.Round(sixMonthTotal / 6),
                ThisYearAverages = decimal.Round(thisYearTotal / DateTime.Now.Month),
                LastYearAverages = decimal.Round(lastYearTotal / 12),
                TwoYearsAgoAverages = decimal.Round(twoYearsAgoTotal / 12),
                SixMonthTotals = sixMonthTotal,
                ThisYearTotals = thisYearTotal,
                LastYearTotals = lastYearTotal,
                TwoYearsAgoTotals = twoYearsAgoTotal
            };
        }
    }
}
