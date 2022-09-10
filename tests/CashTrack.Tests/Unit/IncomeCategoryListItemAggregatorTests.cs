using CashTrack.Data.Entities;
using CashTrack.Repositories.IncomeCategoryRepository;
using CashTrack.Services.Common;
using CashTrack.Tests.Services.Common;
using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace CashTrack.Tests.Unit
{
    public class IncomeCategoryListItemAggregatorTests
    {
        private readonly IncomeCategoryEntity[] _categories;

        public IncomeCategoryListItemAggregatorTests()
        {
            var db = new AppDbContextFactory().CreateDbContext();
            var IncomeCategoryRepo = new IncomeCategoryRepository(db);
            _categories = IncomeCategoryRepo.FindWithPaginationIncludeIncome(x => true, 1, 100).Result;
        }
        [Fact]
        public void Income_Category_Aggregator_Works()
        {
            var income = _categories.SelectMany(x => x.Income).ToArray();
            var results = income.GroupBy(e => e.CategoryId).Select(g =>
            {
                return g.Aggregate(new IncomeCategoryListItemAggregator(g.Key!.Value, _categories), (acc, e) => acc.Accumulate(e), acc => acc.Compute());
            }).ToArray();
            foreach (var result in results)
            {
                result.Payments.ShouldBeGreaterThan(0);
                result.LastPayment.ShouldBeGreaterThan(new DateTime(2011, 12, 31));
                result.IncomecategoryId.ShouldBeGreaterThan(0);
                result.Amount.ShouldBeGreaterThan(0);
                result.Category.ShouldNotBeNull();
            }
        }
    }
}
