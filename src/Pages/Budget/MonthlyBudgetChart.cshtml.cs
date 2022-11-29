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

        public string IncomeColor = JsonSerializer.Serialize("rgba(24, 188, 156, .8)"); //green
        public string SavingsColor = JsonSerializer.Serialize("rgba(54, 162, 235, .8)");  //blue
        public string UnallocatedColor = JsonSerializer.Serialize("rgba(153, 102, 255, .8)"); //purple
        public string NeedsColor = JsonSerializer.Serialize("rgba(231, 76, 60, 0.8)"); //red
        public string WantsColor = JsonSerializer.Serialize("rgba(255, 205, 86, 0.8)"); //yellow
        public string InTheRedSavingsColor = JsonSerializer.Serialize("rgba(149, 165, 166, .8)"); //grey
    }
}
