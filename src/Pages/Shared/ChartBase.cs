using CashTrack.Common;
using System;
using System.Linq;
using System.Text.Json;

namespace CashTrack.Pages.Shared
{
    public class ChartBase
    {
        public string VariableName { get; private set; }
        public ChartBase()
        {
            VariableName = string.Join("", Enumerable.Repeat(0, 6).Select(n => (char)new Random().Next(97, 122)));
        }
        public string ElementId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Dataset { get; set; }
        public string Labels { get; set; }
        public string Colors => GetColors();
        public string GetColors()
        {
            return JsonSerializer.Serialize(new[] {
                ChartColors.Pink,
                ChartColors.Orange,
                ChartColors.Yellow,
                ChartColors.Cyan,
                ChartColors.Azure,
                ChartColors.Purple
            });
        }

    }
}
