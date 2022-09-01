using System;

namespace CashTrack.Common.Exceptions
{
    public class ImportRuleNotFoundException : Exception
    {
        public ImportRuleNotFoundException()
        {

        }
        public ImportRuleNotFoundException(string id) : base(String.Format($"Unable to find Import Rule with id {id}"))
        {

        }
        public ImportRuleNotFoundException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
