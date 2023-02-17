using CashTrack.Common;

namespace CashTrack.Pages.Shared
{
    public class MonthlyStatisticsChart : ChartBase
    {
        public MonthlyStatisticsChart() : base() { }
        public string CountDataset { get; set; }
        public string CountColor = ThemeColors.Primary;
        public string AverageDataset { get; set; }
        public string AverageColor = ThemeColors.Info;
        public string MinDataset { get; set; }
        public string MinColor = ThemeColors.Warning;
        public string MaxDataset { get; set; }
        public string MaxColor = ThemeColors.Danger;
        public bool DisplayLabels { get; set; } = false;
    }
}
