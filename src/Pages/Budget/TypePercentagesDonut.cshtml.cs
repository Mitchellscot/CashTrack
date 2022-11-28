using CashTrack.Pages.Shared;
using System;
using System.Linq;
using System.Text.Json;

namespace CashTrack.Pages.Budget
{
    public class TypePercentagesDonut : ChartBase
    {
        public TypePercentagesDonut() : base() { }
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
