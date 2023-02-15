using CashTrack.Common;

namespace CashTrack.Pages.Shared
{
    public class Icons
    {
        public Icon Icon { get; set; }
        public int Height { get; set; } = 24;
        public int Width { get; set; } = 24;
        public string Color { get; set; } = IconColors.Primary;
        public string Classes { get; set; } = "";
    }
    public enum Icon
    {
        InfoCircle,
        HandThumbsUp,
        ExclamationCircle,
        QuestionCircle,
        Printer,
        CheckCircle,
        XCircle,
        ArrowLeftRight,
        Reboot,
        Search,
        ArrowUp,
        ArrowDown,
        ZoomIn,
        FileEarmarkCheck,
        Pencil,
        Trash,
        BoxArrowRight,
        Speedometer,
        Receipt,
        Cardlist,
        Shop,
        Diagram,
        CashCoin,
        Bank,
        GraphUp,
        Gear,
        BarChartFill,
        BarChart,
        Calendar3
    }
}
