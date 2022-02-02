using CashTrack.Data.Entities.Common;
using CashTrack.Models.Common;
using System;
using System.Linq.Expressions;

namespace CashTrack.Services.Common
{
    public static class DateHelpers
    {
        public static DateTimeOffset GetCurrentMonth() => new DateTimeOffset(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0, new TimeSpan(0, 0, 0));

        public static DateTimeOffset GetCurrentQuarter() => GetQuarterDatesFromDate(DateTime.UtcNow).startDate;

        public static DateTimeOffset GetCurrentYear() => new DateTimeOffset(DateTime.Now.Year, 1, 1, 0, 0, 0, new TimeSpan(0, 0, 0));

        public static (DateTimeOffset startDate, DateTimeOffset endDate) GetMonthDatesFromDate(DateTimeOffset date)
        {
            var beginingOfMonth = new DateTimeOffset(
                date.Year, date.Month, 1, 0, 0, 0, new TimeSpan(0, 0, 0)
                );
            var monthEndDate = GetLastDayOfMonth(date);
            var endingOfMonth = new DateTimeOffset(
                date.Year, date.Month, monthEndDate, 0, 0, 0, new TimeSpan(0, 0, 0)
                );
            return (beginingOfMonth.ToUniversalTime(), endingOfMonth.ToUniversalTime());
        }
        internal static int GetLastDayOfMonth(DateTimeOffset date) => date.Month switch
        {
            4 or 6 or 9 or 11 => 30,
            1 or 3 or 5 or 7 or 8 or 10 or 12 => 31,
            2 => DateTime.IsLeapYear(date.Year) ? 29 : 28,
            _ => throw new ArgumentException($"Unable to determine the end of the month {date}", nameof(date.Month))
        };
        public static (DateTimeOffset startDate, DateTimeOffset endDate) GetQuarterDatesFromDate(DateTimeOffset date) => date.Month switch
        {
            1 or 2 or 3 => (startDate: new DateTimeOffset(date.Year, 1, 1, 0, 0, 0, new TimeSpan(0, 0, 0)), endDate: new DateTimeOffset(date.Year, 3, 31, 0, 0, 0, new TimeSpan(0, 0, 0))),
            4 or 5 or 6 => (startDate: new DateTimeOffset(date.Year, 4, 1, 0, 0, 0, new TimeSpan(0, 0, 0)), endDate: new DateTimeOffset(date.Year, 6, 30, 0, 0, 0, new TimeSpan(0, 0, 0))),
            7 or 8 or 9 => (startDate: new DateTimeOffset(date.Year, 7, 1, 0, 0, 0, new TimeSpan(0, 0, 0)), endDate: new DateTimeOffset(date.Year, 9, 30, 0, 0, 0, new TimeSpan(0, 0, 0))),
            10 or 11 or 12 => (startDate: new DateTimeOffset(date.Year, 10, 1, 0, 0, 0, new TimeSpan(0, 0, 0)), endDate: new DateTimeOffset(date.Year, 12, 31, 0, 0, 0, new TimeSpan(0, 0, 0))),
            _ => throw new ArgumentException($"Unable to determine quarter from given date {date}", nameof(date))
        };
        public static (DateTimeOffset startDate, DateTimeOffset endDate) GetYearDatesFromDate(DateTimeOffset date)
        {
            return (startDate: new DateTimeOffset(date.Year, 1, 1, 0, 0, 0, new TimeSpan(0, 0, 0)), endDate: new DateTimeOffset(date.Year, 12, 31, 0, 0, 0, new TimeSpan(0, 0, 0)));
        }
    }
    public static class DateOption<T, TRequest> where T : Transactions where TRequest : TransactionRequest
    {
        public static Expression<Func<T, bool>> Parse(TRequest request) => request.DateOptions switch
        {
            //1
            DateOptions.All => (T x) => true,
            //2
            DateOptions.SpecificDate => (T x) =>
                x.date == request.BeginDate.ToUniversalTime(),
            //3
            DateOptions.SpecificMonthAndYear => (T x) =>
               x.date >= DateHelpers.GetMonthDatesFromDate(request.BeginDate).startDate &&
               x.date <= DateHelpers.GetMonthDatesFromDate(request.BeginDate).endDate,
            //4
            DateOptions.SpecificQuarter => (T x) =>
                x.date >= DateHelpers.GetQuarterDatesFromDate(request.BeginDate).startDate &&
                x.date <= DateHelpers.GetQuarterDatesFromDate(request.BeginDate).endDate,
            //5
            DateOptions.SpecificYear => (T x) =>
                x.date >= DateHelpers.GetYearDatesFromDate(request.BeginDate).startDate &&
                x.date <= DateHelpers.GetYearDatesFromDate(request.BeginDate).endDate,
            //6
            DateOptions.DateRange => (T x) =>
                x.date >= request.BeginDate.ToUniversalTime() &&
                x.date <= request.EndDate.ToUniversalTime(),
            //7
            DateOptions.Last30Days => (T x) =>
                x.date >= DateTimeOffset.UtcNow.AddDays(-30),
            //8
            DateOptions.CurrentMonth => (T x) =>
                x.date >= DateHelpers.GetCurrentMonth() &&
                x.date <= DateTimeOffset.UtcNow,
            //9
            DateOptions.CurrentQuarter => (T x) =>
                x.date >= DateHelpers.GetCurrentQuarter() &&
                x.date <= DateTimeOffset.UtcNow,
            //10
            DateOptions.CurrentYear => (T x) =>
                x.date >= DateHelpers.GetCurrentYear() &&
                x.date <= DateTimeOffset.UtcNow,

            _ => throw new ArgumentException($"DateOption type not supported {request.DateOptions}", nameof(request.DateOptions))

        };
    }
}
