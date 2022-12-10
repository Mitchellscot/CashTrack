using CashTrack.Common;
using CashTrack.Models.BudgetModels;
using CashTrack.Models.SummaryModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace CashTrack.Pages.Shared
{
    public class MonthlySummaryChart : ChartBase
    {
        public MonthlySummaryChart() : base() { }
        public string Months => JsonSerializer.Serialize(new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" });
        public string BudgetedIncomeDataset { get; set; }
        public string RealizedIncomeDataset { get; set; }
        public string BudgetedSavingsDataset { get; set; }
        public string RealizedSavingsDataset { get; set; }
        public List<RealizedExpenses> RealizedExpenseDatasets { get; set; }
        public List<BudgetedExpenses> BudgetedExpenseDatasets { get; set; }

        public string RealizedIncomeColor = JsonSerializer.Serialize(ThemeColors.SecondaryDark);
        public string BudgetedIncomeColor = JsonSerializer.Serialize(ThemeColors.Success);
        public string BudgetedSavingsColor = JsonSerializer.Serialize(ThemeColors.Info);
        public string RealizedSavingsColor = JsonSerializer.Serialize(ThemeColors.InfoDark);
        public string InTheRedSavingsColor = JsonSerializer.Serialize(DarkChartColors.RedBold);
    }
}
