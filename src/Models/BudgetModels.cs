using CashTrack.Models.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace CashTrack.Models.BudgetModels
{
    public class BudgetListRequest : PaginationRequest
    {
        public bool Reversed { get; set; }
        public BudgetOrderBy Order { get; set; }
    }
    public class BudgetListResponse : PaginationResponse<BudgetListItem>
    {
        public BudgetListResponse(int pageNumber, int pageSize, int totalCount, List<BudgetListItem> listItems) : base(pageNumber, pageSize, totalCount, listItems) { }
    }
    public record BudgetListItem
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Amount { get; set; }
        public string SubCategory { get; set; }
        public int SubCategoryId { get; set; }
        public string MainCategory { get; set; }
        public string Type { get; set; }

    }
    public record MonthlyBudgetPageRequest
    {
        public int Year { get; set; }
        public int Month { get; set; }
    }
    public record AnnualBudgetPageRequest
    {
        public int Year { get; set; }
    }
    public record AnnualBudgetPageResponse
    {
        public AnnualBudgetChartData AnnualBudgetChartData { get; set; }
        public BudgetSummary AnnualSummary { get; set; }
        public Dictionary<string, int> TypePercentages { get; set; }
        public Dictionary<string, int> SubCategoryPercentages { get; set; }
        public Dictionary<string, int> MainCategoryPercentages { get; set; }
        public List<BudgetBreakdown> BudgetBreakdown { get; set; }
    }
    public record MonthlyBudgetPageResponse
    {
        public MonthlyBudgetChartData MonthlyBudgetChartData { get; set; }
        public BudgetSummary MonthlySummary { get; set; }
        public Dictionary<string, int> TypePercentages { get; set; }
        public Dictionary<string, int> SubCategoryPercentages { get; set; }
        public Dictionary<string, int> MainCategoryPercentages { get; set; }
        public List<BudgetBreakdown> BudgetBreakdown { get; set; }

    }
    public record BudgetBreakdown
    {
        public int MainCategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public string Category { get; set; }
        public int Amount { get; set; }
        public decimal Percentage { get; set; }
        public bool IsMainCategory => SubCategoryId == 0 && MainCategoryId != 0;
        public bool IsIncome => Category == "Income";
    }
    public record BudgetSummary
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
    public record MonthlyBudgetChartData()
    {
        public string[] Labels { get; set; }
        //public int[] IncomeData { get; set; }
        public int[] SavingsData { get; set; }
        public int[] Unallocated { get; set; }
        public List<ExpenseDataset> ExpenseData { get; set; }

    }
    public class ExpenseDataset
    {
        public string SubCategoryName { get; set; }
        public int[] DataSet { get; set; }
        public string Color { get; set; }
        public int MainCategoryId { get; set; }
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
    public enum BudgetOrderBy
    {
        Year,
        Month,
        Amount,
        SubCategory,
        MainCategory,
        Type
    }
    public class AddEditBudgetAllocation
    {
        public int? Id { get; set; }
        public int SubCategoryId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Amount { get; set; }
        public BudgetType Type { get; set; }
        public bool IsIncome { get; set; }
        public AllocationTimeSpan TimeSpan { get; set; }
    }
    public class AddEditBudgetAllocationModal : AddEditBudgetAllocation
    {
        public SelectList SubCategoryList { get; set; }
        public SelectList YearList { get; set; } = new SelectList(new[] { DateTime.Now.Year, DateTime.Now.Year + 1 }, DateTime.Now.Year);
        public SelectList AddMonthList { get; set; } = new SelectList(new Dictionary<string, int> {
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
        public SelectList EditMonthList { get; set; } = new SelectList(new Dictionary<string, int> {
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
        public string ReturnUrl { get; set; }
        public int PageNumber { get; set; }
        public BudgetOrderBy Query { get; set; }
        public bool Q2 { get; set; }
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
