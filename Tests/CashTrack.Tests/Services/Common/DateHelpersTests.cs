using CashTrack.Data.Entities;
using CashTrack.Models.Common;
using CashTrack.Models.ExpenseModels;
using CashTrack.Services.Common;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CashTrack.Tests.Services.Common
{
    public class DateHelpersTests
    {
        [Theory]
        [InlineData(DateOptions.All)]
        [InlineData(DateOptions.SpecificDate)]
        [InlineData(DateOptions.SpecificMonthAndYear)]
        [InlineData(DateOptions.SpecificQuarter)]
        [InlineData(DateOptions.SpecificYear)]
        [InlineData(DateOptions.DateRange)]
        [InlineData(DateOptions.Last30Days)]
        [InlineData(DateOptions.CurrentMonth)]
        [InlineData(DateOptions.CurrentQuarter)]
        [InlineData(DateOptions.CurrentYear)]
        public void GetPredicateWorks(DateOptions option)
        {
            var request = new ExpenseRequest() { DateOptions = option };
            var result = DateOption<ExpenseEntity, ExpenseRequest>.Parse(request);
            result.NodeType.ShouldBe(System.Linq.Expressions.ExpressionType.Lambda);
            result.ShouldNotBeNull();
        }
    }
}
