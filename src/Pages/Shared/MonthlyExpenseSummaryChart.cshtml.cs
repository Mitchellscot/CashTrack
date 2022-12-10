using CashTrack.Common;
using CashTrack.Models.BudgetModels;
using CashTrack.Models.SummaryModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace CashTrack.Pages.Shared
{
    public class MonthlyExpenseSummaryChart : ChartBase
    {
        public MonthlyExpenseSummaryChart() : base() { }
        public List<ExpenseDataset> RealizedExpenseDatasets { get; set; }
        public List<ExpenseDataset> BudgetedExpenseDatasets { get; set; }

    }
}
