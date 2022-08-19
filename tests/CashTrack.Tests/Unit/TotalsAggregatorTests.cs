using CashTrack.Data;
using CashTrack.Data.Entities;
using CashTrack.Models.IncomeModels;
using CashTrack.Pages.Import;
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
    public class TotalsAggregatorTests
    {
        private List<ExpenseEntity> _expenses;

        public TotalsAggregatorTests()
        {
            var db = new AppDbContextFactory().CreateDbContext();
            var expenseRepo = new ExpenseRepository(db);
            _expenses = expenseRepo.Find(x => true).Result.ToList();
        }

        [Fact]
        public void Totals_Aggregator_Works()
        {
            var newExpense = new ExpenseEntity() { Amount = 1, Date = DateTime.Today, CategoryId = 1 };
            _expenses.Add(newExpense);
            var result = _expenses.Aggregate(new TotalsAggregator<ExpenseEntity>(),
                (acc, e) => acc.Accumulate(e),
                acc => acc.Compute());
            result.Average.ShouldBe(68.13m);
            result.Min.ShouldBe(1);
            result.Max.ShouldBe(1600);
            result.Count.ShouldBe(463);
            result.TotalSpentAllTime.ShouldBe(31544.403M);
            result.TotalSpentThisMonth.ShouldBe(1);
            result.TotalSpentThisYear.ShouldBe(1);
        }
    }
}
