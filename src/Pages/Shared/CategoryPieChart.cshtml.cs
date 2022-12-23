using CashTrack.Common;
using CashTrack.Pages.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace CashTrack.Pages.Budget
{
    public class CategoryPieChart : ChartBase
    {
        public CategoryPieChart(string labels) : base()
        {
            Labels = labels;
        }
        public new string Colors => GetColors();
        public bool IsSummaryChart { get; set; } = false;
        //i definitely overengineered this one...
        public new string GetColors()
        {
            const string Unallocated = "Unallocated";
            const string Savings = "Savings";
            const string NoMerchantAssigned = "No Merchant Assigned";
            var labelsArray = JsonSerializer.Deserialize<string[]>(this.Labels);
            Stack<string> colorStack = new Stack<string>();
            if (labelsArray.Any(x => x == NoMerchantAssigned))
            {
                for (int i = 0; i < labelsArray.Length; i++)
                {
                    if (i == labelsArray.Length - 1)
                        colorStack.Push(ThemeColors.Primary);
                    else colorStack.Push(GetColorsForExpenses(i, this.IsSummaryChart));
                }
            }
            else if (labelsArray.Any(x => x == Unallocated) && labelsArray.Any(x => x == Savings))
            {
                for (int i = 0; i < labelsArray.Length; i++)
                {
                    if (i == labelsArray.Length - 2)
                        colorStack.Push(CategoryPieChart.SavingsColor);
                    else if (i == labelsArray.Length - 1)
                        colorStack.Push(CategoryPieChart.UnallocatedColor);
                    else colorStack.Push(GetColorsForExpenses(i, this.IsSummaryChart));
                }
            }
            else if (labelsArray.Any(x => x != Unallocated) && labelsArray.Any(x => x == Savings))
            {
                for (int i = 0; i < labelsArray.Length; i++)
                {
                    if (i == labelsArray.Length - 1)
                        colorStack.Push(this.IsSummaryChart ? ThemeColors.InfoDark : CategoryPieChart.SavingsColor);
                    else colorStack.Push(GetColorsForExpenses(i, this.IsSummaryChart));
                }
            }
            else if (labelsArray.Any(x => x == Unallocated) && labelsArray.Any(x => x != Savings))
            {
                for (int i = 0; i < labelsArray.Length - 1; i++)
                {
                    if (i == labelsArray.Length - 1)
                        colorStack.Push(CategoryPieChart.UnallocatedColor);
                    else colorStack.Push(GetColorsForExpenses(i, this.IsSummaryChart));
                }
            }
            else
            {
                for (int i = 0; i < labelsArray.Length; i++)
                {
                    colorStack.Push(GetColorsForExpenses(i, this.IsSummaryChart));
                }
            }
            return JsonSerializer.Serialize(colorStack.ToArray().Reverse());
        }
        private string GetColorsForExpenses(int index, bool isSummary)
        {
            var colors = isSummary ? new[]
            {
                DarkChartColors.Red,
                DarkChartColors.Orange,
                DarkChartColors.Yellow,
                DarkChartColors.Green,
                DarkChartColors.Blue,
                DarkChartColors.Purple
            } : new[]
            {
                LightChartColors.Pink,
                LightChartColors.Orange,
                LightChartColors.Yellow,
                LightChartColors.Cyan,
                LightChartColors.Azure,
                LightChartColors.Purple
            };
            if (index > colors.Length - 1)
            {
                var localIndex = index;
                while (localIndex > colors.Length - 1)
                    localIndex = (localIndex - colors.Length);
                return colors[localIndex];
            }
            else return colors[index];
        }
        public const string SummarySavingsColor = ThemeColors.InfoDark;
        public const string SavingsColor = ThemeColors.Info;
        public const string UnallocatedColor = ThemeColors.Secondary;
    }
}
