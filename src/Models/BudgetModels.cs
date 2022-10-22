using Microsoft.AspNetCore.Mvc.Rendering;

namespace CashTrack.Models.BudgetModels
{
    public enum BudgetType
    {
        Savings,
        Static,
        Dynamic,
        Wishlist
    }
    public enum AllocationTimeSpan
    {
        Month,
        Year,
        Week,
        SpecificMonth
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
}
