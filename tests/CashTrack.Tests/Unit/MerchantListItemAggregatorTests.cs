using CashTrack.Models.MerchantModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Repositories.ExpenseRepository;
using CashTrack.Repositories.MerchantRepository;
using CashTrack.Repositories.SubCategoriesRepository;
using CashTrack.Services.Common;
using CashTrack.Tests.Services.Common;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CashTrack.Tests.Unit
{
    public class MerchantListItemAggregatorTests
    {
        private readonly ExpenseRepository _expenseRepo;
        private readonly MerchantRepository _merchantRepo;
        private readonly SubCategoryRepository _categoryRepo;

        public MerchantListItemAggregatorTests()
        {
            var db = new AppDbContextFactory().CreateDbContext();
            _expenseRepo = new ExpenseRepository(db);
            _merchantRepo = new MerchantRepository(db);
            _categoryRepo = new SubCategoryRepository(db);
        }
        [Fact]
        public async Task Merchant_List_Item_Aggregator_Works()
        {
            var expenses = await _expenseRepo.Find(x => true);
            var merchants = await _merchantRepo.Find(x => true);
            var categories = await _categoryRepo.Find(x => true);
            var results = expenses.GroupBy(e => e.MerchantId)
                        .Select(g =>
                        {
                            return g.Aggregate(new MerchantListItemAggregator(g.Key, merchants, categories), (acc, e) => acc.Accumulate(e), acc => acc.Compute());
                        }).Where(x => x.MerchantId > 0).ToList();

            foreach (var result in results)
            {
                result.LastPurchase.ShouldBeGreaterThan(new System.DateTime(2011, 12, 31));
                result.Amount.ShouldBeGreaterThan(0);
                result.Categories.Count.ShouldBeGreaterThan(0);
                result.Purchases.ShouldBeGreaterThan(0);
                result.MostUsedCategoryId.ShouldBeGreaterThan(0);
                result.MostUsedCategory.ShouldNotBeNullOrWhiteSpace();
            }
        }
    }
}
