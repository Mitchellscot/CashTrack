using System;

namespace CashTrack.Models.ExportModels;

//might add a date range to this request later...
public class ExportTransactionsRequest
{
    public bool IsIncome { get; set; }
    public bool IncludeMainCategory { get; set; }
};
public class ExportTransaction
{
    public string Date { get; set; }
    public string Amount { get; set; }
    public string Category { get; set; }
    public string Notes { get; set; }
};
public class ExportExpense : ExportTransaction
{
    public string Merchant { get; set; }
}
public class ExportIncome : ExportTransaction
{
    public string Source { get; set; }
}