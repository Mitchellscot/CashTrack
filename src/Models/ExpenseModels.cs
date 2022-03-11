using CashTrack.Models.Common;
using CashTrack.Models.TagModels;
using System;
using System.Collections.Generic;

namespace CashTrack.Models.ExpenseModels;

public class ExpenseRequest : TransactionRequest
{

}
public class ExpenseResponse : TransactionResponse<ExpenseListItem>
{
    public ExpenseResponse(int pageNumber, int pageSize, int count, IEnumerable<ExpenseListItem> listItems, decimal amount) : base(pageNumber, pageSize, count, listItems, amount)
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

public class AddEditExpense : Transaction
{
    new public int? Id { get; set; }
    public string Notes { get; set; }
    public int? MerchantId { get; set; }
    //figure this out after you get Tags CRUD set up
    //public ICollection<Tag> Tags { get; set; }
    public int SubCategoryId { get; set; }
    public bool ExcludeFromStatistics { get; set; }
}
public class ExpenseListItem : Transaction
{
    //I will probably remove Notes and add that to a detail view to be viewed in a modal
    public string Notes { get; set; }
    public string Merchant { get; set; }
    public ICollection<TagModel> Tags { get; set; }
    public string SubCategory { get; set; }
    public string MainCategory { get; set; }
    public bool ExcludeFromStatistics { get; set; }
}
public class ExpenseQuickView : Transaction
{
    new public string Date { get; set; }
    public string SubCategory { get; set; }
}

