using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CashTrack.Models.Common
{
    public static class ExpenseQueryOptions
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
            { 7, "Merchant" },
            { 8, "Sub Category" },
            { 9, "Main Category" },
            { 10, "Tag" }
        };
    }
}