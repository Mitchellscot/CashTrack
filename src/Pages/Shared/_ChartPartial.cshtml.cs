using System;
using System.Linq;
using System.Text.Json;

namespace CashTrack.Pages.Shared
{
    public class _ChartPartial
    {

        public string VariableName { get; private set; }
        public _ChartPartial()
        {
            VariableName = string.Join("", Enumerable.Repeat(0, 6).Select(n => (char)new Random().Next(97, 122)));
            ColorArray = GetThemeColors();
        }
        public string DefaultColor = JsonSerializer.Serialize("rgba(24, 188, 156, .8)");
        public string ColorArray { get; private set; }
        public bool UseDefaultColor { get; set; }
        public bool Responsive { get; set; }
        public string ElementId { get; set; }
        public string Labels { get; set; }
        public string Values { get; set; }
        public ChartType ChartType { get; set; }
        private string GetColors()
        {
            return JsonSerializer.Serialize(new[] { "rgba(255, 99, 132, 0.8)", "rgba(255, 159, 64, 0.8)", "rgba(255, 205, 86, 0.8)", "rgba(75, 192, 192, 0.8)", "rgba(54, 162, 235, 0.8)", "rgba(153, 102, 255, 0.8)", "rgba(201, 203, 207, 0.8)" });
        }
        private string GetThemeColors()
        {
            return JsonSerializer.Serialize(new[] { "rgba(44, 62, 88, .8)", "rgba(149, 165, 166, .8)", "rgba(24, 188, 156, .8)", "rgba(52, 152, 219, .8)", "rgba(243, 156, 18, .8)", "rgba(231, 76, 60, .8)", "rgba(123, 138, 139, .8)"});
        }
    }
    public enum ChartType
    {
        Bar,
        BarAndLine,
        Pie,
        Donut
    }

}
