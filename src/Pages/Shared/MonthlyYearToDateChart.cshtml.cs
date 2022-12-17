using CashTrack.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CashTrack.Pages.Shared
{
    public class MonthlyYearToDateChart : ChartBase
    {
        public MonthlyYearToDateChart() : base() { }

        public string IncomeDataset { get; set; }
        public string ExpenseDataset { get; set; }
        public string SavingsDataset { get; set; }
        public string IncomeColor = ThemeColors.Success;
        public string ExpenseColor = ThemeColors.Danger;
        public string SavingsColor = ThemeColors.Info;
        public string SavingsNegativeColor = ThemeColors.DangerDark;
    }
}
