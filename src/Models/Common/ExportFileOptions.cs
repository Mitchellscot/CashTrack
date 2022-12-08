using System.Collections.Generic;

namespace CashTrack.Models.Common
{
    public static class ExportFileOptions
    {
        public static readonly Dictionary<int, string> GetAll = new Dictionary<int, string>()
        {
            { 0, "All Data" },
            { 1, "Budgets" },
            { 2, "Expenses" },
            { 3, "Import Rules" },
            { 4, "Income" },
            { 5, "Income Categories" },
            { 6, "Income Sources" },
            { 7, "Main Categories" },
            { 8, "Merchants" },
            { 9, "Sub Categories" }
        };
    }
}