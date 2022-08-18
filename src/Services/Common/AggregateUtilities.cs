using CashTrack.Data.Entities.Common;
using System;
using System.Collections.Generic;
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

            var monthlyStats = transactions.GroupBy(e => e)
            .Select(g =>
            {
                var results = g.Aggregate(new StatisticsAggregator<T>(),
                    (acc, x) => acc.Accumulate(x),
                    acc => acc.Compute());

                return new MonthlyStatistics()
                {
                    Date = g.Key.Date,
                    Average = results.Average,
                    Min = results.Min,
                    Max = results.Max,
                    Total = results.Total,
                    Count = results.Count
                };
            }).OrderBy(x => x.Date).ToList();
            var year = transactions.FirstOrDefault().Date.Year;
            for (var i = 1; i <= 12; i++)
            {
                if (monthlyStats.Any(x => x.Date.Month == i))
                {
                    monthlyStatistics.Add(monthlyStats.Where(x => x.Date.Month == i).FirstOrDefault());
                }
                else
                {
                    monthlyStatistics.Add(new MonthlyStatistics()
                    {
                        Date = new DateTime(year, i, 1),
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
