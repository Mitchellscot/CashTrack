using Microsoft.AspNetCore.Mvc.Rendering;

namespace CashTrack.Models.BudgetModels
{
    public enum BudgetType
    {
        Savings,
        Need,
        Want
    }
    public enum AllocationTimeSpan
    {
        SpecificMonth,
        Month,
        Year
    }
    public class AddBudgetAllocation
    {
        public int SubCategoryId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Amount { get; set; }
        public BudgetType Type { get; set; }
        public AllocationTimeSpan TimeSpan { get; set; }
    }
    public class AddBudgetAllocationModal : AddBudgetAllocation
    {
        public SelectList SubCategoryList { get; set; }
    }
    public class CategoryAveragesAndTotals
    {
        public decimal SixMonthAverages { get; set; }
        public decimal ThisYearAverages { get; set; }
        public decimal LastYearAverages { get; set; }
        public decimal TwoYearsAgoAverages { get; set; }
        public decimal SixMonthTotals { get; set; }
        public decimal ThisYearTotals { get; set; }
        public decimal LastYearTotals { get; set; }
        public decimal TwoYearsAgoTotals { get; set; }
    }
}
