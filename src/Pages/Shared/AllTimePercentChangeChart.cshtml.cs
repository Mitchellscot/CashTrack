using CashTrack.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CashTrack.Pages.Shared
{
    public class AllTimePercentChangeChart : ChartBase
    {
        public AllTimePercentChangeChart() : base() { }
        public string IncomeDataset { get; set; }
        public string ExpenseDataset { get; set; }
        public string SavingsDataset { get; set; }
        public string IncomeColor = ThemeColors.SuccessDark;
        public string ExpenseColor = ThemeColors.DangerDark;
        public string SavingsColor = ThemeColors.InfoDark;
        public string SavingsNegativeColor = ThemeColors.DangerDark;
    }
}
