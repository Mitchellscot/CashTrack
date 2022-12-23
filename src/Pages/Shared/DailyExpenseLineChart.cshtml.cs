using CashTrack.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace CashTrack.Pages.Shared
{
    public class DailyExpenseLineChart : ChartBase
    {
        public DailyExpenseLineChart() : base() { }
        public string ExpenseColor = JsonSerializer.Serialize(ThemeColors.Danger);
        public int Max { get; set; }
        public int ExpenseBudgetMax { get; set; }
        public int DiscretionarySpendingMax { get; set; }
        public int IncomeMax { get; set; }
        public string BudgetedSpendingColor = DarkChartColors.YellowSoft;
        public string DiscretionarySpendingColor = DarkChartColors.OrangeSoft;
        public string BudgetedIncomeColor = DarkChartColors.RedSoft;
        public string OverIncomeMaxColor = DarkChartColors.PurpleSoft;
    }
}