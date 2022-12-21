using CashTrack.Models.Common;
using CashTrack.Models.IncomeModels;
using CashTrack.Services.Common;
using System;
using System.Collections.Generic;

namespace CashTrack.Models.IncomeSourceModels;

public class IncomeSourceRequest : PaginationRequest
{
}

public class IncomeSourceResponse : PaginationResponse<IncomeSourceListItem>
{
    public IncomeSourceResponse(int pageNumber, int pageSize, int totalCount, IncomeSourceListItem[] listItems) : base(pageNumber, pageSize, totalCount, listItems) { }
}
public record IncomeSource
{
    public int? Id { get; set; }
    public string Name { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public bool IsOnline { get; set; }
    public string Notes { get; set; }
    public bool SuggestOnLookup { get; set; }
}
public record AddEditIncomeSourceModal : IncomeSource
{
    public string ReturnUrl { get; set; }
    public bool IsEdit { get; set; }
}
public record IncomeSourceListItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Payments { get; set; }
    public decimal Amount { get; set; }
    public DateTime LastPayment { get; set; }
    public string Category { get; set; }
}
public record IncomeSourceDetail : IncomeSource
{
    public Totals IncomeTotals { get; set; }
    public string MostUsedCategory { get; set; }
    public List<AnnualStatistics> AnnualIncomeStatistcis { get; set; }
    public List<MonthlyStatistics> MonthlyIncomeStatistcis { get; set; }
    public Dictionary<string, int> PaymentCategoryOcurances { get; set; }
    public Dictionary<string, decimal> PaymentCategoryTotals { get; set; }
    public List<IncomeQuickView> RecentIncomes { get; set; }
}
public class SourceDropdownSelection
{
    public int Id { get; set; }
    public string Name { get; set; }
}
public record IncomeSourceQuickView
{ 
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public int Count { get; set; }
}