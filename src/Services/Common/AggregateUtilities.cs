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
            var monthlyStatistics = new List<MonthlyStatistics>();

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
            }).OrderBy(x => x.Month).ToList();
            var statisticsCompressedByMonth = monthlyStats.GroupBy(m => m.MonthNumber).Select(g =>
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
            for (var i = 1; i <= 12; i++)
            {
                if (statisticsCompressedByMonth.Any(x => x.MonthNumber == i))
                {

                    monthlyStatistics.Add(statisticsCompressedByMonth.FirstOrDefault(m => m.MonthNumber == i));
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
    }
}
