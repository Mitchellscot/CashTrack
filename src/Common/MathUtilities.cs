using System;

namespace CashTrack.Common
{
    public static class MathUtilities
    {
        public static int ToPercentage(this int i, int total) => total != 0 ? (int)decimal.Round(Convert.ToDecimal(i) / Convert.ToDecimal(total) * 100, 0) : 0;
        public static int ToPercentage(this decimal i, decimal total) =>
            total != 0 ? (int)decimal.Round(i / total * 100, 0) : 0;
        public static decimal ToDecimalPercentage(this decimal i, decimal total) =>
            total != 0 ? decimal.Round(i / total * 100, 1) : 0;
        public static decimal ToDecimalPercentage(this int i, int total) =>
    total != 0 ? decimal.Round(Convert.ToDecimal(i) / Convert.ToDecimal(total) * 100, 1) : 0;
    }
}
