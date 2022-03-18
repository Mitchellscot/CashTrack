using CashTrack.Models.Common;
using System;
using System.Collections.Generic;

namespace CashTrack.Models.IncomeModels;

public class IncomeRequest : TransactionRequest
{
}
public class IncomeResponse : TransactionResponse<Income>
{
    public IncomeResponse(int pageNumber, int pageSize, int count, IEnumerable<Income> listItems, decimal amount) : base(pageNumber, pageSize, count, listItems, amount)
    {
    }
}

public class Income : Transaction
{
    new public int? Id { get; set; }
    public string Source { get; set; }
    public string Category { get; set; }
    public string Notes { get; set; }
    public bool CreateNewSource { get; set; }
    public bool IsRefund { get; set; }
}

public class IncomeQuickView : Transaction
{
    public string Category { get; set; }
}