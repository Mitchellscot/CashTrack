using Xunit;
using Shouldly;
using Bogus;
using CashTrack.Common.Extensions;

namespace CashTrack.Tests.Unit
{
    public class ExtensionTests
    {
        private readonly Faker _faker;

        public ExtensionTests()
        {
            _faker = new Bogus.Faker();
        }
        [Fact]
        public void Strings_Should_Be_Equal()
        {
            var stringOne = _faker.Random.Words();
            var stringTwo = stringOne;
            stringOne.IsEqualTo(stringTwo).ShouldBeTrue();
        }
        [Fact]
        public void Strings_Should_Not_Be_Equal()
        {
            var stringOne = _faker.Random.Words();
            var stringTwo = _faker.Random.Words();
            stringOne.IsEqualTo(stringTwo).ShouldBeFalse();
        }
        [Fact]
        public void Accumulate_Adds_Up_Integers()
        {
            var x = new[] { 1, 2, 3, 4, 5 };
            var y = x.Accumulate();
            y.ShouldBe(new[] { 1, 3, 6, 10, 15 });
        }
        [Fact]
        public void Accumulate_Adds_Up_Decimals()
        {
            decimal[] x = new[] { 1.1M, 2.2M, 3.3M, 4.4M, 5.5M };
            var y = x.Accumulate();
            y.ShouldBe(new[] { 1.1M, 3.3M, 6.6M, 11M, 16.5M });
        }
        [Fact]
        public void ToPercentage_Works_On_Integers()
        {
            var num = 40;
            var total = 1000;
            var percentage = num.ToPercentage(total);
            percentage.ShouldBe(4);
        }
        [Fact]
        public void ToPercentage_Works_On_Decimals_Returns_Integers()
        {
            var num = 40M;
            var total = 1000M;
            var percentage = num.ToPercentage(total);
            percentage.ShouldBe(4);
        }
        [Fact]
        public void ToPercentage_Works_On_Decimals_Returns_0_When_Below_One_Percent()
        {
            var num = 4M;
            var total = 1000M;
            var percentage = num.ToPercentage(total);
            percentage.ShouldBe(0);
        }
        [Fact]
        public void ToDecimalPercentage_Works_On_Decimals()
        {
            var num = .8M;
            var total = 100M;
            var percentage = num.ToDecimalPercentage(total);
            percentage.ShouldBe(0.8M);
        }
        [Fact]
        public void ToDecimalPercentage_Works_On_Integers()
        {
            var num = 8;
            var total = 100;
            var percentage = num.ToDecimalPercentage(total);
            percentage.ShouldBe(8M);
        }
    }
}
