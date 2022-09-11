using CashTrack.Models.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace CashTrack.Models.SubCategoryModels;

public class SubCategoryRequest : PaginationRequest
{
    public bool Reversed { get; set; }
    public SubCategoryOrderBy Order { get; set; }
}

public class SubCategoryResponse : PaginationResponse<SubCategoryListItem>
{
    public SubCategoryResponse(int pageNumber, int pageSize, int totalCount, SubCategoryListItem[] listItems) : base(pageNumber, pageSize, totalCount, listItems) { }
}

public class SubCategory : Category
{
    new public int? Id { get; set; }
    public int MainCategoryId { get; set; }
    public string Notes { get; set; }
    public bool InUse { get; set; }
}
public class AddEditSubCategoryModal : SubCategory
{
    public bool IsEdit { get; set; }
    public string Returnurl { get; set; }
    public SelectList MainCategoryList { get; set; }
}

public class SubCategoryListItem : Category
{
    public string MainCategoryName { get; set; }
    public int Purchases { get; set; }
    public decimal Amount { get; set; }
    public DateTime LastPurchase { get; set; }
    public bool InUse { get; set; }
}
public class SubCategoryDetail : Category
{
    public string MainCategoryName { get; set; }
    public string Notes { get; set; }
    public bool InUse { get; set; }
    //Some ideas....
    //expenses and stats by year (like merchants)
    //pie chart showing merchant breakdown showing amount spent at every merchant
    //another one showing # of purchases in given category by merchant
    //Chart showing average monthly cost ? (for budgeting)
    //might have more ideas when I get the app up and running
    //recent expenses in a give category with link to view all expenses by given category

    //        Id = merchant.Id,
    //        Name = merchant.Name,
    //        SuggestOnLookup = merchant.SuggestOnLookup,
    //        City = merchant.City,
    //        State = merchant.State,
    //        Notes = merchant.Notes,
    //        IsOnline = merchant.IsOnline,
    //        ExpenseTotals = new Totals(),
    //        MostUsedCategory = "No expenses yet.",
    //        AnnualExpenseStatistics = new List<AnnualStatistics>(),
    //        MonthlyExpenseStatistics = new List<MonthlyStatistics>(),
    //        PurchaseCategoryOccurances = new Dictionary<string, int>(),
    //        PurchaseCategoryTotals = new Dictionary<string, decimal>(),
    //        RecentExpenses = new List<ExpenseQuickView>(),

}
public class SubCategoryDropdownSelection
{
    public int Id { get; set; }
    public string Category { get; set; }
}