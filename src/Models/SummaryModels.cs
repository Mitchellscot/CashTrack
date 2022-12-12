using CashTrack.Models.BudgetModels;
using CashTrack.Models.Common;
using Microsoft.VisualBasic;
using System.Collections.Generic;

namespace CashTrack.Models.SummaryModels
{
    public record MonthlySummaryRequest
    {
        public int Month { get; set; }
        public int Year { get; set; }

    }
    public record MonthlySummaryResponse
    {
        //TODO: Add an object for the progress bars
        public MonthlyProgress MonthlyProgress { get; set; }
        public AnnualSavingsProgress AnnualSavingsProgress { get; set; }
        public ExpenseSummaryChartData ExpenseSummaryChart { get; set; }
        public MonthlySummary MonthlySummary { get; set; }
        public OverallSummaryChart OverallSummaryChart { get; set; }
        public Dictionary<string, int> SubCategoryPercentages { get; set; }
        public Dictionary<string, int> MainCategoryPercentages { get; set; }
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
        public int AnnualSavings { get; set; }
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
