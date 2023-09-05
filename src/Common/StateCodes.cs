using System.Collections.Generic;

namespace CashTrack.Common
{
    public static class StateCodes
    {
        public static List<string> GetStateCodes()
        {
            List<string> codes = new List<string>
        {
            "AL", "AK", "AZ", "AR", "CA", "CO", "CT", "DE", "DC", "FL", "GA",
            "HI", "ID", "IL", "IN", "IA", "KS", "KY", "LA", "ME", "MD",
            "MA", "MI", "MN", "MS", "MO", "MT", "NE", "NV", "NH", "NJ",
            "NM", "NY", "NC", "ND", "OH", "OK", "OR", "PA", "RI", "SC",
            "SD", "TN", "TX", "UT", "VT", "VA", "WA", "WV", "WI", "WY"
        };
            return codes;
        }
        public static List<string> GetMyStateCodes()
        {
            List<string> codes = new List<string>
        {
            "CA", "MN", "VA", "Various"
        };
            return codes;
        }
    }
}
