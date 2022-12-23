using System;

namespace CashTrack.Common.Exceptions
{
    public class IncomeSourceNotFoundException : Exception
    {
        public IncomeSourceNotFoundException()
        {

        }
        public IncomeSourceNotFoundException(string name) : base(String.Format($"Unable to find income source {name}"))
        {

        }
        public IncomeSourceNotFoundException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
