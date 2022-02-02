using System;

namespace CashTrack.Common.Exceptions
{
    public class DuplicateNameException : Exception
    {
        public DuplicateNameException()
        {

        }
        public DuplicateNameException(string name, string type) : base(String.Format($"{type} already has a the name {name} - please use another name."))
        {

        }
        public DuplicateNameException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
