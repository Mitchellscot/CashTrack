using CashTrack.Data.Entities;
using System.Collections.Generic;

namespace CashTrack.Common
{
    public class AppSettingsOptions
    {
        public const string AppSettings = "AppSettings";
        public string Secret { get; set; }
        public string IpAddress { get; set; }
        public Dictionary<string, string> ConnectionStrings { get; set; }
    }
}
