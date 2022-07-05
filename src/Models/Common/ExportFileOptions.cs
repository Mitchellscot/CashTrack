using System.Collections.Generic;

namespace CashTrack.Models.Common
{
    public static class ExportFileOptions
    {
        public static readonly Dictionary<int, string> GetAll = new Dictionary<int, string>()
        {
            { 0, "All Data" },
            { 1, "Expenses" },
            { 2, "Import Rules" },
            { 3, "Income" },
            { 4, "Income Categories" },
            { 5, "Income Sources" },
            { 6, "Main Categories" },
            { 7, "Merchants" },
            { 8, "Sub Categories" }
        };
    }
}