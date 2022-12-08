using System;

namespace CashTrack.Common.Exceptions
{
    public class IncomeSourceNotFoundException : Exception
    {
        public IncomeSourceNotFoundException()
        {

        }
        public IncomeSourceNotFoundException(string id) : base(String.Format($"Unable to find income source with Id {id}"))
        {

        }
        public IncomeSourceNotFoundException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
