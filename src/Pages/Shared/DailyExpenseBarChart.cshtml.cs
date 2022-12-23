using CashTrack.Models.SummaryModels;
using System.Collections.Generic;

namespace CashTrack.Pages.Shared
{
    public class DailyExpenseBarChart : ChartBase
    {
        public DailyExpenseBarChart() : base() { }

        public List<DailyExpenseDataset> ExpenseDatasets { get; set; }
        public bool IsAnnual { get; set; }


    }
}
