using CashTrack.Data.Entities.Common;
using System;
using System.Globalization;

namespace CashTrack.Services.Common
{
    public class StatisticsAggregator<T> where T : Transactions
    {
        public decimal Max { get; set; }
        public decimal Min { get; set; }
        public decimal Total { get; set; }
        public int Count { get; set; }
        public decimal Average { get; set; }
        public StatisticsAggregator()
        {
            Max = decimal.MinValue;
            Min = decimal.MaxValue;
        }
        public StatisticsAggregator<T> Accumulate(T e)
        {
            Total += e.Amount;
            Count++;
            Max = Math.Max(Max, e.Amount);
            Min = Math.Min(Min, e.Amount);
            return this;
        }
        public StatisticsAggregator<T> Compute()
        {
            Average = Math.Round(Total / Count, 2);
            return this;
        }
    }
    public record AnnualStatistics
    {
        public int Year { get; set; }
        public int Count { get; set; }
        public decimal Average { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public decimal Total { get; set; }
    }
    public class MonthlyStatisticsAggregator
    {
        public decimal Max { get; set; }
        public decimal Min { get; set; }
        public decimal MonthlyTotal { get; set; }
        public int Count { get; set; }
        public decimal Average { get; set; }
        public MonthlyStatisticsAggregator()
        {
            Max = decimal.MinValue;
            Min = decimal.MaxValue;
        }
        public MonthlyStatisticsAggregator Accumulate(MonthlyStatistics m)
        {
            MonthlyTotal += m.Total;
            Count = m.Count;
            Max = Math.Max(Max, m.Max);
            Min = Math.Min(Min, m.Min);
            return this;
        }
        public MonthlyStatisticsAggregator Compute()
        {
            Average = Math.Round(MonthlyTotal / Count, 2);
            return this;
        }
    }

    public record MonthlyStatistics
    {
        public int MonthNumber { get; set; }
        public string Month { get; set; }
        public int Count { get; set; }
        public decimal Average { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public decimal Total { get; set; }
    }
}
