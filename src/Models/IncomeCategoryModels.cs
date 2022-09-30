using CashTrack.Models.Common;
using CashTrack.Models.ExpenseModels;
using CashTrack.Services.Common;
using System;
using System.Collections.Generic;

namespace CashTrack.Models.IncomeCategoryModels;

public class IncomeCategoryRequest : PaginationRequest
{
}
public class IncomeCategoryResponse : PaginationResponse<IncomeCategoryListItem>
{
    public IncomeCategoryResponse(int pageNumber, int pageSize, int totalCount, IncomeCategoryListItem[] listItems) : base(pageNumber, pageSize, totalCount, listItems) { }
}
public class IncomeCategoryListItem : Category
{
    public int Payments { get; set; }
    public decimal Amount { get; set; }
    public DateTime LastPayment { get; set; }
    public bool InUse { get; set; }
}

public class AddEditIncomeCategoryModal : Category
{
    new public int? Id { get; set; }
    public string Notes { get; set; }
    public bool InUse { get; set; } = true;
    public bool IsEdit { get; set; }
    public string Returnurl { get; set; }
}
public class IncomeCategoryDetail : Category
{
    public string Notes { get; set; }
    public bool InUse { get; set; } = true;
    public Totals IncomeTotals { get; set; }
    public List<AnnualStatistics> AnnualIncomeStatistics { get; set; }
    public List<MonthlyStatistics> MonthlyIncomeStatistics { get; set; }
    public List<IncomeQuickViewForCategoryDetail> RecentIncome { get; set; }
    public Dictionary<string, int> SourcePurchaseOccurances { get; set; }
    public Dictionary<string, decimal> SourcePurchaseTotals { get; set; }
}

public class IncomeCategoryDropdownSelection
{
    public int Id { get; set; }
    public string Category { get; set; }
}
public class IncomeQuickViewForCategoryDetail : Transaction
{
    new public string Date { get; set; }
    public string Source { get; set; }
}
