using System;

namespace CashTrack.Common.Exceptions
{

    public class IncomeNotFoundException : Exception
    {
        public IncomeNotFoundException()
        {

        }
        public IncomeNotFoundException(string id) : base(String.Format($"No Income found with an Id of {id}"))
        {

        }
        public IncomeNotFoundException(string message, Exception inner) : base(message, inner)
        {

        }
    }

}
