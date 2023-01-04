using System;

namespace CashTrack.Common.Exceptions
{
    public class DuplicateNameException : Exception
    {
        public DuplicateNameException()
        {

        }
        public DuplicateNameException(string name) : base(String.Format($"The name {name} is already in use - please use another name."))
        {

        }
        public DuplicateNameException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
