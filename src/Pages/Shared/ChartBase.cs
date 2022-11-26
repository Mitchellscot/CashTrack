using System;
using System.Linq;

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
    }
}
