using CashTrack.Models.SummaryModels;
using System.Collections.Generic;

namespace CashTrack.Pages.Shared
{
    public class TransactionBreakdownModalModel
    {
        public List<TransactionBreakdown> Transactions { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
