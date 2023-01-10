using CashTrack.Models.ExpenseModels;
using Shouldly;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
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
            sut.SetTaxIfTaxed();
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
            sut.SetTaxIfTaxed();
            sut.Amount.ShouldBe(randomAmount);
        }
        [Fact]
        public void Error_When_Tax_Is_Too_High()
        {
            var sut = new ExpenseSplit()
            {
                Taxed = false,
                Tax = 1M,
                Amount = 100m,
                SubCategoryId = 1,
                Date = DateTime.Today,
                Merchant = "mitchell"
            };

            ValidateModel(sut).Any(
                x => x!.MemberNames.Contains("Tax") &&
                x!.ErrorMessage!.Contains("must be between"))
                .ShouldBeTrue();
        }
        [Fact]
        public void Error_When_Tax_Is_Too_low()
        {
            var sut = new ExpenseSplit()
            {
                Taxed = false,
                Tax = 0m,
                Amount = 100m,
                SubCategoryId = 1,
                Date = DateTime.Today,
                Merchant = "mitchell"
            };

            ValidateModel(sut).Any(
                x => x!.MemberNames.Contains("Tax") &&
                x!.ErrorMessage!.Contains("must be between"))
                .ShouldBeTrue();
        }
        [Fact]
        public void Error_When_Amount_Is_Too_low()
        {
            var sut = new ExpenseSplit()
            {
                Taxed = false,
                Tax = 0.1m,
                Amount = -100m,
                SubCategoryId = 1,
                Date = DateTime.Today,
                Merchant = "mitchell"
            };

            ValidateModel(sut).Any(
                x => x!.MemberNames.Contains("Amount") &&
                x!.ErrorMessage!.Contains("must be between"))
                .ShouldBeTrue();
        }
        //handy method for testing validation requirements of System.ComponentModel.DataAnnotations !
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            var valid = Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }
    }
}

