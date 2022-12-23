using System;

namespace CashTrack.Common.Exceptions
{
    public class MerchantNotFoundException : Exception
    {
        public MerchantNotFoundException()
        {

        }
        public MerchantNotFoundException(string name) : base(String.Format($"No merchant found named {name}"))
        {

        }
        public MerchantNotFoundException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
