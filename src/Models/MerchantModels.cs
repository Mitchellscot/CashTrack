﻿using System;
using System.Collections.Generic;
using CashTrack.Models.Common;
using CashTrack.Models.ExpenseModels;
using CashTrack.Services.Common;

namespace CashTrack.Models.MerchantModels;


public class MerchantRequest : PaginationRequest
{
    public bool Reversed { get; set; }
    public MerchantOrder Order { get; set; }
}
public class MerchantResponse : PaginationResponse<MerchantListItem>
{
    public MerchantResponse(int pageNumber, int pageSize, int totalCount, List<MerchantListItem> listItems) : base(pageNumber, pageSize, totalCount, listItems) { }
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
public record MerchantListItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public int NumberOfExpenses { get; set; }
    public decimal TotalSpent { get; set; }
    public DateTime LastPurchase { get; set; }
    public string Category { get; set; }
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

