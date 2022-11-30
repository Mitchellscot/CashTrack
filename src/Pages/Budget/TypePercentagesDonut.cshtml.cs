using CashTrack.Common;
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
        public new string GetColors()
        {
            return JsonSerializer.Serialize(new[] {
                ChartColors.Orange,
                ChartColors.Yellow,
                ChartColors.Azure,
                ThemeColors.Secondary
            });
        }
    }

}
