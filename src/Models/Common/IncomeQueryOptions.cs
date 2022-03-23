using System.Collections.Generic;

namespace CashTrack.Models.Common
{
    public static class IncomeQueryOptions
    {
        public static readonly Dictionary<int, string> GetAll = new Dictionary<int, string>()
        {
            { 0, "Date" },
            { 1, "Month" },
            { 2, "Quarter" },
            { 3, "Year" },
            { 4, "Amount" },
            { 5, "Notes" },
            { 6, "Source" },
            { 7, "Category" },
        };
    }
}