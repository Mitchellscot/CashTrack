using CashTrack.Data.Entities;
using System;

namespace CashTrack.Services.Common
{
    public record ExpenseTotalsAggregator
    {
        public decimal TotalSpentThisMonth { get; set; }
        public decimal TotalSpentThisYear { get; set; }
        public decimal TotalSpentAllTime { get; set; }
        public ExpenseTotalsAggregator()
        {
            this.TotalSpentThisMonth = 0;
            this.TotalSpentThisYear = 0;
            this.TotalSpentAllTime = 0;
        }
        public ExpenseTotalsAggregator Accumulate(Expenses e)
        {
            if (e.date.Month == DateTimeOffset.UtcNow.Month && e.date.Year == DateTimeOffset.UtcNow.Year)
            {
                TotalSpentThisMonth += e.amount;
            }
            if (e.date.Year == DateTimeOffset.UtcNow.Year)
            {
                TotalSpentThisYear += e.amount;
            }
            TotalSpentAllTime += e.amount;
            return this;
        }
        public ExpenseTotals Compute()
        {
            return new ExpenseTotals()
            {
                TotalSpentThisMonth = this.TotalSpentThisMonth,
                TotalSpentThisYear = this.TotalSpentThisYear,
                TotalSpentAllTime = this.TotalSpentAllTime
            };
        }
    }
    public record ExpenseTotals
    {
        public decimal TotalSpentThisMonth { get; set; }
        public decimal TotalSpentThisYear { get; set; }
        public decimal TotalSpentAllTime { get; set; }
    }
}
