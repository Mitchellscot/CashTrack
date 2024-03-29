using CashTrack.Common;
using System;
using System.Linq;
using System.Text.Json;

namespace CashTrack.Pages.Budget
{
    public class AnnualBudgetChart
    {
        public string VariableName { get; private set; }
        public AnnualBudgetChart()
        {
            VariableName = string.Join("", Enumerable.Repeat(0, 6).Select(n => (char)new Random().Next(97, 122)));
        }
        public string ElementId { get; set; }
        public string Months => JsonSerializer.Serialize(new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" });
        public string IncomeDataset { get; set; }
        public string NeedDataset { get; set; }
        public string WantDataset { get; set; }
        public string SavingsDataset { get; set; }
        public string UnallocatedDataset { get; set; }
        public string Title { get; set; }
        public string IncomeColor = JsonSerializer.Serialize(ThemeColors.Success);
        public string NeedColor = JsonSerializer.Serialize(ThemeColors.Danger);
        public string WantColor = JsonSerializer.Serialize(ThemeColors.Warning);
        public string SavingsColor = JsonSerializer.Serialize(ThemeColors.Info);
        public string InTheRedSavingsColor = JsonSerializer.Serialize(DarkChartColors.RedBold);
        public string UnallocatedColor = JsonSerializer.Serialize(ThemeColors.Secondary);
    }
}
