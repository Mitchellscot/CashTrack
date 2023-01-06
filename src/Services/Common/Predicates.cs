using CashTrack.Common;
using CashTrack.Data.Entities;
using CashTrack.Models.Common;
using System;
using System.Linq.Expressions;

namespace CashTrack.Services.Common
{
    /// <summary>
    /// SQLite treats dates differently than SQL Server, so this class accounts for the differences. 
    /// </summary>
    public static class Predicates
    {
        private const string Variable = "ASPNETCORE_ENVIRONMENT";
        public static Expression<Func<ExpenseEntity, bool>> GetExpenses(TimeFrame timeFrame, int year = 0, int month = 0)
        {
            switch (timeFrame)
            {
                case TimeFrame.Year:
                    if (Environment.GetEnvironmentVariable(Variable) == CashTrackEnv.RaspberryPi)
                    {
                        var firstDate = new DateTime(year, 1, 1);
                        var lastDay = new DateTime(year, 12, 31);
                        return x => x.Date >= firstDate && x.Date <= lastDay && !x.ExcludeFromStatistics;
                    }
                    else
                    {
                        return x => x.Date.Year == year && !x.ExcludeFromStatistics;
                    }
                case TimeFrame.Month:
                    if (Environment.GetEnvironmentVariable(Variable) == CashTrackEnv.RaspberryPi)
                    {
                        var firstDate = new DateTime(year, month, 1);
                        var lastDay = new DateTime(year, month, 31);
                        return x => x.Date >= firstDate && x.Date <= lastDay && !x.ExcludeFromStatistics;
                    }
                    else
                    {
                        return x => x.Date.Year == year && !x.ExcludeFromStatistics;
                    }
                default: throw new ArgumentOutOfRangeException(nameof(timeFrame));
            }
        }
        public static Expression<Func<IncomeEntity, bool>> GetIncome(TimeFrame timeFrame, int year = 0, int month = 0)
        {
            switch (timeFrame)
            {
                case TimeFrame.Year:
                    if (Environment.GetEnvironmentVariable(Variable) == CashTrackEnv.RaspberryPi)
                    {
                        var firstDate = new DateTime(year, 1, 1);
                        var lastDay = new DateTime(year, 12, 31);
                        return x => x.Date >= firstDate && x.Date <= lastDay && !x.IsRefund;
                    }
                    else
                    {
                        return x => x.Date.Year == year && !x.IsRefund;
                    }
                case TimeFrame.Month:
                    if (Environment.GetEnvironmentVariable(Variable) == CashTrackEnv.RaspberryPi)
                    {
                        var firstDate = new DateTime(year, month, 1);
                        var lastDay = new DateTime(year, month, 31);
                        return x => x.Date >= firstDate && x.Date <= lastDay && !x.IsRefund;
                    }
                    else
                    {
                        return x => x.Date.Year == year && !x.IsRefund;
                    }
                default: throw new ArgumentOutOfRangeException(nameof(timeFrame));
            }
        }
        public enum TimeFrame
        {
            Year,
            Month
        }
    }
}
