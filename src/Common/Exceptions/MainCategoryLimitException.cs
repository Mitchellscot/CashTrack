using System;

namespace CashTrack.Common.Exceptions
{
    public class MainCategoryLimitException : Exception
    {
        public MainCategoryLimitException()
        {

        }
        public MainCategoryLimitException(int number) : base(String.Format($"You must keep the number of main categories below 25. If you would like to create more categories, consider combining main categories or creating more sub categories. You currently have {number} categories."))
        {

        }
        public MainCategoryLimitException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
