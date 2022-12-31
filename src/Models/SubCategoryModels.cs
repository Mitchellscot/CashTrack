using CashTrack.Models.Common;
using CashTrack.Models.ExpenseModels;
using CashTrack.Services.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

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
    public string ReturnUrl { get; set; }
    public SelectList MainCategoryList { get; set; }
}
public class SubCategoryQuickView : Category
{
    public decimal Amount { get; set; }
    public int Count { get; set; }
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
    public int MainCategoryId { get; set; }
    public string Notes { get; set; }
    public bool InUse { get; set; }
    public Totals ExpenseTotals { get; set; }
    public List<AnnualStatistics> AnnualExpenseStatistics { get; set; }
    public List<MonthlyStatistics> MonthlyExpenseStatistics { get; set; }
    public List<ExpenseQuickViewForSubCategoryDetail> RecentExpenses { get; set; }
    public Dictionary<string, int> MerchantPurchaseOccurances { get; set; }
    public Dictionary<string, decimal> MerchantPurchaseTotals { get; set; }
}
public class SubCategoryDropdownSelection
{
    public int Id { get; set; }
    public string Category { get; set; }
}