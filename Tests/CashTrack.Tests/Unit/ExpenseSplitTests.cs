using Bogus.DataSets;
using CashTrack.Models.ExpenseModels;
using CashTrack.Models.MerchantModels;
using CashTrack.Models.SubCategoryModels;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CashTrack.Tests.Unit
{
    public class ExpenseSplitTests
    {
        [Fact]
        public void Adds_Tax_Amount_When_Taxed_Is_True()
        {
            var rando = new Random();
            var randomTax = rando.NextDouble();
            var randomAmount = rando.Next(0, 10000);
            var sut = new ExpenseSplit()
            {
                Taxed = true,
                Tax = Convert.ToDecimal(randomTax),
                Amount = Convert.ToDecimal(randomAmount),
                SubCategoryId = 1,
                Date = DateTime.Today,
                Merchant = "mitchell"
            };
            var expectedValue = decimal.Round(Convert.ToDecimal((randomAmount * randomTax) + randomAmount), 2);
            sut.Amount.ShouldBe(expectedValue);
        }
        [Fact]
        public void Does_Not_Adds_Tax_Amount_When_Taxed_Is_False()
        {
            var rando = new Random();
            var randomTax = rando.NextDouble();
            var randomAmount = rando.Next(0, 10000);
            var sut = new ExpenseSplit()
            {
                Taxed = false,
                Tax = Convert.ToDecimal(randomTax),
                Amount = Convert.ToDecimal(randomAmount),
                SubCategoryId = 1,
                Date = DateTime.Today,
                Merchant = "mitchell"
            };
            sut.Amount.ShouldBe(randomAmount);
        }
    }
}

