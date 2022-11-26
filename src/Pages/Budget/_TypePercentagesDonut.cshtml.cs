using CashTrack.Pages.Shared;
using System;
using System.Linq;
using System.Text.Json;

namespace CashTrack.Pages.Budget
{
    public class _TypePercentagesDonut
    {
        public string VariableName { get; private set; }
        public _TypePercentagesDonut()
        {
            VariableName = string.Join("", Enumerable.Repeat(0, 6).Select(n => (char)new Random().Next(97, 122)));
        }
        public string ElementId { get; set; } = "typePercentagesDonut";
        public string Title { get; set; }
        public string Dataset { get; set; }
        public string Labels { get; set; }
        public string ColorArray => GetColors();
        public string GetColors()
        {
            return JsonSerializer.Serialize(new[] {
                "rgba(231, 76, 60, .8)",
                "rgba(255, 205, 86, .8)",
                "rgba(54, 162, 235, .8)",
                "rgba(153, 102, 255, .8)"
            });
        }
    }

}
