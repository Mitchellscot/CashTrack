using System;
using System.Collections.Generic;
using Xunit.Sdk;
using System.Reflection;

namespace CashTrack.IntegrationTests.Common
{
    public class ExpenseIdDataAttribute : DataAttribute
    {
        private readonly Random _random;
        public ExpenseIdDataAttribute()
        {
            _random = new Random();
        }
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { _random.Next(1, 7176) };
            yield return new object[] { _random.Next(1, 7176) };
            yield return new object[] { _random.Next(1, 7176) };
            yield return new object[] { _random.Next(1, 7176) };
            yield return new object[] { _random.Next(1, 7176) };

        }
    }
}




