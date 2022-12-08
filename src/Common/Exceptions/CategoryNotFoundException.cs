using System;

namespace CashTrack.Common.Exceptions
{
    public class CategoryNotFoundException : Exception
    {
        public CategoryNotFoundException()
        {

        }
        public CategoryNotFoundException(string id) : base(String.Format($"Unable to find category with Id {id}"))
        {

        }
        public CategoryNotFoundException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
