using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CashTrack.Models.Common
{
    public static class IncomeQueryOptions
    {
        public static readonly Dictionary<int, string> GetAll = new Dictionary<int, string>()
        {
            { 0, "Date" },
            { 1, "Date Range" },
            { 2, "Month" },
            { 3, "Quarter" },
            { 4, "Year" },
            { 5, "Amount" },
            { 6, "Notes" },
            { 7, "Source" },
            { 8, "Income Category" },
        };
    }
}