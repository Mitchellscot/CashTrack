using CashTrack.Common;
using CashTrack.Common.Extensions;
using CashTrack.Data.Entities;
using CashTrack.Models.BudgetModels;
using CashTrack.Models.ExpenseModels;
using CashTrack.Models.IncomeModels;
using CashTrack.Models.SummaryModels;
using CashTrack.Repositories.BudgetRepository;
using CashTrack.Repositories.ExpenseRepository;
using CashTrack.Repositories.IncomeRepository;
using CashTrack.Repositories.UserRepository;
using CashTrack.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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


            var user = await _userRepository.FindById(request.UserId);
            return new AnnualSummaryResponse()
            {
                LastImport = user.LastImport,
                OverallSummaryChart = GetOverallSummaryChart(expensesYTD, incomeYTD, budgetsForCharts),
                TopExpenses = expensesYTD.Where(x => !x.ExcludeFromStatistics && x.Category.MainCategory.Name != "Mortgage").OrderByDescending(x => x.Amount).Take(10).Select(x => new ExpenseQuickView() { Amount = x.Amount, Date = x.Date.ToShortDateString(), Id = x.Id, SubCategory = x.Category == null ? "none" : x.Category.Name }).ToList(),
                SavingsChart = GetAnnualSavingsChart(incomeYTD, expensesYTD, annualBudgets)
            };
        }

        private SavingsChart GetAnnualSavingsChart(IncomeEntity[] incomes, ExpenseEntity[] expenses, BudgetEntity[] budgets)
        {
            var labels = Enumerable.Range(1, 12).Select(i => @CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i)).ToArray();

            var incomeData = incomes.GroupBy(x => x.Date.Month).OrderBy(x => x.Key).Select(x => x.Sum(x => x.Amount)).ToArray();

            var expenseData = expenses.GroupBy(x => x.Date.Month).OrderBy(x => x.Key).Select(x => x.Sum(x => x.Amount)).ToArray();
            var savings = incomeData.Zip(expenseData, (a, b) => (a - b)).ToList();

            var lastMonth = savings.Count;
            if (lastMonth != 12)
                savings.AddRange(Enumerable.Range(lastMonth, 12).Select(i => decimal.MaxValue));

            var budgetDataSet = new List<int>();
            var budgetData = budgets.Where(x => x.BudgetType == BudgetType.Savings && x.Month > lastMonth).GroupBy(x => x.Month).Select(x => { return (x.Key, x.Sum(x => x.Amount)); }).OrderBy(x => x.Key).Select(x => x.Item2).ToList();
            if (budgetData.Any())
            {
                budgetDataSet.AddRange(Enumerable.Range(0, lastMonth).Select(i => int.MaxValue));
                budgetDataSet.AddRange(budgetData);
            }

            return new SavingsChart()
            {
                SavingsDataset = JsonSerializer.Serialize(savings.ToArray()).Replace(decimal.MaxValue.ToString(), "NaN"),
                BudgetedSavingsDataset = JsonSerializer.Serialize(budgetDataSet.ToArray()).Replace(int.MaxValue.ToString(), "NaN"),
                Labels = JsonSerializer.Serialize(labels)
            };
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
