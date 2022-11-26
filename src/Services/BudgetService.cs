using CashTrack.Common.Exceptions;
using CashTrack.Data.Entities;
using CashTrack.Models.BudgetModels;
using CashTrack.Models.ExpenseModels;
using CashTrack.Repositories.BudgetRepository;
using CashTrack.Repositories.ExpenseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashTrack.Services.BudgetService
{
    public interface IBudgetService
    {
        Task<CategoryAveragesAndTotals> GetCategoryAveragesAndTotals(int subCategoryId);
        Task<int> CreateBudgetItem(AddBudgetAllocation request);
        Task<bool> DeleteBudgetAsync(int id);
        Task<BudgetPageResponse> GetBudgetPageAsync(BudgetPageRequest request);
    }
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository _budgetRepo;
        private readonly IExpenseRepository _expenseRepo;

        public BudgetService(IBudgetRepository budgetRepo, IExpenseRepository expenseRepo) => (_budgetRepo, _expenseRepo) = (budgetRepo, expenseRepo);

        public async Task<BudgetPageResponse> GetBudgetPageAsync(BudgetPageRequest request)
        {
            var budgets = await _budgetRepo.Find(x => x.Year == DateTime.Now.Year);

            return new BudgetPageResponse()
            {
                AnnualBudgetChartData = new AnnualBudgetChartData()
                {
                    IncomeData = GetMonthlyData(budgets, BudgetType.Income).ToList(),
                    NeedsData = GetMonthlyData(budgets, BudgetType.Need).ToList(),
                    WantsData = GetMonthlyData(budgets, BudgetType.Want).ToList(),
                    SavingsData = GetSavingsData(budgets).ToList(),
                    Unallocated = GetUnallocatedData(budgets).ToList()
                }
            };
        }
        private IEnumerable<int> GetSavingsData(BudgetEntity[] budgets)
        {
            var monthlyIncome = budgets.Where(x => x.BudgetType == BudgetType.Income).GroupBy(x => x.Month).Select(x =>
            {
                return (Month: x.Key, Amount: x.Sum(x => x.Amount));
            }).OrderBy(x => x.Month).ToDictionary(k => k.Month, v => v.Amount);

            var monthlyExpenses = budgets.Where(x => x.BudgetType == BudgetType.Need || x.BudgetType == BudgetType.Want).GroupBy(x => x.Month).Select(x =>
            {
                return (Month: x.Key, Amount: x.Sum(x => x.Amount));
            }).OrderBy(x => x.Month).ToDictionary(k => k.Month, v => v.Amount);

            var monthlySavings = budgets.Where(x => x.BudgetType == BudgetType.Savings).GroupBy(x => x.Month).Select(x =>
            {
                return (Month: x.Key, Amount: x.Sum(x => x.Amount));
            }).OrderBy(x => x.Month).ToDictionary(k => k.Month, v => v.Amount);

            for (int i = 1; i <= 12; i++)
            {
                if (monthlyIncome.Any(x => x.Key == i) && monthlyExpenses.Any(x => x.Key == i) && monthlySavings.Any(x => x.Key == i))
                {
                    if (monthlyIncome[i] > (monthlyExpenses[i] + monthlySavings[i]))
                        yield return monthlySavings[i];
                    else if (monthlyIncome[i] < (monthlyExpenses[i] + monthlySavings[i]))
                        yield return (monthlyIncome[i] - (monthlyExpenses[i] + monthlySavings[i])) + monthlySavings[i];
                    else yield return monthlySavings[i];
                }
                else if (monthlyIncome.Any(x => x.Key == i) && !monthlyExpenses.Any(x => x.Key == i))
                    yield return monthlySavings[i];
                else if (!monthlyIncome.Any(x => x.Key == i) && monthlyExpenses.Any(x => x.Key == i))
                    yield return 0 - monthlyExpenses[i];
                else yield return 0;
            }
        }

        private IEnumerable<int> GetUnallocatedData(BudgetEntity[] budgets)
        {
            var monthlyIncome = budgets.Where(x => x.BudgetType == BudgetType.Income).GroupBy(x => x.Month).Select(x =>
            {
                return (Month: x.Key, Amount: x.Sum(x => x.Amount));
            }).OrderBy(x => x.Month).ToDictionary(k => k.Month, v => v.Amount);

            var monthlyExpenses = budgets.Where(x => x.BudgetType != BudgetType.Income).GroupBy(x => x.Month).Select(x =>
            {
                return (Month: x.Key, Amount: x.Sum(x => x.Amount));
            }).OrderBy(x => x.Month).ToDictionary(k => k.Month, v => v.Amount);

            for (int i = 1; i <= 12; i++)
            {
                if (monthlyIncome.Any(x => x.Key == i) && monthlyExpenses.Any(x => x.Key == i))
                {
                    if (monthlyIncome[i] > monthlyExpenses[i])
                        yield return monthlyIncome[i] - monthlyExpenses[i];
                    else yield return 0;
                }
                else if (monthlyIncome.Any(x => x.Key == i) && !monthlyExpenses.Any(x => x.Key == i))
                    yield return monthlyIncome[i];
                else
                    yield return 0;
            }
        }

        private IEnumerable<int> GetMonthlyData(BudgetEntity[] budgets, BudgetType type)
        {
            var monthlyData = budgets.Where(x => x.BudgetType == type).GroupBy(x => x.Month).Select(x =>
            {
                return (Month: x.Key, Amount: x.Sum(x => x.Amount));
            }).OrderBy(x => x.Month).ToDictionary(k => k.Month, v => v.Amount);

            for (int i = 1; i <= 12; i++)
            {
                if (monthlyData.Any(x => x.Key == i))
                    yield return monthlyData[i];
                else
                    yield return 0;
            }
        }

        public async Task<int> CreateBudgetItem(AddBudgetAllocation request)
        {
            var isIncome = request.IsIncome || request.Type == BudgetType.Income;
            var annualBudget = request.Month == 0 || request.TimeSpan == AllocationTimeSpan.Year;
            var isWeekly = request.TimeSpan == AllocationTimeSpan.Week;

            if (request.Amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(request.Amount));

            if (!annualBudget && request.Month < 0 || !annualBudget && request.Month > 12)
                throw new ArgumentOutOfRangeException(nameof(request.Month));

            if (!isIncome && request.Type != BudgetType.Savings && request.SubCategoryId == 0)
                throw new ArgumentException("Expense Budgets must have a Category");

            if (annualBudget)
            {
                var budgetEntities = new List<BudgetEntity>();
                for (int i = 1; i < 13; i++)
                {
                    var onceAMonthBudget = new BudgetEntity();
                    onceAMonthBudget.Amount = isWeekly ? (request.Amount * 52) / 12 : request.Amount / 12;
                    onceAMonthBudget.Month = i;
                    onceAMonthBudget.Year = request.Year;
                    onceAMonthBudget.BudgetType = isIncome ? BudgetType.Income : request.Type;
                    budgetEntities.Add(onceAMonthBudget);
                }
                return await _budgetRepo.CreateMany(budgetEntities);
            }

            var budgetEntity = new BudgetEntity();
            budgetEntity.Amount = isWeekly ? (request.Amount * 52) / 12 : request.Amount;
            budgetEntity.Month = request.Month;
            budgetEntity.Year = request.Year;
            budgetEntity.BudgetType = isIncome ? BudgetType.Income : request.Type;
            budgetEntity.SubCategoryId = isIncome ? null : request.SubCategoryId;

            return await _budgetRepo.Create(budgetEntity);
        }

        public async Task<bool> DeleteBudgetAsync(int id)
        {
            var budget = await _budgetRepo.FindById(id);
            if (budget == null)
                throw new BudgetNotFoundException("Invalid Budget Id");

            return await _budgetRepo.Delete(budget);
        }

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
