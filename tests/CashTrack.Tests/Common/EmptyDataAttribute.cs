using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;
using System.Reflection;

namespace CashTrack.Tests.Common
{
    internal class EmptyDataAttribute : DataAttribute
    {
        private readonly Random _random;
        public EmptyDataAttribute()
        {
            _random = new Random();
        }
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { "" };
            yield return new object[] { " " };
            yield return new object[] { string.Empty };

        }
    }
}
