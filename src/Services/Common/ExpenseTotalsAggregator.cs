using CashTrack.Data.Entities;
using System;

namespace CashTrack.Services.Common
{
    public record ExpenseTotalsAggregator
    {
        public decimal TotalSpentThisMonth { get; set; }
        public decimal TotalSpentThisYear { get; set; }
        public decimal TotalSpentAllTime { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public decimal Average { get; set; }
        public int Count { get; set; }
        public ExpenseTotalsAggregator()
        {
            this.TotalSpentThisMonth = 0;
            this.TotalSpentThisYear = 0;
            this.TotalSpentAllTime = 0;
            this.Min = decimal.MaxValue;
            this.Max = decimal.MinValue;
            this.Average = 0;
            this.Count = 0;
        }
        public ExpenseTotalsAggregator Accumulate(ExpenseEntity e)
        {
            if (e.Date.Month == DateTimeOffset.UtcNow.Month && e.Date.Year == DateTimeOffset.UtcNow.Year)
            {
                TotalSpentThisMonth += e.Amount;
            }
            if (e.Date.Year == DateTimeOffset.UtcNow.Year)
            {
                TotalSpentThisYear += e.Amount;
            }
            TotalSpentAllTime += e.Amount;
            Min = Math.Min(Min, e.Amount);
            Max = Math.Max(Max, e.Amount);
            Count++;
            return this;
        }
        public ExpenseTotals Compute()
        {
            return new ExpenseTotals()
            {
                TotalSpentThisMonth = this.TotalSpentThisMonth,
                TotalSpentThisYear = this.TotalSpentThisYear,
                TotalSpentAllTime = this.TotalSpentAllTime,
                Average = Math.Round(TotalSpentAllTime / Count, 2),
                Count = this.Count,
                Min = this.Min,
                Max = this.Max,
        };
        }
    }
    public record ExpenseTotals
    {
        public decimal TotalSpentThisMonth { get; set; }
        public decimal TotalSpentThisYear { get; set; }
        public decimal TotalSpentAllTime { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public decimal Average { get; set; }
        public int Count { get; set; }
    }
}
