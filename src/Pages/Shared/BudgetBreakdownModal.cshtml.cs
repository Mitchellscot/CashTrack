using CashTrack.Models.BudgetModels;
using System.Collections.Generic;

namespace CashTrack.Pages.Shared
{
    public class BudgetBreakdownModal
    {
        public List<BudgetBreakdown> Budgets { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
