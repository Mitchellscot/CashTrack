using CashTrack.Common;
using CashTrack.Pages.Shared;
using System.Linq;
using System.Text.Json;

namespace CashTrack.Pages.Budget
{
    public class MonthlyBudgetChart : ChartBase
    {
        public MonthlyBudgetChart() : base() { }
        public string IncomeDataset { get; set; }
        public string NeedDataset { get; set; }
        public string WantDataset { get; set; }
        public string SavingsDataset { get; set; }
        public string UnallocatedDataset { get; set; }

        public string IncomeColor = JsonSerializer.Serialize(Common.ThemeColors.Success);
        public string SavingsColor = JsonSerializer.Serialize(ChartColors.Azure);
        public string UnallocatedColor = JsonSerializer.Serialize(ThemeColors.Secondary);
        public string NeedsColor = JsonSerializer.Serialize(ChartColors.Orange);
        public string WantsColor = JsonSerializer.Serialize(ChartColors.Yellow);
        public string InTheRedSavingsColor = JsonSerializer.Serialize(Common.ThemeColors.Danger);
    }
}
