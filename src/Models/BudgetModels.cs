﻿using Microsoft.AspNetCore.Mvc.Rendering;

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
    public class CategoryMonthlyAverages
    {
        public decimal SixMonths { get; set; }
        public decimal ThisYear { get; set; }
        public decimal LastYear { get; set; }
        public decimal TwoYearsAgo { get; set; }
    }
}
