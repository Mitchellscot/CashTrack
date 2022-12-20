using CashTrack.Common;
using System.Text.Json;

namespace CashTrack.Pages.Shared
{
    public class AnnualSavingsChart : ChartBase
    {
        public AnnualSavingsChart() : base() { }
        public string BudgetedSavingsDataset { get; set; }
        public string BudgetedSavingsColor = JsonSerializer.Serialize(ThemeColors.Info);
        public string SavingsColor = JsonSerializer.Serialize(DarkChartColors.Blue);
        public string NegativeSavingsColor = JsonSerializer.Serialize(DarkChartColors.Red);
    }
}
