using System.Collections.Generic;
using CashTrack.Models.Common;
using CashTrack.Models.ExpenseModels;
using CashTrack.Services.Common;

namespace CashTrack.Models.MerchantModels;


public class MerchantRequest : PaginationRequest
{
}
public class MerchantResponse : PaginationResponse<MerchantListItem>
{
    public MerchantResponse(int pageNumber, int pageSize, int totalCount, MerchantListItem[] listItems) : base(pageNumber, pageSize, totalCount, listItems) { }
}

public record Merchant
{
    public int? Id { get; set; }
    public string Name { get; set; }
    public bool SuggestOnLookup { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public bool IsOnline { get; set; }
    public string Notes { get; set; }
}
public record AddMerchantModal : Merchant
{
    public string Returnurl { get; set; }
}
public record MerchantListItem : Merchant
{
    public int NumberOfExpenses { get; set; }
}
public record MerchantDetail : Merchant
{
    public ExpenseTotals ExpenseTotals { get; set; }
    public string MostUsedCategory { get; set; }
    public List<AnnualExpenseStatistics> AnnualExpenseStatistics { get; set; }
    public Dictionary<string, int> PurchaseCategoryOccurances { get; set; }
    public Dictionary<string, decimal> PurchaseCategoryTotals { get; set; }
    public List<ExpenseQuickView> RecentExpenses { get; set; }
}

