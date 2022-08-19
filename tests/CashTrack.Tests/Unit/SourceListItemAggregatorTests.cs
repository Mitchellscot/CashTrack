using CashTrack.Repositories.IncomeCategoryRepository;
using CashTrack.Repositories.IncomeRepository;
using CashTrack.Repositories.IncomeSourceRepository;
using CashTrack.Services.Common;
using CashTrack.Tests.Services.Common;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CashTrack.Tests.Unit
{

    public class SourceListItemAggregatorTests
    {
        private readonly IncomeRepository _incomeRepo;
        private readonly IncomeSourceRepository _sourceRepo;
        private readonly IncomeCategoryRepository _categoryRepo;

        public SourceListItemAggregatorTests()
        {
            var db = new AppDbContextFactory().CreateDbContext();
            _incomeRepo = new IncomeRepository(db);
            _sourceRepo = new IncomeSourceRepository(db);
            _categoryRepo = new IncomeCategoryRepository(db);
        }
        [Fact]
        public async Task Source_List_Item_Aggregator_Works()
        {
            var expenses = await _incomeRepo.Find(x => true);
            var sources = await _sourceRepo.Find(x => true);
            var categories = await _categoryRepo.Find(x => true);
            var results = expenses.GroupBy(e => e.SourceId)
                        .Select(g =>
                        {
                            return g.Aggregate(new SourceListItemAggregator(g.Key, categories, sources), (acc, e) => acc.Accumulate(e), acc => acc.Compute());
                        }).Where(x => x.SourceId > 0).ToList();

            foreach (var result in results)
            {
                result.LastPayment.ShouldBeGreaterThan(new System.DateTime(2011, 12, 31));
                result.Amount.ShouldBeGreaterThan(0);
                result.Categories.Count.ShouldBeGreaterThan(0);
                result.Payments.ShouldBeGreaterThan(0);
                result.MostUsedCategoryId.ShouldBeGreaterThan(0);
                result.MostUsedCategory.ShouldNotBeNullOrWhiteSpace();
            }
        }
    }


}
