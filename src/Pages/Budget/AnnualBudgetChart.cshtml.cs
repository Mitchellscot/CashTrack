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
        public string IncomeColor = JsonSerializer.Serialize("rgba(24, 188, 156, .8)");
        public string NeedColor = JsonSerializer.Serialize("rgba(231, 76, 60, .8)");
        public string WantColor = JsonSerializer.Serialize("rgba(255, 205, 86, .8)");
        public string SavingsColor = JsonSerializer.Serialize("rgba(54, 162, 235, .8)");
        public string InTheRedSavingsColor = JsonSerializer.Serialize("rgba(149, 165, 166, .8)"); //light grey
        public string UnallocatedColor = JsonSerializer.Serialize("rgba(153, 102, 255, .8)");
    }
}
