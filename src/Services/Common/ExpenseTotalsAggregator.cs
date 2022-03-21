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
