using CashTrack.Common;
using CashTrack.Models.BudgetModels;
using CashTrack.Pages.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace CashTrack.Pages.Budget
{
    public class MonthlyBudgetChart : ChartBase
    {
        public MonthlyBudgetChart() : base() { }
        public string IncomeDataset { get; set; }
        public List<ExpenseDataset> ExpenseDataset { get; set; }
        public string SavingsDataset { get; set; }
        public string UnallocatedDataset { get; set; }

        public string IncomeColor = JsonSerializer.Serialize(ThemeColors.Success);
        public string SavingsColor = JsonSerializer.Serialize(ThemeColors.Info);
        public string UnallocatedColor = JsonSerializer.Serialize(ThemeColors.Secondary);
        public string NeedsColor = JsonSerializer.Serialize(ThemeColors.Danger);
        public string WantsColor = JsonSerializer.Serialize(ThemeColors.Warning);
        public string InTheRedSavingsColor = JsonSerializer.Serialize(ThemeColors.SuccessAlt);
    }
}
