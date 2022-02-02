using CashTrack.Models.Common;
using System;
using System.Collections.Generic;

namespace CashTrack.Models.IncomeModels;

public class IncomeRequest : TransactionRequest
{
}
public class IncomeResponse : TransactionResponse<IncomeListItem>
{
    public IncomeResponse(int pageNumber, int pageSize, int count, IEnumerable<IncomeListItem> listItems, decimal amount) : base(pageNumber, pageSize, count, listItems, amount)
    {
    }
}

public class AddEditIncome : Transaction
{
    new public int? Id { get; set; }
    public string Notes { get; set; }
    public int CategoryId { get; set; }
    public int? SourceId { get; set; }
}

public class IncomeListItem : Transaction
{
    public string Source { get; set; }
    public string Category { get; set; }
}

public class IncomeQuickView : Transaction
{
    public string Category { get; set; }
}
public class IncomeDetail : Transaction
{
    public string Source { get; set; }
    public string Category { get; set; }
    public string Notes { get; set; }
    //add more to this
}