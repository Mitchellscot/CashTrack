using CashTrack.Models.BudgetModels;
using CashTrack.Models.Common;
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
        public ExpenseSummaryChartData ExpenseSummaryChart { get; set; }
        public MonthlySummary MonthlySummary { get; set; }
        public OverallSummaryChart OverallSummaryChart { get; set; }
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
    }
}
