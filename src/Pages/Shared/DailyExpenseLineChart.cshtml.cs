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
        public string BudgetedSpendingColor = DarkChartColors.YellowLight;
        public string DiscretionarySpendingColor = DarkChartColors.OrangeLight;
        public string BudgetedIncomeColor = DarkChartColors.RedLight;
        public string OverIncomeMaxColor = DarkChartColors.PurpleLight;
    }
}