using CashTrack.Models.Common;
using CashTrack.Models.TagModels;
using System;
using System.Collections.Generic;

namespace CashTrack.Models.ExpenseModels;

public class ExpenseRequest : TransactionRequest
{
}
public class ExpenseResponse : TransactionResponse<Expense>
{
    public ExpenseResponse(int pageNumber, int pageSize, int count, IEnumerable<Expense> listItems, decimal amount) : base(pageNumber, pageSize, count, listItems, amount)
    {
    }
}
public class AmountSearchRequest : PaginationRequest
{
    private decimal _query;
    new public decimal Query
    {
        get => _query;
        set => _query = Decimal.Round(value, 2);
    }
}

public class Expense : Transaction
{
    new public int? Id { get; set; }
    public string Notes { get; set; }
    public string Merchant { get; set; }
    public ICollection<TagModel> Tags { get; set; }
    public string SubCategory { get; set; }
    public string MainCategory { get; set; }
    public bool ExcludeFromStatistics { get; set; }
    public bool CreateNewMerchant { get; set; }
}
public class ExpenseQuickView : Transaction
{
    new public string Date { get; set; }
    public string SubCategory { get; set; }
}

