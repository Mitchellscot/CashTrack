using CashTrack.Models.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public int? SourceId { get; set; }
    public int CategoryId { get; set; }
    public string Category { get; set; }
    public string Notes { get; set; }
    public bool CreateNewSource { get; set; }
    public bool IsRefund { get; set; }
    public string RefundNotes { get; set; }
}
public class AddEditIncomeModal : Income
{
    public bool IsEdit { get; set; }
    public string ReturnUrl { get; set; }
    public SelectList CategoryList { get; set; }
    public bool ShowAddCategoryButton { get; set; }
}

public class IncomeQuickView : Transaction
{
    new public string Date { get; set; }
    public string Category { get; set; }
}