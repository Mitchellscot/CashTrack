using CashTrack.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace CashTrack.Pages.Shared
{
    public class AnnualIncomeExpenseChart : ChartBase
    {
        public AnnualIncomeExpenseChart() : base() { }
        public string IncomeDataset { get; set; }
        public string ExpenseDataset { get; set; }
        public int MonthBudgetIncomeDataBegins { get; set; }
        public int MonthBudgetExpenseDataBegins { get; set; }
        public string BudgetedIncomeColor = JsonSerializer.Serialize(ThemeColors.Success);
        public string IncomeColor = JsonSerializer.Serialize(ThemeColors.SuccessDark);
        public string BudgetedExpenseColor = JsonSerializer.Serialize(ThemeColors.Danger);
        public string ExpenseColor = JsonSerializer.Serialize(ThemeColors.DangerDark);
    }
}
