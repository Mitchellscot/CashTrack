using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace CashTrack.Models.BudgetModels
{
    public record AnnualBudgetPageRequest
    {
        public int Year { get; set; }
    }
    public record BudgetPageResponse
    {
        public AnnualBudgetChartData AnnualBudgetChartData { get; set; }
        public AnnualSummary AnnualSummary { get; set; }
        public Dictionary<string, int> TypePercentages { get; set; }
        public Dictionary<string, int> SubCategoryPercentages { get; set; }
        public Dictionary<string, int> MainCategoryPercentages { get; set; }
    }
    public record AnnualSummary
    {
        public int IncomeAmount { get; set; }
        public int ExpensesAmount { get; set; }
        public int NeedsAmount { get; set; }
        public int WantsAmount { get; set; }
        public int SavingsAmount { get; set; }
        public int UnallocatedAmount { get; set; }
    }
    public record AnnualBudgetChartData()
    {
        public List<int> IncomeData { get; set; }
        public List<int> NeedsData { get; set; }
        public List<int> WantsData { get; set; }
        public List<int> SavingsData { get; set; }
        public List<int> Unallocated { get; set; }

    }
    public enum BudgetType
    {
        Need,
        Want,
        Savings,
        Income
    }
    public enum AllocationTimeSpan
    {
        Month,
        Week,
        Year
    }
    public class AddBudgetAllocation
    {
        public int SubCategoryId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Amount { get; set; }
        public BudgetType Type { get; set; }
        public bool IsIncome { get; set; }
        public AllocationTimeSpan TimeSpan { get; set; }
    }
    public class AddBudgetAllocationModal : AddBudgetAllocation
    {
        public SelectList SubCategoryList { get; set; }
        public SelectList YearList { get; set; } = new SelectList(new[] { DateTime.Now.Year, DateTime.Now.Year + 1 }, DateTime.Now.Year);
        public SelectList MonthList { get; set; } = new SelectList(new Dictionary<string, int> {
            { "Every", 0},
            { "January", 1 },
            { "February", 2 },
            { "March", 3 },
            { "April", 4 },
            { "May", 5 },
            { "June", 6 },
            { "July", 7 },
            { "August", 8 },
            { "September", 9 },
            { "October", 10 },
            { "November", 11 },
            { "December", 12 }
        }, "Value", "Key", DateTime.Now.Month);
        public string[] TimeSpans = new[] { "Specific Month", "Year", "Month", "Week" };
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
