using CashTrack.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace CashTrack.Pages.Shared
{
    public class AnnualMonthlySummaryChart : ChartBase
    {
        public AnnualMonthlySummaryChart() : base() { }
        public string Months => JsonSerializer.Serialize(new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" });
        public string IncomeDataset { get; set; }
        public string ExpenseDataset { get; set; }
        public string SavingsDataset { get; set; }
        public string BudgetedIncomeDataset { get; set; }
        public string BudgetedExpenseDataset { get; set; }
        public string BudgetedSavingsDataset { get; set; }
        public string IncomeColor = JsonSerializer.Serialize(ThemeColors.SuccessDark);
        public string SavingsColor = JsonSerializer.Serialize(ThemeColors.InfoDark);
        public string ExpenseColor = JsonSerializer.Serialize(ThemeColors.DangerDark);
        public string BudgetedIncomeColor = JsonSerializer.Serialize(ThemeColors.Success);
        public string BudgetedExpensesColor = JsonSerializer.Serialize(ThemeColors.Danger);
        public string BudgetedSavingsColor = JsonSerializer.Serialize(ThemeColors.Info);
        public string InTheRedSavingsColor = JsonSerializer.Serialize(DarkChartColors.RedBold);
    }
}
