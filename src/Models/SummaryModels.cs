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
        public MonthlySummaryChartData SummaryChartData { get; set; }
        public MonthlySummary MonthlySummary { get; set; }
    }
    public record MonthlySummaryChartData
    {
        public string[] Labels { get; set; }
        public int[] BudgetedSavingsData { get; set; }
        public int[] RealizedSavingsData { get; set; }
        public int[] BudgetedIncomeData { get; set; }
        public int[] RealizedIncomeData { get; set; }
        public List<BudgetedExpenses> BudgetedExpenses { get; set; }
        public List<RealizedExpenses> RealizedExpenses { get; set; }

    }

    public class BudgetedExpenses
    {
        public string SubCategoryName { get; set; }
        public string Color { get; set; }
        public int MainCategoryId { get; set; }
        public int[] DataSet { get; set; }
    }
    public class RealizedExpenses
    {
        public string SubCategoryName { get; set; }
        public string Color { get; set; }
        public int MainCategoryId { get; set; }
        public decimal[] DataSet { get; set; }
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
