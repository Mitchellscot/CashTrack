using CashTrack.Data.Entities;
using System.Collections.Generic;

namespace CashTrack.Common
{
    public class AppSettingsOptions
    {
        public const string AppSettings = "AppSettings";
        public string Secret { get; set; }
        public decimal DefaultTax { get; set; }
        public KeyValuePair<string, string> ConnectionStrings { get; set; }
    }
}
