using CashTrack.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace CashTrack.Pages.Shared
{
    public class DonutChart : ChartBase
    {
        public DonutChart(string labels) : base()
        {
             Labels = labels;
        }
        public new string Colors => GetColors();
        public bool TwelveColors { get; set; } = false;
        public bool DisplayLabels { get; set; }
        public new string GetColors()
        {
            const string Unallocated = "Unallocated";
            const string Savings = "Savings";
            const string NoMerchantAssigned = "No Merchant Assigned";
            var labelsArray = JsonSerializer.Deserialize<string[]>(this.Labels);
            Stack<string> colorStack = new();
            if (labelsArray.Any(x => x == NoMerchantAssigned))
            {
                for (int i = 0; i <= labelsArray.Length; i++)
                {
                    if (i == labelsArray.Length - 1)
                        colorStack.Push(ThemeColors.Primary);
                    else colorStack.Push(GetColorsForExpenses(i, this.TwelveColors));
                }
            }
            else if (labelsArray.Any(x => x == Unallocated) && labelsArray.Any(x => x == Savings))
            {
                for (int i = 0; i <= labelsArray.Length; i++)
                {
                    if (i == labelsArray.Length - 2)
                        colorStack.Push(CategoryPieChart.SavingsColor);
                    else if (i == labelsArray.Length - 1)
                        colorStack.Push(CategoryPieChart.UnallocatedColor);
                    else colorStack.Push(GetColorsForExpenses(i, this.TwelveColors));
                }
            }
            else if (labelsArray.Any(x => x != Unallocated) && labelsArray.Any(x => x == Savings))
            {
                for (int i = 0; i <= labelsArray.Length; i++)
                {
                    if (i == labelsArray.Length - 1)
                        colorStack.Push(this.TwelveColors ? ThemeColors.InfoDark : CategoryPieChart.SavingsColor);
                    else colorStack.Push(GetColorsForExpenses(i, this.TwelveColors));
                }
            }
            else if (labelsArray.Any(x => x == Unallocated) && labelsArray.Any(x => x != Savings))
            {
                for (int i = 0; i <= labelsArray.Length - 1; i++)
                {
                    if (i == labelsArray.Length - 1)
                        colorStack.Push(CategoryPieChart.UnallocatedColor);
                    else colorStack.Push(GetColorsForExpenses(i, this.TwelveColors));
                }
            }
            else
            {
                for (int i = 0; i <= labelsArray.Length; i++)
                {
                    colorStack.Push(GetColorsForExpenses(i, this.TwelveColors));
                }
            }
            return JsonSerializer.Serialize(colorStack.ToArray().Reverse());
        }
        private string GetColorsForExpenses(int index, bool twelveColors)
        {
            var colors = twelveColors ? new[]
            {
                Qualitative12Set.Red,
                Qualitative12Set.Orange,
                Qualitative12Set.LightOrange,
                Qualitative12Set.Yellow,
                Qualitative12Set.LightGreen,
                Qualitative12Set.Green,
                Qualitative12Set.Blue,
                Qualitative12Set.LightBlue,
                Qualitative12Set.LightPurple,
                Qualitative12Set.Purple,
                Qualitative12Set.Pink,
                Qualitative12Set.Brown

            } : new[]
            {
                Qualitative6Set.Red,
                Qualitative6Set.Orange,
                Qualitative6Set.Yellow,
                Qualitative6Set.Green,
                Qualitative6Set.Blue,
                Qualitative6Set.Purple
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
