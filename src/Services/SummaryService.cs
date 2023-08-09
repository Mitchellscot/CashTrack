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
        Task<List<TransactionBreakdown>> GetTransactionsToPrint(PrintTransactionsRequest request);
        Task<MonthlySummaryResponse> GetMonthlySummaryAsync(MonthlySummaryRequest request);
        Task<AnnualSummaryResponse> GetAnnualSummaryAsync(AnnualSummaryRequest request);
        Task<AllTimeSummaryResponse> GetAllTimeSummaryAsync();
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
        public async Task<AllTimeSummaryResponse> GetAllTimeSummaryAsync()
        {
            var expenses = await _expenseRepo.Find(x => !x.ExcludeFromStatistics);
            var income = await _incomeRepo.Find(x => !x.IsRefund);
            var incomeForPercentageCharts = Convert.ToInt32(income.Sum(x => x.Amount));
            if (expenses.Length == 0)
                return EmptyAllTimeSummaryResponse();

            if (income.Length == 0)
                return EmptyAllTimeSummaryResponse();

            var spansMultipleYears = expenses.GroupBy(x => x.Date.Year).Select(x => x.Key).Count() > 1 || income.GroupBy(x => x.Date.Year).Select(x => x.Key).Count() > 1;

            return new AllTimeSummaryResponse()
            {
                DataSpansMultipleYears = spansMultipleYears,
                TopExpenses = GetTopExpenses(expenses),
                TopCategories = GetTopSubCategories(expenses),
                TopMerchants = GetTopMerchants(expenses),
                TopSources = GetTopSources(income),
                OverallSummaryChart = GetOverallSummaryChart(expenses, income, new BudgetEntity[] { }),
                IncomeSourcePercentages = GetIncomeSourcePercentages(income),
                SubCategoryPercentages = GetSubCategoryPercentages(expenses, incomeForPercentageCharts),
                MainCategoryPercentages = GetMainCategoryPercentages(expenses, incomeForPercentageCharts),
                MerchantPercentages = GetMerchantPercentages(expenses, incomeForPercentageCharts),
                ExpenseStatistics = AggregateUtilities<ExpenseEntity>.GetAnnualStatistics(expenses),
                IncomeStatistics = AggregateUtilities<IncomeEntity>.GetAnnualStatistics(income),
                TransactionBreakdown = GetTransactionBreakdown(expenses, income, new BudgetEntity[] { }, false),
                SummaryTotals = GetAllTimeSummaryTotals(expenses, income),
                AnnualSummaryChart = GetAllTimeAnnualSummaryChart(expenses, income),
                SavingsChart = GetAllTimeSavingsChart(expenses, income),
                IncomeExpenseChart = GetAllTimeIncomeExpenseChart(expenses, income),
                PercentChangesChart = GetAllTimePercentChanges(expenses, income)

            };
        }

        public async Task<AnnualSummaryResponse> GetAnnualSummaryAsync(AnnualSummaryRequest request)
        {
            var isCurrentYear = request.Year == DateTime.Now.Year;
            var user = await _userRepository.FindById(request.UserId);
            var expensesYTD = await _expenseRepo.Find(x => x.Date.Year == request.Year && !x.ExcludeFromStatistics);
            var incomeYTD = await _incomeRepo.Find(x => x.Date.Year == request.Year && !x.IsRefund);
            var annualBudgets = await _budgetRepo.FindWithMainCategories(x => x.Year == request.Year);
            var budgetsYTD = annualBudgets.Where(x => x.Month <= DateTime.Now.Month).ToArray();
            var budgetsForCharts = isCurrentYear ? budgetsYTD : annualBudgets;
            int incomeForPercentageCharts = incomeYTD.Any() ? Convert.ToInt32(incomeYTD.Sum(x => x.Amount)) : 0;

            if (!expensesYTD.Any() && !incomeYTD.Any() && !annualBudgets.Any())
                return EmptyAnnualSummaryResponse();
            else if (!expensesYTD.Any() && incomeYTD.Any() && !annualBudgets.Any())
                //income but no expenses
                return new AnnualSummaryResponse()
                {
                    LastImport = user.LastImport,
                    OverallSummaryChart = GetOverallSummaryChart(expensesYTD, incomeYTD, budgetsForCharts),
                    TopExpenses = GetTopExpenses(expensesYTD),
                    TopCategories = GetTopSubCategories(expensesYTD),
                    TopMerchants = GetTopMerchants(expensesYTD),
                    TopSources = GetTopSources(incomeYTD),
                    SavingsChart = GetAnnualSavingsChart(incomeYTD, expensesYTD, annualBudgets),
                    SubCategoryPercentages = GetSubCategoryPercentages(expensesYTD, incomeForPercentageCharts),
                    IncomeExpenseChart = GetAnnualIncomeExpenseChart(expensesYTD, incomeYTD, annualBudgets),
                    MainCategoryPercentages = GetMainCategoryPercentages(expensesYTD, incomeForPercentageCharts),
                    MerchantPercentages = GetMerchantPercentages(expensesYTD, incomeForPercentageCharts),
                    IncomeSourcePercentages = GetIncomeSourcePercentages(incomeYTD),
                    MonthlyExpenseStatistics = AggregateUtilities<ExpenseEntity>.GetAnnualStatisticsByMonth(expensesYTD, request.Year, true),
                    AnnualSummary = GetAnnualSummary(expensesYTD, incomeYTD, annualBudgets, isCurrentYear),
                    AnnualMonthlySummaryChart = GetAnnualMonthlySummaryChart(expensesYTD, incomeYTD, annualBudgets),
                    TransactionBreakdown = GetTransactionBreakdown(expensesYTD, incomeYTD, budgetsYTD, false)
                };
            else if (expensesYTD.Any() && !incomeYTD.Any() && !annualBudgets.Any())
                //expenses and no income, can still show data!
                return EmptyAnnualSummaryResponse();
            else if (!expensesYTD.Any() || !incomeYTD.Any() && annualBudgets.Any())
                //budget but no income or expenses
                return EmptyAnnualSummaryResponse();



            return new AnnualSummaryResponse()
            {
                LastImport = user.LastImport,
                OverallSummaryChart = GetOverallSummaryChart(expensesYTD, incomeYTD, budgetsForCharts),
                TopExpenses = GetTopExpenses(expensesYTD),
                TopCategories = GetTopSubCategories(expensesYTD),
                TopMerchants = GetTopMerchants(expensesYTD),
                TopSources = GetTopSources(incomeYTD),
                SavingsChart = GetAnnualSavingsChart(incomeYTD, expensesYTD, annualBudgets),
                SubCategoryPercentages = GetSubCategoryPercentages(expensesYTD, incomeForPercentageCharts),
                IncomeExpenseChart = GetAnnualIncomeExpenseChart(expensesYTD, incomeYTD, annualBudgets),
                MainCategoryPercentages = GetMainCategoryPercentages(expensesYTD, incomeForPercentageCharts),
                MerchantPercentages = GetMerchantPercentages(expensesYTD, incomeForPercentageCharts),
                IncomeSourcePercentages = GetIncomeSourcePercentages(incomeYTD),
                MonthlyExpenseStatistics = AggregateUtilities<ExpenseEntity>.GetAnnualStatisticsByMonth(expensesYTD, request.Year, true),
                AnnualSummary = GetAnnualSummary(expensesYTD, incomeYTD, annualBudgets, isCurrentYear),
                AnnualMonthlySummaryChart = GetAnnualMonthlySummaryChart(expensesYTD, incomeYTD, annualBudgets),
                TransactionBreakdown = GetTransactionBreakdown(expensesYTD, incomeYTD, budgetsYTD, false)
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
                OverallSummaryChart = GetOverallSummaryChart(monthlyExpenses, monthlyIncome, monthlyBudgets, isCurrentMonth),
                SubCategoryPercentages = GetSubCategoryPercentages(monthlyExpenses, incomeToCompare),
                MainCategoryPercentages = GetMainCategoryPercentages(monthlyExpenses, decimal.Round(incomeToCompare, 0)),
                MerchantPercentages = GetMerchantPercentages(monthlyExpenses, incomeToCompare),
                MonthlyProgress = GetMonthlyProgress(monthlySummary, request.Year, request.Month),
                AnnualSavingsProgress = GetAnnualSavingsProgress(annualBudgets, expensesYTD, incomeYTD, request.Year, request.Month),
                DailyExpenseLineChart = GetDailyExpenseLineChart(request.Month, request.Year, monthlyExpenses, monthlyBudgets, monthlyIncome),
                YearToDate = GetMonthlyYearToDate(expensesYTD, incomeYTD, request.Month),
                TopExpenses = GetTopExpenses(monthlyExpenses),
                TopCategories = GetTopSubCategories(monthlyExpenses),
                TopMerchants = GetTopMerchants(monthlyExpenses),
                TransactionBreakdown = GetTransactionBreakdown(monthlyExpenses, monthlyIncome, monthlyBudgets, isCurrentMonth),
                DailyExpenseChart = ChartUtilities.GetDailyExpenseData(monthlyExpenses, false)

            };
        }

        private static AllTimeAnnualPercentChanges GetAllTimePercentChanges(ExpenseEntity[] expenses, IncomeEntity[] income)
        {
            if (!income.Any() || !expenses.Any())
                return new AllTimeAnnualPercentChanges();

            var firstExpenseYear = expenses.Any() ? expenses.OrderBy(x => x.Date).Select(x => x.Date).Min().Year : 0;
            var firstIncomeYear = income.Any() ? income.OrderBy(x => x.Date).Select(x => x.Date).Min().Year : 0;
            var firstYear = firstExpenseYear < firstIncomeYear ? firstExpenseYear : firstIncomeYear;

            var years = Enumerable.Range(firstYear, (DateTime.Now.Year - firstYear) + 1).ToArray();
            var yearsToZip = years.Prepend(0).SkipLast(1);
            var labels = years.Zip(yearsToZip, (a, b) => b > 0 ? $"{b.ToString().Substring(2)}-{a.ToString().Substring(2)}" : "0").Where(x => x != "0").ToArray();

            var expenseData = expenses.GroupBy(x => x.Date.Year).OrderBy(x => x.Key).Select(x => (Year: x.Key, Sum: x.Sum(x => x.Amount))).ToList();
            for (int i = years.Min(); i <= years.Max(); i++)
            {
                if (!expenseData.Any(x => x.Year == i))
                    expenseData.Add((Year: i, Sum: 0));
            }
            var expenseAmount = expenseData.OrderBy(x => x.Year).Select(x => x.Sum).ToArray();

            var expensePercentChanges = new List<decimal>();
            for (int i = 1; i < expenseAmount.Length; i++)
            {
                var lastValue = 0m;

                lastValue = expenseAmount[i - 1] == 0 ? lastValue : expenseAmount[i - 1];
                if (lastValue != 0M)
                {
                    expensePercentChanges.Add(decimal.Round(((expenseAmount[i] - lastValue) / lastValue * 100), 1));
                }
                else if (lastValue == 0M)
                {
                    expensePercentChanges.Add(100m);
                }
            }

            var incomeData = income.GroupBy(x => x.Date.Year).OrderBy(x => x.Key).Select(x => (Year: x.Key, Sum: x.Sum(x => x.Amount))).ToList();
            for (int i = years.Min(); i <= years.Max(); i++)
            {
                if (!incomeData.Any(x => x.Year == i))
                    incomeData.Add((Year: i, Sum: 0));
            }
            var incomeAmount = incomeData.OrderBy(x => x.Year).Select(x => x.Sum).ToArray();
            var incomePercentChanges = new List<decimal>();
            for (int i = 1; i < incomeAmount.Length; i++)
            {
                var lastValue = 0m;

                lastValue = incomeAmount[i - 1] == 0 ? lastValue : incomeAmount[i - 1];
                if (lastValue != 0M)
                {
                    incomePercentChanges.Add(decimal.Round(((incomeAmount[i] - lastValue) / lastValue * 100), 1));
                }
                else if (lastValue == 0M)
                {
                    if (lastValue == 0m && incomeAmount[i] == 0m)
                        incomePercentChanges.Add(decimal.MaxValue);
                    else
                        incomePercentChanges.Add(100M);
                }
            }


            var calculatedSavings = incomeAmount.Zip(expenseAmount, (a, b) => (a - b)).ToArray();

            var savingsPercentChanges = new List<decimal>();
            for (int i = 1; i < calculatedSavings.Length; i++)
            {
                var lastValue = 0m;

                lastValue = calculatedSavings[i - 1] == 0 ? lastValue : calculatedSavings[i - 1];
                if (lastValue != 0M)
                {
                    savingsPercentChanges.Add(decimal.Round(((calculatedSavings[i] - lastValue) / lastValue * 100), 1));
                }
                else if (lastValue == 0M)
                {
                    if (lastValue == 0m && calculatedSavings[i] == 0m)
                        savingsPercentChanges.Add(decimal.MaxValue);
                    else
                        savingsPercentChanges.Add(100M);
                }
            }


            return new AllTimeAnnualPercentChanges()
            {
                ExpenseDataset = expensePercentChanges.Count > 0 ? JsonSerializer.Serialize(expensePercentChanges) : string.Empty,
                IncomeDataset = incomePercentChanges.Count > 0 ? JsonSerializer.Serialize(incomePercentChanges) : string.Empty,
                SavingsDataset = savingsPercentChanges.Count > 0 ? JsonSerializer.Serialize(savingsPercentChanges) : string.Empty,
                Labels = labels.Count() > 0 ? JsonSerializer.Serialize(labels) : string.Empty,
            };

        }

        private static AllTimeIncomeExpenseChart GetAllTimeIncomeExpenseChart(ExpenseEntity[] expenses, IncomeEntity[] income)
        {
            if (!income.Any())
                return new AllTimeIncomeExpenseChart();
            if (!expenses.Any())
                return new AllTimeIncomeExpenseChart();

            var firstExpenseYear = expenses.Any() ? expenses.OrderBy(x => x.Date).Select(x => x.Date).Min().Year : 0;
            var firstIncomeYear = income.Any() ? income.OrderBy(x => x.Date).Select(x => x.Date).Min().Year : 0;
            var firstYear = firstExpenseYear < firstIncomeYear ? firstExpenseYear : firstIncomeYear;

            var labels = Enumerable.Range(firstYear, (DateTime.Now.Year - firstYear) + 1).ToArray();
            var incomeData = income.GroupBy(x => x.Date.Year).OrderBy(x => x.Key).Select(x => (Year: x.Key, Sum: x.Sum(x => x.Amount))).ToList();
            var expenseData = expenses.GroupBy(x => x.Date.Year).OrderBy(x => x.Key).Select(x => (Year: x.Key, Sum: x.Sum(x => x.Amount))).ToList();

            for (var i = labels.Min(); i <= labels.Max(); i++)
            {
                if (!incomeData.Any(x => x.Year == i))
                    incomeData.Add((Year: i, Sum: 0M));
                if (!expenseData.Any(x => x.Year == i))
                    expenseData.Add((Year: i, Sum: 0M));
            }

            var incomes = incomeData.OrderBy(x => x.Year).Select(x => x.Sum).ToList();
            var expense = expenseData.OrderBy(x => x.Year).Select(x => x.Sum).ToList();

            return new AllTimeIncomeExpenseChart()
            {
                ExpenseDataset = expense.Sum() > 0 ? JsonSerializer.Serialize(expense) : string.Empty,
                IncomeDataset = incomes.Sum() > 0 ? JsonSerializer.Serialize(incomes) : string.Empty,
                Labels = labels.Sum() > 0 ? JsonSerializer.Serialize(labels) : string.Empty
            };
        }

        private static AllTimeSavingsChart GetAllTimeSavingsChart(ExpenseEntity[] expenses, IncomeEntity[] income)
        {
            if (!income.Any() || !expenses.Any())
                return new AllTimeSavingsChart();

            var firstExpenseYear = expenses.Any() ? expenses.OrderBy(x => x.Date).Select(x => x.Date).Min().Year : 0;
            var firstIncomeYear = income.Any() ? income.OrderBy(x => x.Date).Select(x => x.Date).Min().Year : 0;
            var firstYear = firstExpenseYear < firstIncomeYear ? firstExpenseYear : firstIncomeYear;

            var labels = Enumerable.Range(firstYear, (DateTime.Now.Year - firstYear) + 1).ToArray();

            var incomeData = income.GroupBy(x => x.Date.Year).OrderBy(x => x.Key).Select(x => (Year: x.Key, Sum: x.Sum(x => x.Amount))).ToList();
            var expenseData = expenses.GroupBy(x => x.Date.Year).OrderBy(x => x.Key).Select(x => (Year: x.Key, Sum: x.Sum(x => x.Amount))).ToList();

            for (var i = labels.Min(); i <= labels.Max(); i++)
            {
                if (!incomeData.Any(x => x.Year == i))
                    incomeData.Add((Year: i, Sum: 0M));
                if (!expenseData.Any(x => x.Year == i))
                    expenseData.Add((Year: i, Sum: 0M));
            }

            var incomes = incomeData.OrderBy(x => x.Year).Select(x => x.Sum).ToList();
            var expense = expenseData.OrderBy(x => x.Year).Select(x => x.Sum).ToList();

            var calculatedSavings = incomes.Zip(expense, (a, b) => (a - b)).ToList();
            return new AllTimeSavingsChart()
            {
                SavingsDataset = calculatedSavings.Sum() > 0 ? JsonSerializer.Serialize(calculatedSavings) : string.Empty,
                Labels = calculatedSavings.Sum() > 0 ? JsonSerializer.Serialize(labels) : string.Empty
            };
        }

        private static AllTimeAnnualSummaryChart GetAllTimeAnnualSummaryChart(ExpenseEntity[] expense, IncomeEntity[] income)
        {
            if (!expense.Any() || !income.Any())
                return new AllTimeAnnualSummaryChart();

            var firstExpenseYear = expense.Any() ? expense.OrderBy(x => x.Date).Select(x => x.Date).Min().Year : 0;
            var firstIncomeYear = income.Any() ? income.OrderBy(x => x.Date).Select(x => x.Date).Min().Year : 0;
            var firstYear = firstExpenseYear < firstIncomeYear ? firstExpenseYear : firstIncomeYear;

            var numberOfYears = (DateTime.Now.Year - firstYear) + 1;
            var years = Enumerable.Range(firstYear, numberOfYears).ToArray();

            var incomeData = income.GroupBy(x => x.Date.Year).OrderBy(x => x.Key).Select(x => (Year: x.Key, Sum: x.Sum(x => x.Amount))).ToList();
            var expenseData = expense.GroupBy(x => x.Date.Year).OrderBy(x => x.Key).Select(x => (Year: x.Key, Sum: x.Sum(x => x.Amount))).ToList();

            for (var i = years.Min(); i <= years.Max(); i++)
            {
                if (!incomeData.Any(x => x.Year == i))
                    incomeData.Add((Year: i, Sum: 0M));
                if (!expenseData.Any(x => x.Year == i))
                    expenseData.Add((Year: i, Sum: 0M));
            }

            var incomes = incomeData.OrderBy(x => x.Year).Select(x => x.Sum).ToList();
            var expenses = expenseData.OrderBy(x => x.Year).Select(x => x.Sum).ToList();
            var calculatedSavings = incomes.Zip(expenses, (a, b) => (a - b)).ToList();

            return new AllTimeAnnualSummaryChart()
            {
                IncomeDataset = incomes.Sum() > 0M ? JsonSerializer.Serialize(incomes) : string.Empty,
                ExpenseDataset = expenses.Sum() > 0M ? JsonSerializer.Serialize(expenses) : string.Empty,
                SavingsDataset = calculatedSavings.Sum() > 0M ? JsonSerializer.Serialize(calculatedSavings) : string.Empty,
                Labels = JsonSerializer.Serialize(years)
            };
        }

        private static AllTimeSummaryTotals GetAllTimeSummaryTotals(ExpenseEntity[] expenses, IncomeEntity[] income)
        {

            var spent = expenses.Sum(s => s.Amount);
            var earned = income.Sum(e => e.Amount);
            var saved = (earned - spent);

            var averageSavedPerYear = 0M;
            var averageSavedPerMonth = 0m;
            if (spent > 0)
            {
                var dateOfFirstPurchase = expenses.OrderBy(x => x.Date).Select(x => x.Date).Min();
                var monthsSinceFirstPurchase = ((DateTime.Now.Year - dateOfFirstPurchase.Year) * 12) + DateTime.Now.Month - dateOfFirstPurchase.Month;
                averageSavedPerMonth = monthsSinceFirstPurchase > 0 ? decimal.Round((saved / monthsSinceFirstPurchase), 2) : saved;
                var yearsSinceFirstPurchase = (DateTime.Now.Year - dateOfFirstPurchase.Year);
                averageSavedPerYear = yearsSinceFirstPurchase > 0 ? (saved / yearsSinceFirstPurchase) : saved;
            }
            else if (spent == 0 && earned > 0)
            {
                var dateOfFirstIncome = income.OrderBy(x => x.Date).Select(x => x.Date).Min();
                var monthsSinceFirstIncome = ((DateTime.Now.Year - dateOfFirstIncome.Year) * 12) + DateTime.Now.Month - dateOfFirstIncome.Month;
                averageSavedPerMonth = monthsSinceFirstIncome > 0 ? decimal.Round((saved / monthsSinceFirstIncome), 2) : saved;
                var yearsSinceFirstIncome = (DateTime.Now.Year - dateOfFirstIncome.Year);
                averageSavedPerYear = yearsSinceFirstIncome > 0 ? (saved / yearsSinceFirstIncome) : saved;
            }

            var expenseYears = expenses.GroupBy(x => x.Date.Year).Select(x =>
            {
                return (Year: x.Key, Amount: x.Sum(x => x.Amount));
            }).Where(x => x.Amount > 0).ToDictionary(k => k.Year, v => v.Amount);
            //fill in missing years, if any
            for (int i = expenseYears.Keys.Min() + 1; i <= expenseYears.Keys.Max(); i++)
            {
                if (!expenseYears.ContainsKey(i))
                    expenseYears.Add(i, 0);
            }

            var expenseGrowthsPerYear = new List<decimal>();
            for (int i = expenseYears.Keys.Min() + 1; i <= expenseYears.Keys.Max(); i++)
            {
                if (expenseYears[i - 1] == 0M)
                    expenseGrowthsPerYear.Add(0M);
                else
                {
                    var percentGrowth = decimal.Round(
                        ((expenseYears[i] - expenseYears[i - 1]) / expenseYears[i - 1] * 100), 2);
                    expenseGrowthsPerYear.Add(percentGrowth);
                }
            }
            var averageExpenseGrowthPerYear = decimal.Round((expenseGrowthsPerYear.Sum() / expenseYears.Count), 2);

            var incomesYears = income.GroupBy(x => x.Date.Year).Select(x =>
            {
                return (Year: x.Key, Amount: x.Sum(x => x.Amount));
            }).Where(x => x.Amount > 0).ToDictionary(k => k.Year, v => v.Amount);
            //fill in missing years, if any
            for (int i = incomesYears.Keys.Min() + 1; i <= incomesYears.Keys.Max(); i++)
            {
                if (!incomesYears.ContainsKey(i))
                    incomesYears.Add(i, 0);
            }
            var incomeGrowthsPerYear = new List<decimal>();
            for (int i = incomesYears.Keys.Min() + 1; i <= incomesYears.Keys.Max(); i++)
            {
                if (incomesYears[i - 1] == 0M)
                    incomeGrowthsPerYear.Add(0M);
                else
                {
                    var percentGrowth = decimal.Round(
                        ((incomesYears[i] - incomesYears[i - 1]) / incomesYears[i - 1] * 100), 2);
                    incomeGrowthsPerYear.Add(percentGrowth);
                }
            }
            var averageIncomeGrowthPerYear = decimal.Round((incomeGrowthsPerYear.Sum() / incomesYears.Count), 2);

            return new AllTimeSummaryTotals()
            {
                Earned = earned,
                Spent = spent,
                Saved = saved,
                AverageSavedPerMonth = averageSavedPerMonth,
                AverageSavedPerYear = averageSavedPerYear,
                ExpenseGrowthPerYear = averageExpenseGrowthPerYear,
                IncomeGrowthPerYear = averageIncomeGrowthPerYear
            };
        }

        private static AnnualMonthlySummaryChart GetAnnualMonthlySummaryChart(ExpenseEntity[] expenses, IncomeEntity[] incomes, BudgetEntity[] budgets)
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
                SavingsDataset = calculatedSavings.Sum() != 0 ? JsonSerializer.Serialize(calculatedSavings) : string.Empty,
                BudgetedIncomeDataset = budgetedIncome.Sum() > 0 ? JsonSerializer.Serialize(budgetedIncomeDataset).Replace(decimal.MaxValue.ToString(), "NaN") : string.Empty,
                BudgetedExpenseDataset = budgetedExpenses.Sum() > 0 ? JsonSerializer.Serialize(budgetedExpenseDataset).Replace(decimal.MaxValue.ToString(), "NaN") : string.Empty,
                BudgetedSavingsDataset = budgetedSavings.Sum() > 0 ? JsonSerializer.Serialize(budgetedSavingsDataset).Replace(decimal.MaxValue.ToString(), "NaN") : string.Empty
            };
        }

        private static AnnualSummaryTotals GetAnnualSummary(ExpenseEntity[] expenses, IncomeEntity[] incomes, BudgetEntity[] budgets, bool isCurrentYear)
        {
            var totalSpent = expenses.Length == 0 ? 0 : expenses.Sum(x => x.Amount);
            var totalEarned = incomes.Length == 0 ? 0 : incomes.Sum(x => x.Amount);
            var totalSaved = totalEarned - totalSpent;
            var savingsGoal = budgets.Length == 0 ? 0 : budgets.Where(x => x.BudgetType == BudgetType.Savings).Sum(x => x.Amount);
            var budgetedExpenses = budgets.Length == 0 ? 0 : budgets.Where(x => x.BudgetType == BudgetType.Want || x.BudgetType == BudgetType.Need).Sum(x => x.Amount);
            var totalSavedAsInt = (int)decimal.Round(totalSaved, 0);
            var percentTowardsSavingsGoal = totalSavedAsInt < savingsGoal ? totalSavedAsInt.ToPercentage(savingsGoal) : 100;

            var suggestedMonthlyAmount = 0;
            var averageAmountSaved = 0;
            if (isCurrentYear)
            {
                var lastMonthOfIncome = incomes.OrderBy(x => x.Date.Month).Select(x => x.Date.Month).LastOrDefault() + 1;
                var iterations = 13 - lastMonthOfIncome;
                suggestedMonthlyAmount = iterations > 0 && savingsGoal > 0 ? (savingsGoal - totalSavedAsInt) / iterations : (savingsGoal - totalSavedAsInt);
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
                SuggestedMonthlySavingsToMeetGoal = savingsGoal > 0 ? suggestedMonthlyAmount : 0,
                AveragedSavedPerMonth = averageAmountSaved,
                BudgetVariance = budgetedExpenses > 0 ? ((int)decimal.Round(totalSpent, 0) - budgetedExpenses) / budgetedExpenses : 0
            };
        }

        private static AnnualIncomeExpenseChart GetAnnualIncomeExpenseChart(ExpenseEntity[] expenses, IncomeEntity[] incomes, BudgetEntity[] annualBudgets)
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
            return new AnnualIncomeExpenseChart()
            {
                Labels = JsonSerializer.Serialize(labels),
                IncomeDataset = JsonSerializer.Serialize(incomeDataset),
                MonthBudgetIncomeDataBegins = lastIncomeMonth - 1,
                ExpensesDataset = JsonSerializer.Serialize(expenseDataset),
                MonthBudgetExpenseDataBegins = lastExpenseMonth - 1
            };
        }

        private static AnnualSavingsChart GetAnnualSavingsChart(IncomeEntity[] incomes, ExpenseEntity[] expenses, BudgetEntity[] budgets)
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

            return new AnnualSavingsChart()
            {
                SavingsDataset = JsonSerializer.Serialize(savingsDataset),
                Labels = JsonSerializer.Serialize(labels),
                MonthBudgetDataBegins = lastMonth - 1,
                SuggestedSavingsDataset = suggestedSavingsDataset
            };
        }

        private static List<TransactionBreakdown> GetTransactionBreakdown(ExpenseEntity[] expenses, IncomeEntity[] income, BudgetEntity[] budgets, bool isCurrentMonth)
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
                Percentage = 100
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
                Percentage = expenses.Sum(x => x.Amount) < incomeCategories.Sum(x => x.Amount) ? expenses.Sum(x => x.Amount).ToDecimalPercentage(incomeCategories.Sum(x => x.Amount)) : 100
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
        private static Dictionary<string, decimal> GetMainCategoryPercentages(ExpenseEntity[] expenses, decimal income)
        {
            if (expenses.Length == 0)
                return new Dictionary<string, decimal>();

            var expenseAmount = expenses.Sum(x => x.Amount);
            var amountToPercentBy = expenseAmount > income ? expenseAmount : income;

            var expensePercentages = expenses.Where(x => !x.ExcludeFromStatistics).OrderBy(x => x.Category.MainCategoryId).GroupBy(x => x.Category.MainCategory.Name).Select(x => (Name: x.Key, Amount: x.Sum(x => x.Amount))).Select(x =>
                (x.Name, Percentage: x.Amount.ToDecimalPercentage(amountToPercentBy)))
                .Where(x => x.Percentage > 0).OrderByDescending(x => x.Percentage).ToDictionary(k => k.Name, v => v.Percentage);


            var savingsPercentage = income > expenseAmount ? (income - expenseAmount).ToPercentage(income) : 0;

            if (savingsPercentage > 0)
                expensePercentages.Add("Savings", savingsPercentage);
            return expensePercentages;
        }

        private static Dictionary<string, int> GetSubCategoryPercentages(ExpenseEntity[] expenses, int income)
        {
            if (expenses.Length == 0)
                return new Dictionary<string, int>();

            var expenseAmount = expenses.Sum(x => x.Amount);
            var amountToPercentBy = expenseAmount > income ? (int)decimal.Round(expenseAmount, 0) : income;
            var expensePercentages = expenses.OrderBy(x => x.Category.MainCategoryId).Where(x => !x.ExcludeFromStatistics).GroupBy(x => x.Category.Name).Select(x => (Name: x.Key, Amount: x.Sum(x => x.Amount)))
                .Select(x => (x.Name, Percentage: x.Amount.ToPercentage(amountToPercentBy)))
                .Where(x => x.Percentage > 0).OrderByDescending(x => x.Percentage).ToDictionary(k => k.Name, v => v.Percentage);

            var savingsPercentage = income > expenseAmount ? (income - expenseAmount).ToPercentage(income) : 0;

            if (savingsPercentage > 0)
                expensePercentages.Add("Savings", savingsPercentage);

            return expensePercentages;
        }
        private static Dictionary<string, int> GetMerchantPercentages(ExpenseEntity[] expenses, int income)
        {
            if (expenses.Length == 0)
                return new Dictionary<string, int>();

            var expenseAmount = expenses.Sum(x => x.Amount);
            var amountToPercentBy = expenseAmount > income ? decimal.Round(expenseAmount, 0) : income;

            var expensePercentages = expenses.Where(x => !x.ExcludeFromStatistics && x.MerchantId != null).OrderBy(x => x.Merchant.Name).GroupBy(x => x.Merchant.Name).Select(x => (Name: x.Key, Amount: x.Sum(x => x.Amount))).Select(x =>
                (x.Name, Percentage: x.Amount.ToPercentage(amountToPercentBy)))
                .Where(x => x.Percentage > 0).OrderByDescending(x => x.Percentage).ToDictionary(k => k.Name, v => v.Percentage);

            var expenseTotalsWithMerchantsAssigned = expensePercentages.Sum(x => x.Value);

            var noMerchantPercentage = expenseTotalsWithMerchantsAssigned < 100 ? 100 - expenseTotalsWithMerchantsAssigned : 0;
            if (noMerchantPercentage > 0)
                expensePercentages.Add("No Merchant Assigned", noMerchantPercentage);
            return expensePercentages;
        }
        private static Dictionary<string, decimal> GetIncomeSourcePercentages(IncomeEntity[] income)
        {
            if (income.Length == 0)
                return new Dictionary<string, decimal>();

            var incomeAmount = income.Sum(x => x.Amount);

            var incomePercentages = income.Where(x => !x.IsRefund && x.Category != null).OrderBy(x => x.Category.Name).GroupBy(x => x.Category.Name).Select(x => (Name: x.Key, Amount: x.Sum(x => x.Amount))).Select(x =>
                (x.Name, Percentage: x.Amount.ToDecimalPercentage(incomeAmount)))
                .Where(x => x.Percentage > 0).OrderByDescending(x => x.Percentage).ToDictionary(k => k.Name, v => v.Percentage);

            var incomeWithCategories = incomePercentages.Sum(x => x.Value);

            var noCategoryPercentage = incomeWithCategories < 100 ? 100 - incomeWithCategories : 0;
            if (noCategoryPercentage > 0)
                incomePercentages.Add("No Category Assigned", noCategoryPercentage);
            return incomePercentages;
        }

        private static MonthlyYearToDate GetMonthlyYearToDate(ExpenseEntity[] expenses, IncomeEntity[] incomes, int month)
        {
            var labels = Enumerable.Range(1, month).Select(i => @CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i)).ToArray();

            var incomeData = incomes.GroupBy(x => x.Date.Month).OrderBy(x => x.Key).Select(x => (int)decimal.Round(x.Sum(x => x.Amount), 0)).ToArray();

            var expenseData = expenses.GroupBy(x => x.Date.Month).OrderBy(x => x.Key).Select(x => (int)decimal.Round(x.Sum(x => x.Amount), 0)).ToArray();
            var savings = incomeData.Zip(expenseData, (a, b) => (a - b)).ToArray().Accumulate();
            return new MonthlyYearToDate()
            {
                IncomeDataset = incomeData.Accumulate(),
                ExpenseDataset = expenseData.Accumulate(),
                SavingsDataset = savings,
                Labels = labels
            };
        }

        private static DailyExpenseChart GetDailyExpenseLineChart(int month, int year, ExpenseEntity[] expenses, BudgetEntity[] budgets, IncomeEntity[] income)
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

        private static AnnualSavingsProgress GetAnnualSavingsProgress(BudgetEntity[] budgets, ExpenseEntity[] expenses, IncomeEntity[] income, int year, int month)
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
                    if (DateTime.Now.Month == month && extraOneTimeSavings > 0)
                    {
                        message = $"Save an extra ${extraOneTimeSavings} this month to meet your goal";
                    }
                    else if (extraOneTimeSavings > 0)
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

        private static MonthlyProgress GetMonthlyProgress(MonthlySummary summary, int year, int month)
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

        private static OverallSummaryChart GetOverallSummaryChart(ExpenseEntity[] expenses, IncomeEntity[] income, BudgetEntity[] budgets, bool isCurrentMonth = false)
        {
            var realizedIncome = income.Sum(x => x.Amount);
            var realizedExpenses = expenses.Sum(x => x.Amount);
            var budgetedIncome = budgets.Where(x => x.BudgetType == BudgetType.Income).Sum(x => x.Amount);
            var incomeToCompareBy = isCurrentMonth ? budgetedIncome : realizedIncome;
            var realizedSavings = incomeToCompareBy - realizedExpenses;
            var budgetedExpenses = budgets.Where(x => x.BudgetType == BudgetType.Need || x.BudgetType == BudgetType.Want).Sum(x => x.Amount);
            var budgetedSavings = budgets.Where(x => x.BudgetType == BudgetType.Savings).Sum(x => x.Amount);
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

        internal static ExpenseSummaryChartData GetExpenseSummaryChartData(ExpenseEntity[] expenses, BudgetEntity[] budgets)
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

        internal static MonthlySummary GetMonthlySummary(ExpenseEntity[] expenses, IncomeEntity[] income, BudgetEntity[] budgets, int Year, int Month)
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
            var estimatedSavings = budgetedIncome > 0 && dippingIntoRealSavings ?
                (incomeToCompareBy - realizedExpenses) :
                budgetedIncome > 0 && dippingIntoBudgetedSavings ?
                realizedSavings :
                budgetedIncome > 0 ?
                budgetedSavings + unspent : incomeToCompareBy - realizedExpenses;
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

        public async Task<List<TransactionBreakdown>> GetTransactionsToPrint(PrintTransactionsRequest request)
        {
            if (request.AllTimeSummary)
            {
                return GetTransactionBreakdown(await _expenseRepo.Find(x => !x.ExcludeFromStatistics), await _incomeRepo.Find(x => !x.IsRefund), new BudgetEntity[] { }, false);
            }

            if (request.Year < 1900)
                return new List<TransactionBreakdown>();

            var validatedMonth = request.Month > 0 && request.Month <= 12 ? request.Month : 0;
            var isCurrentMonth = validatedMonth == DateTime.Now.Month && DateTime.Now.Year == request.Year;

            var expenses = validatedMonth > 0 ? await _expenseRepo.Find(x => x.Date.Year == request.Year && x.Date.Month == request.Month && !x.ExcludeFromStatistics) : await _expenseRepo.Find(x => x.Date.Year == request.Year && !x.ExcludeFromStatistics);

            var income = validatedMonth > 0 ? await _incomeRepo.Find(x => x.Date.Year == request.Year && x.Date.Month == request.Month && !x.IsRefund) : await _incomeRepo.Find(x => x.Date.Year == request.Year && !x.IsRefund);

            var budgets = validatedMonth > 0 ? await _budgetRepo.FindWithMainCategories(x => x.Year == request.Year && x.Month == request.Month && x.BudgetType == BudgetType.Need || x.BudgetType == BudgetType.Want) : await _budgetRepo.FindWithMainCategories(x => x.Year == request.Year && x.BudgetType == BudgetType.Need || x.BudgetType == BudgetType.Want);

            return GetTransactionBreakdown(expenses, income, budgets, isCurrentMonth);
        }
        private static List<IncomeSourceQuickView> GetTopSources(IncomeEntity[] income)
        {
            if (income.Length == 0)
                return new List<IncomeSourceQuickView>();

            return income.Where(x => !x.IsRefund && x.SourceId != null)
                    .GroupBy(x => x.Source.Name)
                    .Select(x => new IncomeSourceQuickView()
                    {
                        Name = x.Key,
                        Amount = x.Sum(x => x.Amount),
                        Count = x.Count()
                    }).OrderByDescending(x => x.Amount).Take(10).ToList();
        }
        private static List<MerchantQuickView> GetTopMerchants(ExpenseEntity[] expenses)
        {
            if (expenses.Length == 0)
                return new List<MerchantQuickView>();

            return expenses.Where(x => !x.ExcludeFromStatistics && x.MerchantId != null)
                .GroupBy(x => x.Merchant.Name)
                .Select(x => new MerchantQuickView()
                {
                    Amount = x.Sum(y => y.Amount),
                    Name = x.Key,
                    Count = x.Count()
                }).OrderByDescending(x => x.Amount)
                .Take(10).ToList();
        }
        private static List<SubCategoryQuickView> GetTopSubCategories(ExpenseEntity[] expenses)
        {
            if (expenses.Length == 0)
                return new List<SubCategoryQuickView>();

            return expenses.Where(x => !x.ExcludeFromStatistics)
                .GroupBy(x => x.Category.Name)
                .Select(x => new SubCategoryQuickView()
                {
                    Amount = x.Sum(y => y.Amount),
                    Name = x.Key,
                    Count = x.Count()
                }).OrderByDescending(x => x.Amount)
                .Take(10).ToList();
        }
        private static List<ExpenseQuickView> GetTopExpenses(ExpenseEntity[] expenses)
        {
            if (expenses.Length == 0)
                return new List<ExpenseQuickView>();

            return expenses.Where(x => !x.ExcludeFromStatistics)
                    .Where(x => x.Category.MainCategory.Name != "Rent") //this is mostly for the demo app...
                    .OrderByDescending(x => x.Amount)
                    .Take(10)
                    .Select(x => new ExpenseQuickView()
                    {
                        Amount = x.Amount,
                        Date = x.Date.ToShortDateString(),
                        Id = x.Id,
                        SubCategory = x.Category == null ? "none" : x.Category.Name
                    }).ToList();
        }
        private static AnnualSummaryResponse EmptyAnnualSummaryResponse() => new AnnualSummaryResponse()
        {
            LastImport = DateTime.MinValue,
            TransactionBreakdown = new List<TransactionBreakdown>(),
            OverallSummaryChart = new OverallSummaryChart(),
            TopCategories = new List<SubCategoryQuickView>(),
            TopMerchants = new List<MerchantQuickView>(),
            TopExpenses = new List<ExpenseQuickView>(),
            TopSources = new List<IncomeSourceQuickView>(),
            SavingsChart = new AnnualSavingsChart(),
            IncomeExpenseChart = new AnnualIncomeExpenseChart(),
            AnnualSummary = new AnnualSummaryTotals(),
            SubCategoryPercentages = new Dictionary<string, int>(),
            MainCategoryPercentages = new Dictionary<string, decimal>(),
            MerchantPercentages = new Dictionary<string, int>(),
            IncomeSourcePercentages = new Dictionary<string, decimal>(),
            MonthlyExpenseStatistics = new List<MonthlyStatistics>(),
            AnnualMonthlySummaryChart = new AnnualMonthlySummaryChart(),
        };
        private static AllTimeSummaryResponse EmptyAllTimeSummaryResponse() => new AllTimeSummaryResponse()
        {
            DataSpansMultipleYears = false,
            SummaryTotals = new AllTimeSummaryTotals(),
            TransactionBreakdown = new List<TransactionBreakdown>(),
            OverallSummaryChart = new OverallSummaryChart(),
            TopCategories = new List<SubCategoryQuickView>(),
            TopMerchants = new List<MerchantQuickView>(),
            TopExpenses = new List<ExpenseQuickView>(),
            SavingsChart = new AllTimeSavingsChart(),
            IncomeExpenseChart = new AllTimeIncomeExpenseChart(),
            SubCategoryPercentages = new Dictionary<string, int>(),
            MainCategoryPercentages = new Dictionary<string, decimal>(),
            MerchantPercentages = new Dictionary<string, int>(),
            IncomeSourcePercentages = new Dictionary<string, decimal>(),
            PercentChangesChart = new AllTimeAnnualPercentChanges(),
            AnnualSummaryChart = new AllTimeAnnualSummaryChart(),
            ExpenseStatistics = new List<AnnualStatistics>(),
            IncomeStatistics = new List<AnnualStatistics>(),
            TopSources = new List<IncomeSourceQuickView>()
        };
    }
}
