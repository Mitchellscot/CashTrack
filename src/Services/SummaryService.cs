﻿using CashTrack.Common;
using CashTrack.Common.Extensions;
using CashTrack.Data.Entities;
using CashTrack.Models.BudgetModels;
using CashTrack.Models.ExpenseModels;
using CashTrack.Models.IncomeSourceModels;
using CashTrack.Models.MerchantModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Models.SummaryModels;
using CashTrack.Repositories.BudgetRepository;
using CashTrack.Repositories.ExpenseRepository;
using CashTrack.Repositories.IncomeRepository;
using CashTrack.Repositories.UserRepository;
using CashTrack.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CashTrack.Services.SummaryService
{
    public interface ISummaryService
    {
        Task<MonthlySummaryResponse> GetMonthlySummaryAsync(MonthlySummaryRequest request);
        Task<AnnualSummaryResponse> GetAnnualSummaryAsync(AnnualSummaryRequest request);

    }
    public class SummaryService : ISummaryService
    {
        private readonly IBudgetRepository _budgetRepo;
        private readonly IExpenseRepository _expenseRepo;
        private readonly IIncomeRepository _incomeRepo;
        private readonly IUserRepository _userRepository;

        public SummaryService(IBudgetRepository budgetRepository, IExpenseRepository expenseRepository, IIncomeRepository incomeRepository, IUserRepository userRepository)
        {
            _budgetRepo = budgetRepository;
            _expenseRepo = expenseRepository;
            _incomeRepo = incomeRepository;
            _userRepository = userRepository;
        }
        public async Task<AnnualSummaryResponse> GetAnnualSummaryAsync(AnnualSummaryRequest request)
        {
            var isCurrentYear = request.Year == DateTime.Now.Year;
            var expensesYTD = await _expenseRepo.Find(x => x.Date.Year == request.Year && !x.ExcludeFromStatistics);
            var incomeYTD = await _incomeRepo.Find(x => x.Date.Year == request.Year && !x.IsRefund);
            var annualBudgets = await _budgetRepo.FindWithMainCategories(x => x.Year == request.Year);
            var budgetsYTD = annualBudgets.Where(x => x.Month <= DateTime.Now.Month).ToArray();
            var budgetsForCharts = isCurrentYear ? budgetsYTD : annualBudgets;
            var incomeForPercentageCharts = Convert.ToInt32(incomeYTD.Sum(x => x.Amount));

            var user = await _userRepository.FindById(request.UserId);
            return new AnnualSummaryResponse()
            {
                LastImport = user.LastImport,
                OverallSummaryChart = GetOverallSummaryChart(expensesYTD, incomeYTD, budgetsForCharts),
                TopExpenses = expensesYTD.Where(x => !x.ExcludeFromStatistics && x.Category.MainCategory.Name != "Mortgage")
                .OrderByDescending(x => x.Amount)
                .Take(10)
                .Select(x => new ExpenseQuickView()
                {
                    Amount = x.Amount,
                    Date = x.Date.ToShortDateString(),
                    Id = x.Id,
                    SubCategory = x.Category == null ? "none" : x.Category.Name
                }).ToList(),
                TopCategories = expensesYTD.Where(x => !x.ExcludeFromStatistics)
                .GroupBy(x => x.Category.Name)
                .Select(x => new SubCategoryQuickView()
                {
                    Amount = x.Sum(y => y.Amount),
                    Name = x.Key,
                    Count = x.Count()
                }).OrderByDescending(x => x.Amount)
                .Take(10).ToList(),
                TopMerchants = expensesYTD.Where(x => !x.ExcludeFromStatistics && x.MerchantId != null)
                .GroupBy(x => x.Merchant.Name)
                .Select(x => new MerchantQuickView()
                {
                    Amount = x.Sum(y => y.Amount),
                    Name = x.Key,
                    Count = x.Count()
                }).OrderByDescending(x => x.Amount)
                .Take(10).ToList(),
                TopSources = incomeYTD.Where(x => !x.IsRefund && x.SourceId != null)
                    .GroupBy(x => x.Source.Name)
                    .Select(x => new IncomeSourceQuickView()
                    {
                        Name = x.Key,
                        Amount = x.Sum(x => x.Amount),
                        Count = x.Count()
                    }).OrderByDescending(x => x.Amount).Take(10).ToList(),
                SavingsChart = GetAnnualSavingsChart(incomeYTD, expensesYTD, annualBudgets),
                SubCategoryPercentages = GetSubCategoryPercentages(expensesYTD, incomeForPercentageCharts),
                IncomeExpenseChart = GetAnnualIncomeExpenseChart(expensesYTD, incomeYTD, annualBudgets),
                MainCategoryPercentages = GetMainCategoryPercentages(expensesYTD, incomeForPercentageCharts),
                MerchantPercentages = GetMerchantPercentages(expensesYTD, incomeForPercentageCharts),
                IncomeSourcePercentages = GetIncomeSourcePercentages(incomeYTD),
                MonthlyExpenseStatistics = AggregateUtilities<ExpenseEntity>.GetAnnualStatisticsByMonth(expensesYTD, request.Year, true),
                AnnualSummary = GetAnnualSummary(expensesYTD, incomeYTD, annualBudgets),
                AnnualMonthlySummaryChart = GetAnnualMonthlySummaryChart(expensesYTD, incomeYTD, annualBudgets)

            };
        }

        private AnnualMonthlySummaryChart GetAnnualMonthlySummaryChart(ExpenseEntity[] expenses, IncomeEntity[] incomes, BudgetEntity[] budgets)
        {
            var incomeData = incomes.GroupBy(x => x.Date.Month).OrderBy(x => x.Key).Select(x => x.Sum(x => x.Amount)).ToList();
            var expenseData = expenses.GroupBy(x => x.Date.Month).OrderBy(x => x.Key).Select(x => x.Sum(x => x.Amount)).ToList();
            var calculatedSavings = incomeData.Zip(expenseData, (a, b) => (a - b)).ToList();

            var lastMonth = calculatedSavings.Count;
            var lastExpenseMonth = expenseData.Count;

            var budgetedIncome = budgets.Where(x => x.BudgetType == BudgetType.Income && x.Month > lastMonth).GroupBy(x => x.Month).Select(x => { return (x.Key, decimal.Round(x.Sum(x => x.Amount), 0)); }).OrderBy(x => x.Key).Where(x => x.Key > lastMonth).Select(x => x.Item2).ToList();

            var budgetedExpenses = budgets.Where(x => x.BudgetType == BudgetType.Need || x.BudgetType == BudgetType.Want).GroupBy(x => x.Month).Select(x => { return (x.Key, decimal.Round(x.Sum(x => x.Amount), 0)); }).OrderBy(x => x.Key).Where(x => x.Key > lastExpenseMonth).Select(x => x.Item2).ToList();

            var budgetedSavings = budgets.Where(x => x.BudgetType == BudgetType.Savings && x.Month > lastMonth).GroupBy(x => x.Month).Select(x => { return (x.Key, decimal.Round(x.Sum(x => x.Amount), 0)); }).OrderBy(x => x.Key).Select(x => x.Item2).ToList();

            var budgetedIncomeDataset = Enumerable.Repeat(decimal.MaxValue, lastMonth).ToList();
            var budgetedExpenseDataset = Enumerable.Repeat(decimal.MaxValue, lastExpenseMonth).ToList();
            var budgetedSavingsDataset = Enumerable.Repeat(decimal.MaxValue, lastMonth).ToList();
            if (!budgetedIncome.Any() && lastMonth != 12)
            {
                var iterations = 12 - lastMonth;
                var lastMonthsIncomeRepeated = Enumerable.Repeat((int)decimal.Round(incomeData.LastOrDefault(), 0), iterations).ToList();
                budgetedIncomeDataset.AddRange(lastMonthsIncomeRepeated.Select(x => (decimal)x).ToList());
            }
            else
            {
                budgetedIncomeDataset.AddRange(budgetedIncome);
            }

            if (!budgetedExpenses.Any() && lastExpenseMonth != 12)
            {
                var iterations = 12 - lastExpenseMonth;
                var lastMonthsExpensesRepeated = Enumerable.Repeat((int)decimal.Round(expenseData.LastOrDefault(), 0), iterations).ToList();
                budgetedExpenseDataset.AddRange(lastMonthsExpensesRepeated.Select(x => (decimal)x).ToList());
            }
            else
            {
                budgetedExpenseDataset.AddRange(budgetedExpenses);
            }

            if (!budgetedSavings.Any() && lastMonth != 12)
            {
                var iterations = 12 - lastMonth;
                var lastMonthsSavingsRepeated = Enumerable.Repeat((int)decimal.Round(calculatedSavings.LastOrDefault(), 0), iterations).ToList();
                budgetedSavingsDataset.AddRange(lastMonthsSavingsRepeated.Select(x => (decimal)x).ToList());
            }
            else
            {
                budgetedSavingsDataset.AddRange(budgetedSavings);
            }
            return new AnnualMonthlySummaryChart()
            {
                IncomeDataset = incomeData.Sum() > 0 ? JsonSerializer.Serialize(incomeData) : string.Empty,
                ExpenseDataset = expenseData.Sum() > 0 ? JsonSerializer.Serialize(expenseData) : string.Empty,
                SavingsDataset = calculatedSavings.Sum() > 0 ? JsonSerializer.Serialize(calculatedSavings) : string.Empty,
                BudgetedIncomeDataset = budgetedIncome.Sum() > 0 ? JsonSerializer.Serialize(budgetedIncomeDataset).Replace(decimal.MaxValue.ToString(), "NaN") : string.Empty,
                BudgetedExpenseDataset = budgetedExpenses.Sum() > 0 ? JsonSerializer.Serialize(budgetedExpenseDataset).Replace(decimal.MaxValue.ToString(), "NaN") : string.Empty,
                BudgetedSavingsDataset = budgetedSavings.Sum() > 0 ? JsonSerializer.Serialize(budgetedSavingsDataset).Replace(decimal.MaxValue.ToString(), "NaN") : string.Empty
            };
        }

        private AnnualSummaryTotals GetAnnualSummary(ExpenseEntity[] expenses, IncomeEntity[] incomes, BudgetEntity[] budgets)
        {
            var isCurrentYear = DateTime.Now.Year == expenses.FirstOrDefault().Date.Year;

            var totalSpent = expenses.Sum(x => x.Amount);
            var totalEarned = incomes.Sum(x => x.Amount);
            var totalSaved = totalEarned - totalSpent;
            var savingsGoal = budgets.Where(x => x.BudgetType == BudgetType.Savings).Sum(x => x.Amount);
            var budgetedExpenses = budgets.Where(x => x.BudgetType == BudgetType.Want || x.BudgetType == BudgetType.Need).Sum(x => x.Amount);
            var totalSavedAsInt = (int)decimal.Round(totalSaved, 0);
            var percentTowardsSavingsGoal = totalSavedAsInt <= savingsGoal ? totalSavedAsInt.ToPercentage(savingsGoal) : 100;

            var suggestedMonthlyAmount = 0;
            var averageAmountSaved = 0;
            if (isCurrentYear)
            {
                var lastMonthOfIncome = incomes.OrderBy(x => x.Date.Month).Select(x => x.Date.Month).LastOrDefault() + 1;
                var iterations = 13 - lastMonthOfIncome;
                suggestedMonthlyAmount = (savingsGoal - totalSavedAsInt) / iterations;
            }
            else
            {
                averageAmountSaved = totalSavedAsInt / 12;
            }
            return new AnnualSummaryTotals()
            {
                Earned = totalEarned,
                Spent = totalSpent,
                Saved = totalSaved,
                SavingsGoalProgress = percentTowardsSavingsGoal,
                SuggestedMonthlySavingsToMeetGoal = suggestedMonthlyAmount,
                AveragedSavedPerMonth = averageAmountSaved,
                BudgetVariance = budgetedExpenses > 0 ? ((int)decimal.Round(totalSpent, 0) - budgetedExpenses) / budgetedExpenses : 0
            };
        }

        private IncomeExpenseChart GetAnnualIncomeExpenseChart(ExpenseEntity[] expenses, IncomeEntity[] incomes, BudgetEntity[] annualBudgets)
        {
            var labels = Enumerable.Range(1, 12).Select(i => @CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i)).ToArray();

            var incomeData = incomes.GroupBy(x => x.Date.Month).OrderBy(x => x.Key).Select(x => x.Sum(x => x.Amount)).ToList();
            var expenseData = expenses.GroupBy(x => x.Date.Month).OrderBy(x => x.Key).Select(x => x.Sum(x => x.Amount)).ToList();
            var lastIncomeMonth = incomeData.Count;
            var lastExpenseMonth = expenseData.Count;

            var budgetedIncome = annualBudgets.Where(x => x.BudgetType == BudgetType.Income && x.Month > lastIncomeMonth).GroupBy(x => x.Month).Select(x => { return (x.Key, decimal.Round(x.Sum(x => x.Amount), 0)); }).OrderBy(x => x.Key).Where(x => x.Key > lastIncomeMonth).Select(x => x.Item2).ToList();

            var budgetedExpenses = annualBudgets.Where(x => x.BudgetType == BudgetType.Need || x.BudgetType == BudgetType.Want).GroupBy(x => x.Month).Select(x => { return (x.Key, decimal.Round(x.Sum(x => x.Amount), 0)); }).OrderBy(x => x.Key).Where(x => x.Key > lastExpenseMonth).Select(x => x.Item2).ToList();

            if (!budgetedIncome.Any() && lastIncomeMonth != 12)
            {
                var iterations = 12 - lastIncomeMonth;
                var lastMonthsIncomeRepeated = Enumerable.Repeat((int)decimal.Round(incomeData.LastOrDefault(), 0), iterations).ToList();
                incomeData.AddRange(lastMonthsIncomeRepeated.Select(x => (decimal)x).ToList());
            }
            else
            {
                incomeData.AddRange(budgetedIncome);
            }

            if (!budgetedExpenses.Any() && lastExpenseMonth != 12)
            {
                var iterations = 12 - lastIncomeMonth;
                var lastMonthsExpensesRepeated = Enumerable.Repeat((int)decimal.Round(expenseData.LastOrDefault(), 0), iterations).ToList();
                expenseData.AddRange(lastMonthsExpensesRepeated.Select(x => (decimal)x).ToList());
            }
            else
            {
                expenseData.AddRange(budgetedExpenses);
            }
            var incomeDataset = incomeData.ToArray().Accumulate();
            var expenseDataset = expenseData.ToArray().Accumulate();
            return new IncomeExpenseChart()
            {
                Labels = JsonSerializer.Serialize(labels),
                IncomeDataset = JsonSerializer.Serialize(incomeDataset),
                MonthBudgetIncomeDataBegins = lastIncomeMonth - 1,
                ExpensesDataset = JsonSerializer.Serialize(expenseDataset),
                MonthBudgetExpenseDataBegins = lastExpenseMonth - 1
            };
        }

        private SavingsChart GetAnnualSavingsChart(IncomeEntity[] incomes, ExpenseEntity[] expenses, BudgetEntity[] budgets)
        {
            var labels = Enumerable.Range(1, 12).Select(i => @CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i)).ToArray();

            var incomeData = incomes.GroupBy(x => x.Date.Month).OrderBy(x => x.Key).Select(x => x.Sum(x => x.Amount)).ToArray();

            var expenseData = expenses.GroupBy(x => x.Date.Month).OrderBy(x => x.Key).Select(x => x.Sum(x => x.Amount)).ToArray();
            var calculatedSavings = incomeData.Zip(expenseData, (a, b) => (a - b)).ToList();

            var lastMonth = calculatedSavings.Count;

            var budgetedSavings = budgets.Where(x => x.BudgetType == BudgetType.Savings && x.Month > lastMonth).GroupBy(x => x.Month).Select(x => { return (x.Key, decimal.Round(x.Sum(x => x.Amount), 0)); }).OrderBy(x => x.Key).Select(x => x.Item2).ToList();

            if (!budgetedSavings.Any() && lastMonth != 12)
            {
                var iterations = 12 - lastMonth;
                var lastMonthsSavingsRepeated = Enumerable.Repeat((int)decimal.Round(calculatedSavings.LastOrDefault(), 0), iterations).ToList();
                calculatedSavings.AddRange(lastMonthsSavingsRepeated.Select(x => (decimal)x).ToList());
            }
            else
            {
                calculatedSavings.AddRange(budgetedSavings);
            }
            var savingsDataset = calculatedSavings.ToArray().Accumulate();

            var projectedSavings = (int)decimal.Round(calculatedSavings.Sum() + budgetedSavings.Sum(), 0);
            var annualSavingsGoal = budgets.Where(x => x.BudgetType == BudgetType.Savings).Sum(x => x.Amount);

            var suggestedSavingsDataset = "";
            if (projectedSavings < annualSavingsGoal && lastMonth != 12)
            {
                var iterations = 12 - lastMonth;
                var suggestedSavings = Enumerable.Repeat(0, lastMonth - 1).ToList();
                suggestedSavings.Add((int)decimal.Round(savingsDataset.ElementAt(lastMonth - 1), 0));
                var suggestedMonthlySavingsAmount = (annualSavingsGoal - (int)decimal.Round(incomeData.Zip(expenseData, (a, b) => (a - b)).ToList().Sum())) / iterations;
                suggestedSavings.AddRange(Enumerable.Repeat(suggestedMonthlySavingsAmount, iterations));
                var accumulatedSavings = suggestedSavings.ToArray().Accumulate().Select(x => x == 0 ? x = int.MaxValue : x);
                suggestedSavingsDataset = JsonSerializer.Serialize(accumulatedSavings).Replace(int.MaxValue.ToString(), "NaN");
            }

            return new SavingsChart()
            {
                SavingsDataset = JsonSerializer.Serialize(savingsDataset),
                Labels = JsonSerializer.Serialize(labels),
                MonthBudgetDataBegins = lastMonth - 1,
                SuggestedSavingsDataset = suggestedSavingsDataset
            };
        }
        private Dictionary<string, decimal> GetIncomeSourcePercentages(IncomeEntity[] income)
        {
            if (income.Length == 0)
                return new Dictionary<string, decimal>();

            var incomeAmount = income.Sum(x => x.Amount);

            var incomePercentages = income.Where(x => !x.IsRefund && x.Category != null).OrderBy(x => x.Category.Name).GroupBy(x => x.Category.Name).Select(x => (Name: x.Key, Amount: x.Sum(x => x.Amount))).Select(x =>
                (x.Name, Percentage: x.Amount.ToDecimalPercentage(incomeAmount)))
                .Where(x => x.Percentage > 0).ToDictionary(k => k.Name, v => v.Percentage);

            var incomeWithCategories = incomePercentages.Sum(x => x.Value);

            var noCategoryPercentage = incomeWithCategories < 100 ? 100 - incomeWithCategories : 0;
            if (noCategoryPercentage > 0)
                incomePercentages.Add("No Category Assigned", noCategoryPercentage);
            return incomePercentages;
        }

        public async Task<MonthlySummaryResponse> GetMonthlySummaryAsync(MonthlySummaryRequest request)
        {
            var isCurrentMonth = request.Month == DateTime.Now.Month && request.Year == DateTime.Now.Year;
            var expensesYTD = await _expenseRepo.Find(x => x.Date.Year == request.Year && x.Date.Month <= request.Month && !x.ExcludeFromStatistics);
            var incomeYTD = await _incomeRepo.Find(x => x.Date.Year == request.Year && x.Date.Month <= request.Month && !x.IsRefund);
            var annualBudgets = await _budgetRepo.FindWithMainCategories(x => x.Year == request.Year);

            var monthlyIncome = incomeYTD.Where(x => x.Date.Month == request.Month).ToArray();
            var monthlyExpenses = expensesYTD.Where(x => x.Date.Month == request.Month).ToArray();
            var monthlyBudgets = annualBudgets.Where(x => x.Month == request.Month).ToArray();

            var incomeToCompare = isCurrentMonth ?
                monthlyBudgets.Where(x => x.BudgetType == BudgetType.Income).Sum(x => x.Amount) :
                (int)decimal.Round(monthlyIncome.Sum(x => x.Amount), 0);

            var monthlySummary = GetMonthlySummary(monthlyExpenses, monthlyIncome, monthlyBudgets, request.Year, request.Month);

            var user = await _userRepository.FindById(request.UserId);

            return new MonthlySummaryResponse()
            {
                LastImport = user.LastImport,
                MonthlySummary = monthlySummary,
                ExpenseSummaryChart = GetExpenseSummaryChartData(monthlyExpenses, monthlyBudgets),
                OverallSummaryChart = GetOverallSummaryChart(monthlyExpenses, monthlyIncome, monthlyBudgets),
                SubCategoryPercentages = GetSubCategoryPercentages(monthlyExpenses, incomeToCompare),
                MainCategoryPercentages = GetMainCategoryPercentages(monthlyExpenses, incomeToCompare),
                MerchantPercentages = GetMerchantPercentages(monthlyExpenses, incomeToCompare),
                MonthlyProgress = GetMonthlyProgress(monthlySummary, request.Year, request.Month),
                AnnualSavingsProgress = GetAnnualSavingsProgress(annualBudgets, expensesYTD, incomeYTD, request.Year, request.Month),
                DailyExpenseLineChart = GetDailyExpenseLineChart(request.Month, request.Year, monthlyExpenses, monthlyBudgets, monthlyIncome),
                YearToDate = GetMonthlyYearToDate(expensesYTD, incomeYTD, request.Month),
                TopExpenses = monthlyExpenses.Where(x => !x.ExcludeFromStatistics).OrderByDescending(x => x.Amount).Take(10).Select(x => new ExpenseQuickView() { Amount = x.Amount, Date = x.Date.ToShortDateString(), Id = x.Id, SubCategory = x.Category == null ? "none" : x.Category.Name }).ToList(),
                TransactionBreakdown = GetTransactionBreakdown(monthlyExpenses, monthlyIncome, monthlyBudgets, isCurrentMonth)
            };
        }

        private List<TransactionBreakdown> GetTransactionBreakdown(ExpenseEntity[] expenses, IncomeEntity[] income, BudgetEntity[] budgets, bool isCurrentMonth)
        {
            if (expenses.Length == 0)
                return new List<TransactionBreakdown>();
            var stats = new List<TransactionBreakdown>();
            var incomeToCompareBy = isCurrentMonth ? decimal.Round(budgets.Where(x => x.BudgetType == BudgetType.Income).Sum(x => x.Amount), 1) : income.Sum(x => x.Amount);


            var subCategories = expenses.Where(x => x.Amount > 0 && !x.ExcludeFromStatistics && x.CategoryId != null).GroupBy(x => x.CategoryId).Select(x =>
            {
                return new TransactionBreakdown()
                {
                    MainCategoryId = x.Select(x => x.Category.MainCategoryId).FirstOrDefault(),
                    SubCategoryId = x.Key.Value,
                    Category = x.Select(x => x.Category.Name).FirstOrDefault(),
                    Amount = x.Sum(x => x.Amount),
                    Percentage = x.Sum(x => x.Amount).ToDecimalPercentage(incomeToCompareBy)
                };
            }).ToList();
            stats.AddRange(subCategories);
            var mainCategories = expenses.Where(x => x.Amount > 0 && !x.ExcludeFromStatistics && x.CategoryId != null).GroupBy(x => x.Category.MainCategory.Id).Select(x =>
            {
                return new TransactionBreakdown()
                {
                    MainCategoryId = x.Key,
                    SubCategoryId = 0,
                    Category = x.Select(x => x.Category.MainCategory.Name).FirstOrDefault(),
                    Amount = x.Sum(x => x.Amount),
                    Percentage = x.Sum(x => x.Amount).ToDecimalPercentage(incomeToCompareBy)
                };
            }).ToList();
            stats.AddRange(mainCategories);

            var incomeTransactions = new TransactionBreakdown()
            {
                MainCategoryId = int.MaxValue - 2,
                SubCategoryId = 0,
                Category = "Income",
                Amount = incomeToCompareBy,
                Percentage = 0
            };
            stats.Add(incomeTransactions);
            var incomeCategories = income.Where(x => x.Amount > 0 && !x.IsRefund && x.CategoryId != null).GroupBy(x => x.Category.Id).Select(x =>
            {
                return new TransactionBreakdown()
                {
                    MainCategoryId = int.MaxValue - 2,
                    SubCategoryId = x.Key,
                    Category = x.Select(x => x.Category.Name).FirstOrDefault(),
                    Amount = x.Sum(x => x.Amount),
                    Percentage = x.Sum(x => x.Amount).ToDecimalPercentage(incomeToCompareBy)
                };
            }).ToList();
            stats.AddRange(incomeCategories);
            var expenseTransactions = new TransactionBreakdown()
            {
                MainCategoryId = int.MaxValue - 1,
                SubCategoryId = 0,
                Category = "Expenses",
                Amount = expenses.Sum(x => x.Amount),
                Percentage = 0
            };
            stats.Add(expenseTransactions);
            var savingsAmount = incomeToCompareBy - expenses.Sum(x => x.Amount);
            var savings = new TransactionBreakdown()
            {
                MainCategoryId = int.MaxValue,
                SubCategoryId = 0,
                Category = "Savings",
                Amount = savingsAmount,
                Percentage = savingsAmount > 0 ? savingsAmount.ToDecimalPercentage(incomeToCompareBy) : 0
            };
            if (savings.Amount != 0)
                stats.Add(savings);

            return stats.OrderBy(x => x.MainCategoryId).ThenBy(x => x.SubCategoryId).ToList();
        }

        private Dictionary<string, int> GetMerchantPercentages(ExpenseEntity[] expenses, int income)
        {
            if (expenses.Length == 0)
                return new Dictionary<string, int>();

            var expenseAmount = expenses.Sum(x => x.Amount);
            var amountToPercentBy = expenseAmount > income ? (int)decimal.Round(expenseAmount, 0) : income;

            var expensePercentages = expenses.Where(x => !x.ExcludeFromStatistics && x.MerchantId != null).OrderBy(x => x.Merchant.Name).GroupBy(x => x.Merchant.Name).Select(x => (Name: x.Key, Amount: (int)decimal.Round(x.Sum(x => x.Amount)))).Select(x =>
                (x.Name, Percentage: x.Amount.ToPercentage(amountToPercentBy)))
                .Where(x => x.Percentage > 0).ToDictionary(k => k.Name, v => v.Percentage);

            var expenseTotalsWithMerchantsAssigned = expensePercentages.Sum(x => x.Value);

            var noMerchantPercentage = expenseTotalsWithMerchantsAssigned < 100 ? 100 - expenseTotalsWithMerchantsAssigned : 0;
            if (noMerchantPercentage > 0)
                expensePercentages.Add("No Merchant Assigned", noMerchantPercentage);
            return expensePercentages;
        }

        private MonthlyYearToDate GetMonthlyYearToDate(ExpenseEntity[] expenses, IncomeEntity[] incomes, int month)
        {
            var labels = Enumerable.Range(1, month).Select(i => @CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i)).ToArray();

            var incomeData = incomes.GroupBy(x => x.Date.Month).OrderBy(x => x.Key).Select(x => (int)decimal.Round(x.Sum(x => x.Amount), 0)).ToArray().Accumulate();

            var expenseData = expenses.GroupBy(x => x.Date.Month).OrderBy(x => x.Key).Select(x => (int)decimal.Round(x.Sum(x => x.Amount), 0)).ToArray().Accumulate();
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
            var budgetedIncomeDataset = budgetedIncome > 0 ? JsonSerializer.Serialize(new[] { budgetedIncome, 0, 0 }) : string.Empty;
            var budgetedExpensesDataset = budgetedExpenses > 0 ? JsonSerializer.Serialize(new[] { 0, budgetedExpenses, 0 }) : string.Empty;
            var budgetedSavingsDataset = budgetedSavings > 0 ? JsonSerializer.Serialize(new[] { 0, 0, budgetedSavings }) : string.Empty;
            return new OverallSummaryChart()
            {
                BudgetedIncome = budgetedIncomeDataset,
                RealizedIncome = new[] { realizedIncome, 0, 0 },
                BudgetedExpenses = budgetedExpensesDataset,
                RealizedExpenses = new[] { 0, realizedExpenses, 0 },
                BudgetedSavings = budgetedSavingsDataset,
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
            bool isCurrentMonth = DateTime.Now.Year == Year && DateTime.Now.Month == Month;
            var realizedIncome = income.Sum(x => x.Amount);
            var budgetedIncome = budgets.Where(x => x.BudgetType == BudgetType.Income).Sum(x => x.Amount);
            //if the summary is generated in the current month, calculate based on projected income
            //if the summary is generated for a previous month, calculate based on realized income
            var incomeToCompareBy = isCurrentMonth ? budgetedIncome : realizedIncome;
            var realizedExpenses = expenses.Sum(x => x.Amount);
            var budgetedNeeds = budgets.Where(x => x.BudgetType == BudgetType.Need).Sum(x => x.Amount);
            var budgetedWants = budgets.Where(x => x.BudgetType == BudgetType.Want).Sum(x => x.Amount);
            var budgetedExpenses = budgetedNeeds + budgetedWants;
            var budgetedSavings = budgets.Where(x => x.BudgetType == BudgetType.Savings).Sum(x => x.Amount);
            bool dippingIntoBudgetedSavings = (realizedExpenses + budgetedSavings) > incomeToCompareBy && ((realizedExpenses + budgetedSavings) - incomeToCompareBy) < budgetedSavings;
            bool dippingIntoRealSavings = realizedExpenses > incomeToCompareBy;

            var realizedSavings = (realizedExpenses + budgetedSavings) <= incomeToCompareBy ?
                budgetedSavings : dippingIntoBudgetedSavings ? incomeToCompareBy - realizedExpenses : 0;

            decimal unspent = 0;
            if (!isCurrentMonth && (realizedExpenses + budgetedSavings) <= realizedIncome)
            {
                unspent = realizedIncome - (realizedExpenses + budgetedSavings);
            }
            else if (isCurrentMonth && realizedExpenses < budgetedExpenses)
            {
                unspent = budgetedIncome - (budgetedExpenses + budgetedSavings);
            }
            else if (isCurrentMonth && realizedExpenses > budgetedExpenses && (realizedExpenses + budgetedSavings) < budgetedIncome)
            {
                unspent = budgetedIncome - (realizedExpenses + budgetedSavings);
            }
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
