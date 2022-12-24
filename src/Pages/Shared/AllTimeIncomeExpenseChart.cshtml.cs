using CashTrack.Common;
using System.Text.Json;

namespace CashTrack.Pages.Shared
{
    public class AllTimeIncomeExpenseChart : ChartBase
    {
        public AllTimeIncomeExpenseChart() : base() { }
        public string IncomeDataset { get; set; }
        public string ExpenseDataset { get; set; }
        public string IncomeColor = JsonSerializer.Serialize(ThemeColors.SuccessDark);
        public string ExpenseColor = JsonSerializer.Serialize(ThemeColors.DangerDark);
    }
}
