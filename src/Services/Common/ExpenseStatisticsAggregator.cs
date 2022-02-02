﻿using CashTrack.Data.Entities;
using System;

namespace CashTrack.Services.Common
{
    public class ExpenseStatisticsAggregator
    {
        public decimal Max { get; set; }
        public decimal Min { get; set; }
        public decimal Total { get; set; }
        public int Count { get; set; }
        public decimal Average { get; set; }
        public ExpenseStatisticsAggregator()
        {
            Max = decimal.MinValue;
            Min = decimal.MaxValue;
        }
        public ExpenseStatisticsAggregator Accumulate(Expenses e)
        {
            Total += e.amount;
            Count++;
            Max = Math.Max(Max, e.amount);
            Min = Math.Min(Min, e.amount);
            return this;
        }
        public ExpenseStatisticsAggregator Compute()
        {
            Average = Math.Round(Total / Count, 2);
            return this;
        }
    }
    public record AnnualExpenseStatistics
    {
        public int Year { get; set; }
        public int Count { get; set; }
        public decimal Average { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public decimal Total { get; set; }
    }
}
