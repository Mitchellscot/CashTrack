using CashTrack.Common;
using CashTrack.Models.MainCategoryModels;
using System;
using System.Collections.Generic;
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
        }
        public string DefaultColor = JsonSerializer.Serialize(ThemeColors.Success);
        public bool DisplayLabels { get; set; } = true;
        public string ColorArray => GetColors();
        public bool UseDefaultColor { get; set; }
        public bool Responsive { get; set; }
        public string ElementId { get; set; }
        public string Labels { get; set; }
        public string Dataset { get; set; }
        public string Dataset2 { get; set; }
        public string Dataset3 { get; set; }
        public string Dataset4 { get; set; }
        public List<SubCategoryAmountDataset> MultipleDataSets { get; set; }
        public ChartType ChartType { get; set; }
        public string Title { get; set; }
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
        private string GetThemeColors()
        {
            return JsonSerializer.Serialize(new[] {
                ThemeColors.Primary,
                ThemeColors.Secondary,
                ThemeColors.Success,
                ThemeColors.Info,
                ThemeColors.Warning,
                ThemeColors.Danger,
                ThemeColors.SecondaryDark
            });
        }
    }
    public enum ChartType
    {
        Bar,
        DoubleBar,
        BarAndLine,
        LineMultiAxis,
        Pie,
        Donut,
        StackedBar
    }
}
