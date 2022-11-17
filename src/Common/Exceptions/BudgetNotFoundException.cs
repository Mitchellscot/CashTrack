using System;

namespace CashTrack.Common.Exceptions
{
    public class BudgetNotFoundException : Exception
    {
        public BudgetNotFoundException()
        {

        }
        public BudgetNotFoundException(string id) : base(String.Format($"No Budget found with an id of {id}"))
        {

        }
        public BudgetNotFoundException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
