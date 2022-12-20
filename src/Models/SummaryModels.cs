using CashTrack.Models.BudgetModels;
using CashTrack.Models.Common;
using CashTrack.Models.ExpenseModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.Common;
using System;
using System.Collections.Generic;

namespace CashTrack.Models.SummaryModels
{
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
        public SavingsChart SavingsChart { get; set; }
        public IncomeExpenseChart IncomeExpenseChart { get; set; }
        public AnnualSummaryTotals AnnualSummary { get; set; }
        public Dictionary<string, int> SubCategoryPercentages { get; set; }
        public Dictionary<string, int> MainCategoryPercentages { get; set; }
        public Dictionary<string, int> MerchantPercentages { get; set; }
        public Dictionary<string, decimal> IncomeSourcePercentages { get; set; }
        public List<MonthlyStatistics> MonthlyExpenseStatistics { get; set; }

        //public List<TransactionBreakdown> TransactionBreakdown { get; set; }
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
    public record IncomeExpenseChart
    {
        public string Labels { get; set; }
        public string IncomeDataset { get; set; }
        public int MonthBudgetIncomeDataBegins { get; set; }
        public string ExpensesDataset { get; set; }
        public int MonthBudgetExpenseDataBegins { get; set; }
    }
    public record SavingsChart
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
        public Dictionary<string, int> MainCategoryPercentages { get; set; }
        public Dictionary<string, int> MerchantPercentages { get; set; }
        public DailyExpenseChart DailyExpenseLineChart { get; set; }
        public MonthlyYearToDate YearToDate { get; set; }
        public List<ExpenseQuickView> TopExpenses { get; set; }
        public List<TransactionBreakdown> TransactionBreakdown { get; set; }
    }
    public record TransactionBreakdown
    {
        public int MainCategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
        public bool IsMainCategory => SubCategoryId == 0 && MainCategoryId != 0;
        public bool IsIncome => Category == "Income";
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
        public int[] BudgetedIncome { get; set; }
        public decimal[] RealizedIncome { get; set; }
        public int[] BudgetedExpenses { get; set; }
        public decimal[] RealizedExpenses { get; set; }
        public int[] BudgetedSavings { get; set; }
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
}
