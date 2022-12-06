using CashTrack.Common;
using CashTrack.Pages.Shared;
using System.Collections.Generic;
using System.Linq;
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
        //i definitely overengineered this one...
        public new string GetColors()
        {
            const string Unallocated = "Unallocated";
            const string Savings = "Savings";
            var labelsArray = JsonSerializer.Deserialize<string[]>(this.Labels);
            Stack<string> colorStack = new Stack<string>();
            if (labelsArray.Any(x => x == Unallocated) && labelsArray.Any(x => x == Savings))
            {
                for (int i = 0; i < labelsArray.Length; i++)
                {
                    if (i == labelsArray.Length - 2)
                        colorStack.Push(CategoryPieChart.SavingsColor);
                    else if (i == labelsArray.Length - 1)
                        colorStack.Push(CategoryPieChart.UnallocatedColor);
                    else colorStack.Push(GetColorsForExpenses(i));
                }
            }
            else if (labelsArray.Any(x => x != Unallocated) && labelsArray.Any(x => x == Savings))
            {
                for (int i = 0; i < labelsArray.Length; i++)
                {
                    if (i == labelsArray.Length - 1)
                        colorStack.Push(CategoryPieChart.SavingsColor);
                    else colorStack.Push(GetColorsForExpenses(i));
                }
            }
            else if (labelsArray.Any(x => x == Unallocated) && labelsArray.Any(x => x != Savings))
            {
                for (int i = 0; i < labelsArray.Length - 1; i++)
                {
                    if (i == labelsArray.Length - 1)
                        colorStack.Push(CategoryPieChart.UnallocatedColor);
                    else colorStack.Push(GetColorsForExpenses(i));
                }
            }
            else
            {
                for (int i = 0; i < labelsArray.Length; i++)
                {
                    colorStack.Push(GetColorsForExpenses(i));
                }
            }
            return JsonSerializer.Serialize(colorStack.ToArray().Reverse());
        }
        private string GetColorsForExpenses(int index)
        {
            var colors = new string[]
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
        public const string SavingsColor = ThemeColors.Info;
        public const string UnallocatedColor = ThemeColors.Secondary;
    }
}
