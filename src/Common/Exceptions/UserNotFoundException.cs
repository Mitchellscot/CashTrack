using System;

namespace CashTrack.Common.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException()
        {

        }
        public UserNotFoundException(string id) : base(String.Format($"No user found with an Id of {id}"))
        {

        }
        public UserNotFoundException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
