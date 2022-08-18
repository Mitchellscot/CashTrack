using CashTrack.Data.Entities.Common;
using CashTrack.Models.Common;
using System;
using System.Linq.Expressions;

namespace CashTrack.Services.Common
{
    public static class DateHelpers
    {
        public static DateTime GetCurrentMonth() => new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);

        public static DateTime GetCurrentQuarter() => GetQuarterDatesFromDate(DateTime.Now).startDate;

        public static DateTime GetCurrentYear() => new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0);

        public static (DateTime startDate, DateTime endDate) GetMonthDatesFromDate(DateTime date)
        {
            var beginingOfMonth = new DateTime(
                date.Year, date.Month, 1, 0, 0, 0
                );
            var monthEndDate = GetLastDayOfMonth(date);
            var endingOfMonth = new DateTime(
                date.Year, date.Month, monthEndDate, 0, 0, 0
                );
            return (beginingOfMonth, endingOfMonth);
        }
        internal static int GetLastDayOfMonth(DateTime date) => date.Month switch
        {
            4 or 6 or 9 or 11 => 30,
            1 or 3 or 5 or 7 or 8 or 10 or 12 => 31,
            2 => DateTime.IsLeapYear(date.Year) ? 29 : 28,
            _ => throw new ArgumentException($"Unable to determine the end of the month {date}", nameof(date.Month))
        };
        public static (DateTime startDate, DateTime endDate) GetQuarterDatesFromDate(DateTime date) => date.Month switch
        {
            1 or 2 or 3 => (startDate: new DateTime(date.Year, 1, 1, 0, 0, 0), endDate: new DateTime(date.Year, 3, 31, 0, 0, 0)),
            4 or 5 or 6 => (startDate: new DateTime(date.Year, 4, 1, 0, 0, 0), endDate: new DateTime(date.Year, 6, 30, 0, 0, 0)),
            7 or 8 or 9 => (startDate: new DateTime(date.Year, 7, 1, 0, 0, 0), endDate: new DateTime(date.Year, 9, 30, 0, 0, 0)),
            10 or 11 or 12 => (startDate: new DateTime(date.Year, 10, 1, 0, 0, 0), endDate: new DateTime(date.Year, 12, 31, 0, 0, 0)),
            _ => throw new ArgumentException($"Unable to determine quarter from given date {date}", nameof(date))
        };
        public static (DateTime startDate, DateTime endDate) GetYearDatesFromDate(DateTime date)
        {
            return (startDate: new DateTime(date.Year, 1, 1, 0, 0, 0), endDate: new DateTime(date.Year, 12, 31, 0, 0, 0));
        }
        public static DateTime GetLast30Days()
        {
            return DateTime.Today.AddDays(-30).Date;
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
                x.Date == request.BeginDate,
            //3
            DateOptions.SpecificMonthAndYear => (T x) =>
               x.Date >= DateHelpers.GetMonthDatesFromDate(request.BeginDate).startDate &&
               x.Date <= DateHelpers.GetMonthDatesFromDate(request.BeginDate).endDate,
            //4
            DateOptions.SpecificQuarter => (T x) =>
                x.Date >= DateHelpers.GetQuarterDatesFromDate(request.BeginDate).startDate &&
                x.Date <= DateHelpers.GetQuarterDatesFromDate(request.BeginDate).endDate,
            //5
            DateOptions.SpecificYear => (T x) =>
                x.Date >= DateHelpers.GetYearDatesFromDate(request.BeginDate).startDate &&
                x.Date <= DateHelpers.GetYearDatesFromDate(request.BeginDate).endDate,
            //6
            DateOptions.DateRange => (T x) =>
                x.Date >= request.BeginDate &&
                x.Date <= request.EndDate,
            //7
            DateOptions.Last30Days => (T x) =>
                x.Date >= DateHelpers.GetLast30Days(),
            //8
            DateOptions.CurrentMonth => (T x) =>
                x.Date >= DateHelpers.GetCurrentMonth() &&
                x.Date <= DateTime.Today,
            //9
            DateOptions.CurrentQuarter => (T x) =>
                x.Date >= DateHelpers.GetCurrentQuarter() &&
                x.Date <= DateTime.Today,
            //10
            DateOptions.CurrentYear => (T x) =>
                x.Date >= DateHelpers.GetCurrentYear() &&
                x.Date <= DateTime.Today,

            _ => throw new ArgumentException($"DateOption type not supported {request.DateOptions}", nameof(request.DateOptions))

        };
    }
}
