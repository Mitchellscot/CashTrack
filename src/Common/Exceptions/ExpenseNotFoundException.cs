using System;

namespace CashTrack.Common.Exceptions
{
    public class ExpenseNotFoundException : Exception
    {
        public ExpenseNotFoundException()
        {

        }
        public ExpenseNotFoundException(string id) : base(String.Format($"No expense found with an id of {id}"))
        {

        }
        public ExpenseNotFoundException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
