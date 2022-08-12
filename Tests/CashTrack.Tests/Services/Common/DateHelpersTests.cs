using CashTrack.Data.Entities;
using CashTrack.Models.Common;
using CashTrack.Models.ExpenseModels;
using CashTrack.Services.Common;
using Shouldly;
using System;
using Xunit;

namespace CashTrack.Tests.Services.Common
{
    public class DateHelpersTests
    {
        [Fact]
        public void Get_Current_Month_Works()
        {
            var currentMonth = DateHelpers.GetCurrentMonth();
            currentMonth.Month.ShouldBe(DateTime.Today.Month);
        }
        [Fact]
        public void Get_Current_Quarter_Works()
        {
            //not the best test but whatever
            var currentQuarter = DateHelpers.GetCurrentQuarter();
            currentQuarter.Year.ShouldBe(DateTime.Today.Year);
        }
        [Fact]
        public void Get_Current_Year_Works()
        {
            var currentYear = DateHelpers.GetCurrentYear();
            currentYear.Year.ShouldBe(DateTime.Today.Year);
        }
        [Fact]
        public void Get_Current_Month_Dates_Works()
        {
            var currentMonthDates = DateHelpers.GetMonthDatesFromDate(DateTime.UtcNow);
            currentMonthDates.startDate.Day.ShouldBe(1);
            currentMonthDates.startDate.Month.ShouldBe(DateTime.UtcNow.Month);
        }
        [Fact]
        public void Get_Last_Day_Of_Month_Works()
        {
            var lastDay = DateHelpers.GetLastDayOfMonth(DateTime.UtcNow);
            lastDay.ShouldBeGreaterThan(27);
            lastDay.ShouldBeLessThan(32);
        }
        [Fact]
        public void Get_Quarter_Dates_Works()
        {
            var dates = DateHelpers.GetQuarterDatesFromDate(DateTime.UtcNow);
            dates.startDate.ShouldBeLessThan(DateTime.UtcNow);
            dates.endDate.ShouldBeGreaterThan(DateTime.UtcNow);
        }
        [Fact]
        public void Get_Year_Dates_Works()
        {
            var dates = DateHelpers.GetYearDatesFromDate(DateTime.UtcNow);
            dates.startDate.Day.ShouldBe(1);
            dates.endDate.Day.ShouldBe(31);
            dates.startDate.Year.ShouldBe(DateTime.UtcNow.Year);
        }

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
        public void Get_Predicate_Works(DateOptions option)
        {
            var request = new ExpenseRequest() { DateOptions = option };
            var result = DateOption<ExpenseEntity, ExpenseRequest>.Parse(request);
            result.NodeType.ShouldBe(System.Linq.Expressions.ExpressionType.Lambda);
            result.ShouldNotBeNull();
        }
    }
}
