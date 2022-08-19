using CashTrack.Data.Entities;
using CashTrack.Models.ExpenseModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Pages.Expenses.Categories;
using CashTrack.Repositories.SubCategoriesRepository;
using CashTrack.Services.Common;
using CashTrack.Tests.Services.Common;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CashTrack.Tests.Unit
{
    public class SubCategoryListItemAggregatorTests
    {
        private readonly SubCategoryEntity[] _categories;

        public SubCategoryListItemAggregatorTests()
        {
            var db = new AppDbContextFactory().CreateDbContext();
            var subCategoryRepo = new SubCategoryRepository(db);
            _categories = subCategoryRepo.FindWithPaginationIncludeExpenses(x => true, 1, 100).Result;
        }
        [Fact]
        public void Sub_Category_Aggregator_Works()
        {
            var expenses = _categories.SelectMany(x => x.Expenses).ToArray();
            var results = expenses.GroupBy(e => e.CategoryId).Select(g =>
            {
                return g.Aggregate(new SubCategoryListItemAggregator(g.Key!.Value, _categories), (acc, e) => acc.Accumulate(e), acc => acc.Compute());
            }).ToArray();
            foreach (var result in results)
            {
                result.Purchases.ShouldBeGreaterThan(0);
                result.LastPurchase.ShouldBeGreaterThan(new DateTime(2011, 12, 31));
                result.SubcategoryId.ShouldBeGreaterThan(0);
                result.Amount.ShouldBeGreaterThan(0);
                result.Category.ShouldNotBeNull();
            }
        }
    }
}
