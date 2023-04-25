using CashTrack.Common;
using System.Text.Json;

namespace CashTrack.Pages.Shared
{
    public class OverallSummaryChart : ChartBase
    {
        public OverallSummaryChart() : base() { }
        public string BudgetedIncome { get; set; }
        public string RealizedIncome { get; set; }
        public string BudgetedExpenses { get; set; }
        public string RealizedExpenses { get; set; }
        public string BudgetedSavings { get; set; }
        public string RealizedSavings { get; set; }
        public string BudgetedIncomeColor = JsonSerializer.Serialize(ThemeColors.Success);
        public string RealizedIncomeColor = JsonSerializer.Serialize(ThemeColors.SuccessDark);
        public string BudgetedExpensesColor = JsonSerializer.Serialize(ThemeColors.Danger);
        public string RealizedExpensesColor = JsonSerializer.Serialize(ThemeColors.DangerDark);
        public string BudgetedSavingsColor = JsonSerializer.Serialize(ThemeColors.Info);
        public string RealizedSavingsColor = JsonSerializer.Serialize(ThemeColors.InfoDark);
        public string InTheRedSavingsColor = JsonSerializer.Serialize(DarkChartColors.RedBold);


    }
}
