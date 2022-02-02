using Bogus;
using CashTrack.Services.Common;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CashTrack.Tests.Helpers
{
    public class DateHelpersTests
    {
        private readonly Faker _faker;

        public DateHelpersTests()
        {
            _faker = new Faker();

        }
        [Theory]
        [InlineData("2021-01-05")]
        [InlineData("2011-02-05")]
        [InlineData("2012-03-05")]
        [InlineData("2023-04-05")]
        [InlineData("2024-05-05")]
        [InlineData("2015-06-05")]
        [InlineData("2016-07-05")]
        [InlineData("2017-08-05")]
        [InlineData("2019-09-05")]
        [InlineData("2003-10-05")]
        [InlineData("2009-11-05")]
        [InlineData("2005-12-03")]
        public void GetMonthDatesWorks(string month)
        {
            var parsedMonth = DateTimeOffset.Parse(month);
            var result = DateHelpers.GetMonthDatesFromDate(parsedMonth);
            var beginingOfMonth = new DateTimeOffset(parsedMonth.Year, parsedMonth.Month, 1, 0, 0, 0, new TimeSpan(0, 0, 0));

            result.startDate.ShouldBe(beginingOfMonth);
            result.endDate.Day.ShouldBeOneOf(28, 29, 30, 31);
            result.endDate.Year.ShouldBe(parsedMonth.Year);
        }
        [Fact]
        public void GetYearDatesWorks()
        {
            var date = _faker.Date.PastOffset(2021);
            var result = DateHelpers.GetYearDatesFromDate(date);
            result.startDate.Year.ShouldBe(date.Year);
            result.startDate.Day.ShouldBe(1);
            result.endDate.Day.ShouldBe(31);
        }
        [Fact]
        public void GetQuarterDatesWorks()
        {
            for (int i = 0; i < 10; i++)
            {
                var date = _faker.Date.PastOffset(i);
                var result = DateHelpers.GetQuarterDatesFromDate(date);
                result.startDate.Month.ShouldBeOneOf(1, 4, 7, 10);
            }
        }
        [Fact]
        public void GetCurrentYearWorks()
        {
            var result = DateHelpers.GetCurrentYear();
            result.Year.ShouldBe(DateTimeOffset.UtcNow.Year);
        }
        [Fact]
        public void GetCurrentMonthWorks()
        {
            var result = DateHelpers.GetCurrentMonth();
            result.Month.ShouldBe(DateTimeOffset.UtcNow.Month);
        }
        [Fact]
        public void GetCurrentQuarterWorks()
        {
            var result = DateHelpers.GetCurrentQuarter();
            result.Month.ShouldBeOneOf(1, 4, 7, 10);
        }
        [Fact]
        public void GetLastDayOfMonthWorks()
        {
            var rand = new Random();
            for (int i = 0; i < 10; i++)
            {
                var date = _faker.Date.PastOffset(rand.Next(1, 2020));
                var result = DateHelpers.GetLastDayOfMonth(date);
                result.ShouldBeOneOf(28, 29, 30, 31);
            }
        }
    }
}
