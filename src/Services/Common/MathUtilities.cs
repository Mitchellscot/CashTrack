using System;

namespace CashTrack.Services.Common
{
    public static class MathUtilities
    {
        public static int ToPercentage(this int i, int total) => (int)decimal.Round((Convert.ToDecimal(i) / Convert.ToDecimal(total)) * 100, 0);
        public static int ToPercentage(this decimal i, decimal total) => 
            (int)decimal.Round((i / total) * 100, 0);
    }
}
