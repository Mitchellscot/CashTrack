using CashTrack.Pages.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace CashTrack.Pages.Budget
{
    public class SubCategoryPieChart : ChartBase
    {
        public SubCategoryPieChart(string labels) : base() 
        { 
            Labels = labels;
        }
        new public string Colors => GetColors();
        //i definitely overthought this one...
        new public string GetColors()
        {
            var labelsArray = this.Labels.Split(',');
            Stack<string> colorStack = new Stack<string>();
            if (labelsArray.Any(x => x.Contains("Unallocated")) && labelsArray.Any(x => x.Contains("Savings")))
            {
                for (int i = 0; i < labelsArray.Length; i++)
                {
                    if (i == labelsArray.Length - 2)
                        colorStack.Push(SubCategoryPieChart.SavingsColor);
                    else if (i == labelsArray.Length -1)
                        colorStack.Push(SubCategoryPieChart.UnallocatedColor);
                    else colorStack.Push(GetColorsForExpenses(i));
                }
            }
            else if (labelsArray.Any(x => !x.Contains("Unallocated")) && labelsArray.Any(x => x.Contains("Savings")))
            {
                for (int i = 0; i < labelsArray.Length; i++)
                {
                    if (i == labelsArray.Length -1)
                        colorStack.Push(SubCategoryPieChart.SavingsColor);
                    else colorStack.Push(GetColorsForExpenses(i));
                }
            }
            else if (labelsArray.Any(x => x.Contains("Unallocated")) && !labelsArray.Any(x => x.Contains("Savings")))
            {
                for (int i = 0; i < labelsArray.Length - 1; i++)
                {
                    if (i == labelsArray.Length -1)
                        colorStack.Push(SubCategoryPieChart.UnallocatedColor);
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
        public const string SavingsColor = "rgba(54, 162, 235, .8)";
        public const string UnallocatedColor = "rgba(153, 102, 255, .8)";
    }
}
