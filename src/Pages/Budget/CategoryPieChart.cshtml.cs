using CashTrack.Pages.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace CashTrack.Pages.Budget
{
    public class CategoryPieChart : ChartBase
    {
        public CategoryPieChart(string labels) : base()
        {
            Labels = labels;
        }
        public string Colors => GetColors();
        //i definitely overengineered this one...
        public string GetColors()
        {
            var labelsArray = JsonSerializer.Deserialize<string[]>(this.Labels);
            Stack<string> colorStack = new Stack<string>();
            if (labelsArray.Any(x => x == "Unallocated") && labelsArray.Any(x => x == "Savings"))
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
            else if (labelsArray.Any(x => x != "Unallocated") && labelsArray.Any(x => x == "Savings"))
            {
                for (int i = 0; i < labelsArray.Length; i++)
                {
                    if (i == labelsArray.Length - 1)
                        colorStack.Push(CategoryPieChart.SavingsColor);
                    else colorStack.Push(GetColorsForExpenses(i));
                }
            }
            else if (labelsArray.Any(x => x == "Unallocated") && labelsArray.Any(x => x != "Savings"))
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
                "rgba(231, 76, 60, 0.8)", //red
                "rgba(255, 159, 64, 0.8)", //orange
                "rgba(255, 205, 86, 0.8)" //yellow
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
        public const string SavingsColor = "rgba(54, 162, 235, .8)"; //blue
        public const string UnallocatedColor = "rgba(153, 102, 255, .8)"; //purple
    }
}
