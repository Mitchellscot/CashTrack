using System;

namespace CashTrack.Common.Exceptions
{
    public class MerchantNotFoundException : Exception
    {
        public MerchantNotFoundException()
        {

        }
        public MerchantNotFoundException(string id) : base(String.Format($"No merchant found with an id of {id}"))
        {

        }
        public MerchantNotFoundException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
