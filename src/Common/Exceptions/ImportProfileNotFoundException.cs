using System;

namespace CashTrack.Common.Exceptions
{
    public class ImportProfileNotFoundException : Exception
    {
        public ImportProfileNotFoundException()
        {

        }
        public ImportProfileNotFoundException(string id) : base(String.Format($"Unable to find Import Profile with Id {id}"))
        {

        }
        public ImportProfileNotFoundException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
