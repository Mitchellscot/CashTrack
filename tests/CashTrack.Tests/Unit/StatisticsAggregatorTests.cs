using CashTrack.Data.Entities;
using CashTrack.Data.Entities.Common;
using CashTrack.Models.ExpenseModels;
using CashTrack.Repositories.ExpenseRepository;
using CashTrack.Services.Common;
using CashTrack.Tests.Services.Common;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CashTrack.Tests.Unit
{
    public class StatisticsAggregatorTests
    {
        private readonly ExpenseEntity[] _expenses;

        public StatisticsAggregatorTests()
        {
            var db = new AppDbContextFactory().CreateDbContext();
            var expenseRepo = new ExpenseRepository(db);
            _expenses = expenseRepo.Find(x => true).Result;
        }
        [Fact]
        public void Annual_Statistics_Can_Be_Generated()
        {
            var result = AggregateUtilities<ExpenseEntity>.GetAnnualStatistics(_expenses);
            //only one year available for test data
            var annualResult = result.FirstOrDefault();
            annualResult!.Total.ShouldBe(31543.403M);
            annualResult.Year.ShouldBe(2012);
            annualResult.Count.ShouldBe(462);
            annualResult.Min.ShouldBe(1);
            annualResult.Max.ShouldBe(1600);
            annualResult.Average.ShouldBe(68.28m);
        }
        [Fact]
        public void Monthly_Statistics_Can_Be_Generated()
        {
            var result = AggregateUtilities<ExpenseEntity>.GetMonthlyStatistics(_expenses);
            foreach (var month in result)
            {
                month.Month.ShouldNotBeEmpty();
                month.MonthNumber.ShouldBeGreaterThan(0);
                month.Count.ShouldBeGreaterThan(0);
                month.Average.ShouldBeGreaterThan(0);
                month.Min.ShouldBeGreaterThan(0);
                month.Max.ShouldBeGreaterThan(0);
                month.Total.ShouldBeGreaterThan(0);
            }
        }
        [Fact]
        public void Statistics_Aggregator_Works()
        {
            var results = _expenses.GroupBy(e => e.Date.Year)
                            .Select(g =>
                            {
                                return g.Aggregate(new StatisticsAggregator<ExpenseEntity>(),
                                    (acc, e) => acc.Accumulate(e),
                                    acc => acc.Compute());
                            }).ToList();
            var result = results.FirstOrDefault();
            result!.Count.ShouldBe(462);
            result.Max.ShouldBe(1600);
            result.Min.ShouldBe(1);
            result.Total.ShouldBe(31543.403M);
            result.Average.ShouldBe(68.28m);
        }
    }
}
