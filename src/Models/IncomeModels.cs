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
    public string Notes { get; set; }
    public bool CreateNewSource { get; set; }
    //TODO: Add IsRefundOrReimbursement Boolean to manage refunds better and not count as income in reports.
    //might need to find a way to link to the expense that is refunded and deduct that amount?
    //Either way, I need to have a better way of managing refunds
    //and to think through what kind of data I want to produce. Counting refunds and reimbursements as income
    //is not wise and doesn't produce accurate end of year reports.
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