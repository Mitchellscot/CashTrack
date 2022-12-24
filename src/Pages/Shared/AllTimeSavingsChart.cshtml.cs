
using CashTrack.Common;
using System.Text.Json;

namespace CashTrack.Pages.Shared
{
    public class AllTimeSavingsChart : ChartBase
    {
        public AllTimeSavingsChart() : base () { }
        public string SuggestedSavingsDataset { get; set; }
        public string SavingsColor = JsonSerializer.Serialize(DarkChartColors.Blue);
        public string NegativeSavingsColor = JsonSerializer.Serialize(DarkChartColors.Red);
    }
}
