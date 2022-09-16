using CashTrack.Data.Entities.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CashTrack.Services.Common
{
    public static class AggregateUtilities<T> where T : Transactions
    {
        public static List<AnnualStatistics> GetAnnualStatistics(T[] transactions)
        {
            return transactions.GroupBy(e => e.Date.Year)
                            .Select(g =>
                            {
                                var results = g.Aggregate(new StatisticsAggregator<T>(),
                                    (acc, e) => acc.Accumulate(e),
                                    acc => acc.Compute());
                                return new AnnualStatistics()
                                {
                                    Year = g.Key,
                                    Average = results.Average,
                                    Min = results.Min,
                                    Max = results.Max,
                                    Total = results.Total,
                                    Count = results.Count
                                };
                            }).OrderBy(x => x.Year).ToList();
        }
        public static List<MonthlyStatistics> GetMonthlyStatistics(T[] transactions)
        {
            var monthlyStats = transactions.GroupBy(e => e.Date.Month)
            .Select(g =>
            {
                var results = g.Aggregate(new StatisticsAggregator<T>(),
                    (acc, x) => acc.Accumulate(x),
                    acc => acc.Compute());

                return new MonthlyStatistics()
                {
                    MonthNumber = g.Key,
                    Average = results.Average,
                    Min = results.Min,
                    Max = results.Max,
                    Total = results.Total,
                    Count = results.Count
                };
            }).GroupBy(m => m.MonthNumber).Select(g =>
            {
                var results = g.Aggregate(new MonthlyStatisticsAggregator(),
                    (acc, x) => acc.Accumulate(x),
                    acc => acc.Compute());

                return new MonthlyStatistics()
                {
                    MonthNumber = g.Key,
                    Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key),
                    Average = results.Average,
                    Min = results.Min,
                    Max = results.Max,
                    Total = results.MonthlyTotal,
                    Count = results.Count
                };
            }).OrderByDescending(m => m.MonthNumber).ToList();
            var year = transactions.FirstOrDefault().Date.Year;
            var monthlyStatistics = new List<MonthlyStatistics>();
            for (var i = 1; i <= 12; i++)
            {
                if (monthlyStats.Any(x => x.MonthNumber == i))
                {

                    monthlyStatistics.Add(monthlyStats.FirstOrDefault(m => m.MonthNumber == i));
                }
                else
                {
                    monthlyStatistics.Add(new MonthlyStatistics()
                    {
                        MonthNumber = i,
                        Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i),
                        Average = 0,
                        Min = 0,
                        Max = 0,
                        Total = 0,
                        Count = 0
                    });
                }
            }
            return monthlyStatistics;
        }
        public static List<MonthlyStatistics> GetStatisticsLast12Months(T[] transactions)
        {
            var thisYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;
            var monthlyStatistics = new List<MonthlyStatistics>();
            for (var i = 0; i < 12; i++)
            {
                var month = currentMonth - i;
                var year = thisYear;
                if (month < 1)
                {
                    month = 12 - Math.Abs(currentMonth - i);
                    year = thisYear - 1;
                }

                var stats = GetMonthlyStatisticsFromMonthYear(year, month, transactions);
                if (stats.Any(x => x.MonthNumber == month))
                {
                    monthlyStatistics.Add(stats.FirstOrDefault(m => m.MonthNumber == month));
                }
                else
                {
                    monthlyStatistics.Add(new MonthlyStatistics()
                    {
                        MonthNumber = month,
                        Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month).Remove(3),
                        Average = 0,
                        Min = 0,
                        Max = 0,
                        Total = 0,
                        Count = 0
                    });
                }
            }
            monthlyStatistics.Reverse();
            return monthlyStatistics;
        }
        private static List<MonthlyStatistics> GetMonthlyStatisticsFromMonthYear(int year, int month, T[] transactions)
        {
            var thisYearsTransactions = transactions.Where(x => x.Date.Year == year).ToList();
            return thisYearsTransactions.GroupBy(e => e.Date.Month).Select(g =>
            {
                var results = g.Aggregate(new StatisticsAggregator<T>(),
                    (acc, x) => acc.Accumulate(x), acc => acc.Compute());
                return new MonthlyStatistics()
                {
                    MonthNumber = g.Key,
                    Average = results.Average,
                    Min = results.Min,
                    Max = results.Max,
                    Total = results.Total,
                    Count = results.Count
                };
            }).GroupBy(s => s.MonthNumber).Select(g =>
            {
                var results = g.Aggregate(new MonthlyStatisticsAggregator(),
                    (acc, x) => acc.Accumulate(x),
                    acc => acc.Compute());

                return new MonthlyStatistics()
                {
                    MonthNumber = g.Key,
                    Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key).Remove(3),
                    Average = results.Average,
                    Min = results.Min,
                    Max = results.Max,
                    Total = results.MonthlyTotal,
                    Count = results.Count
                };
            }).OrderByDescending(x => x.MonthNumber).ToList();
        }
    }

}
