using CashTrack.Common;
using System.Text.Json;

namespace CashTrack.Pages.Shared
{
    public class AnnualSavingsChart : ChartBase
    {
        public AnnualSavingsChart() : base() { }
        public string SavingsColor = JsonSerializer.Serialize(ThemeColors.Info);
        public string NegativeSavingsColor = JsonSerializer.Serialize(ThemeColors.InfoAlt);
    }
}
