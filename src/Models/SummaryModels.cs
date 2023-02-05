using CashTrack.Common.Extensions;
using CashTrack.Models.BudgetModels;
using CashTrack.Models.ExpenseModels;
using CashTrack.Models.IncomeSourceModels;
using CashTrack.Models.MerchantModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Services.Common;
using System;
using System.Collections.Generic;

namespace CashTrack.Models.SummaryModels
{
    public record AllTimeSummaryResponse
    {
        public bool DataSpansMultipleYears { get; set; }
        public AllTimeSummaryTotals SummaryTotals { get; set; }
        public OverallSummaryChart OverallSummaryChart { get; set; }
        public List<ExpenseQuickView> TopExpenses { get; set; }
        public List<SubCategoryQuickView> TopCategories { get; set; }
        public List<MerchantQuickView> TopMerchants { get; set; }
        public List<IncomeSourceQuickView> TopSources { get; set; }
        public Dictionary<string, int> SubCategoryPercentages { get; set; }
        public Dictionary<string, decimal> MainCategoryPercentages { get; set; }
        public Dictionary<string, int> MerchantPercentages { get; set; }
        public Dictionary<string, decimal> IncomeSourcePercentages { get; set; }
        public List<TransactionBreakdown> TransactionBreakdown { get; set; }
        public List<AnnualStatistics> ExpenseStatistics { get; set; }
        public List<AnnualStatistics> IncomeStatistics { get; set; }
        public AllTimeAnnualSummaryChart AnnualSummaryChart { get; set; }
        public AllTimeSavingsChart SavingsChart { get; set; }
        public AllTimeIncomeExpenseChart IncomeExpenseChart { get; set; }
        public AllTimeAnnualPercentChanges PercentChangesChart { get; set; }
    }
    public record AllTimeAnnualPercentChanges
    {
        public string SavingsDataset { get; set; }
        public string IncomeDataset { get; set; }
        public string ExpenseDataset { get; set; }
        public string Labels { get; set; }
    }
    public record AllTimeIncomeExpenseChart
    {
        public string IncomeDataset { get; set; }
        public string ExpenseDataset { get; set; }
        public string Labels { get; set; }
    }
    public record AllTimeSavingsChart
    {
        public string SavingsDataset { get; set; }
        public string Labels { get; set; }
    }
    public record AllTimeAnnualSummaryChart
    {
        public string IncomeDataset { get; set; }
        public string ExpenseDataset { get; set; }
        public string SavingsDataset { get; set; }
        public string Labels { get; set; }
    }
    public record AllTimeSummaryTotals
    {
        public decimal Earned { get; set; }
        public decimal Spent { get; set; }
        public decimal Saved { get; set; }
        public decimal AverageSavedPerMonth { get; set; }
        public decimal AverageSavedPerYear { get; set; }
        public decimal ExpenseGrowthPerYear { get; set; }
        public decimal IncomeGrowthPerYear { get; set; }

    }
    public record PrintTransactionsRequest
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public bool AllTimeSummary => this.Year == default && this.Month == default;
    }
    public record AnnualSummaryRequest
    {
        public int UserId { get; set; }
        public int Year { get; set; }

    }
    public record AnnualSummaryResponse
    {
        public DateTime LastImport { get; set; }
        public OverallSummaryChart OverallSummaryChart { get; set; }
        public List<ExpenseQuickView> TopExpenses { get; set; }
        public List<SubCategoryQuickView> TopCategories { get; set; }
        public List<MerchantQuickView> TopMerchants { get; set; }
        public List<IncomeSourceQuickView> TopSources { get; set; }
        public AnnualSavingsChart SavingsChart { get; set; }
        public AnnualIncomeExpenseChart IncomeExpenseChart { get; set; }
        public AnnualSummaryTotals AnnualSummary { get; set; }
        public Dictionary<string, int> SubCategoryPercentages { get; set; }
        public Dictionary<string, decimal> MainCategoryPercentages { get; set; }
        public Dictionary<string, int> MerchantPercentages { get; set; }
        public Dictionary<string, decimal> IncomeSourcePercentages { get; set; }
        public List<MonthlyStatistics> MonthlyExpenseStatistics { get; set; }
        public AnnualMonthlySummaryChart AnnualMonthlySummaryChart { get; set; }
        public List<TransactionBreakdown> TransactionBreakdown { get; set; }
    }
    public record AnnualMonthlySummaryChart
    {
        public string IncomeDataset { get; set; }
        public string ExpenseDataset { get; set; }
        public string SavingsDataset { get; set; }
        public string BudgetedIncomeDataset { get; set; }
        public string BudgetedExpenseDataset { get; set; }
        public string BudgetedSavingsDataset { get; set; }
    }
    public record AnnualSummaryTotals
    {
        public decimal Earned { get; set; }
        public decimal Spent { get; set; }
        public decimal Saved { get; set; }
        public decimal SavingsGoalProgress { get; set; }
        public int SuggestedMonthlySavingsToMeetGoal { get; set; }
        public int AveragedSavedPerMonth { get; set; }
        public int BudgetVariance { get; set; }
    }
    public record AnnualIncomeExpenseChart
    {
        public string Labels { get; set; }
        public string IncomeDataset { get; set; }
        public int MonthBudgetIncomeDataBegins { get; set; }
        public string ExpensesDataset { get; set; }
        public int MonthBudgetExpenseDataBegins { get; set; }
    }
    public record AnnualSavingsChart
    {
        public string SavingsDataset { get; set; }
        public string SuggestedSavingsDataset { get; set; }
        public string Labels { get; set; }
        public int MonthBudgetDataBegins { get; set; }
    }
    public record MonthlySummaryRequest
    {
        public int UserId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

    }
    public record MonthlySummaryResponse
    {
        public DateTime LastImport { get; set; }
        public MonthlyProgress MonthlyProgress { get; set; }
        public AnnualSavingsProgress AnnualSavingsProgress { get; set; }
        public ExpenseSummaryChartData ExpenseSummaryChart { get; set; }
        public MonthlySummary MonthlySummary { get; set; }
        public OverallSummaryChart OverallSummaryChart { get; set; }
        public Dictionary<string, int> SubCategoryPercentages { get; set; }
        public Dictionary<string, decimal> MainCategoryPercentages { get; set; }
        public Dictionary<string, int> MerchantPercentages { get; set; }
        public DailyExpenseChart DailyExpenseLineChart { get; set; }
        public MonthlyYearToDate YearToDate { get; set; }
        public List<ExpenseQuickView> TopExpenses { get; set; }
        public List<SubCategoryQuickView> TopCategories { get; set; }
        public List<MerchantQuickView> TopMerchants { get; set; }
        public List<TransactionBreakdown> TransactionBreakdown { get; set; }
        public List<DailyExpenseDataset> DailyExpenseChart { get; set; }
    }
    public record TransactionBreakdown
    {
        public int MainCategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
        public bool IsMainCategory => SubCategoryId == 0 && MainCategoryId != 0;
        public bool IsIncome => Category.IsEqualTo("Income");
    }
    public class MonthlyYearToDate
    {
        public int[] IncomeDataset { get; set; }
        public int[] ExpenseDataset { get; set; }
        public int[] SavingsDataset { get; set; }
        public string[] Labels { get; set; }
    }

    public class DailyExpenseChart
    {
        public decimal[] Dataset { get; set; }
        public int[] Labels { get; set; }
        public int ExpenseBudgetMax { get; set; }
        public int DiscretionarySpendingMax { get; set; }
        public int IncomeMax { get; set; }
        public int ExpenseMax { get; set; }
        public int Max { get; set; }
    }

    public record MonthlyProgress
    {
        public int RealizedIncome { get; set; }
        public string RealizedIncomeMessage { get; set; }
        public int BudgetedExpenses { get; set; }
        public int BudgetedSavings { get; set; }
        public int DiscretionarySpendingPercent { get; set; }
        public decimal DiscretionarySpendingAmount { get; set; }
    }
    public record AnnualSavingsProgress
    {
        public int AnnualSavingsPercentDone { get; set; }
        public decimal AnnualSavingsAmount { get; set; }
        public string AnnualSavingsMessage { get; set; }
    }
    public record OverallSummaryChart
    {
        public string[] Labels { get; set; } = new string[] { "Income", "Expenses", "Savings" };
        public string BudgetedIncome { get; set; }
        public decimal[] RealizedIncome { get; set; }
        public string BudgetedExpenses { get; set; }
        public decimal[] RealizedExpenses { get; set; }
        public string BudgetedSavings { get; set; }
        public decimal[] RealizedSavings { get; set; }

    }
    public record ExpenseSummaryChartData
    {
        public string[] Labels { get; set; }
        public List<ExpenseDataset> BudgetedExpenses { get; set; }
        public List<ExpenseDataset> RealizedExpenses { get; set; }
    }

    public record MonthlySummary
    {
        public decimal RealizedIncome { get; set; }
        public int BudgetedIncome { get; set; }
        public decimal RealizedExpenses { get; set; }
        public int BudgetedExpenses { get; set; }
        public decimal RealizedSavings { get; set; }
        public int BudgetedSavings { get; set; }
        public decimal Unspent { get; set; }
        public decimal EstimatedSavings { get; set; }
        public decimal BudgetVariance { get; set; }
    }
    public class DailyExpenseDataset
    {
        public string SubCategoryName { get; set; }
        public decimal[] DataSet { get; set; }
        public string Color { get; set; }
    }
}
