using CashTrack.Models.ImportRuleModels;
using CashTrack.Repositories.ImportRuleRepository;
using CashTrack.Repositories.IncomeCategoryRepository;
using CashTrack.Repositories.IncomeSourceRepository;
using CashTrack.Repositories.MerchantRepository;
using CashTrack.Repositories.SubCategoriesRepository;
using CashTrack.Services.ImportRulesService;
using CashTrack.Tests.Services.Common;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace CashTrack.Tests.Services
{
    public class ImportRulesServiceTests
    {
        private readonly ImportRulesService _service;

        public ImportRulesServiceTests()
        {
            var db = new AppDbContextFactory().CreateDbContext();
            var repo = new ImportRulesRepository(db);
            var merchantRepo = new MerchantRepository(db);
            var sourceRepo = new IncomeSourceRepository(db);
            var subCategoryRepo = new SubCategoryRepository(db);
            var incomeCategoryRepo = new IncomeCategoryRepository(db);
            _service = new ImportRulesService(repo, merchantRepo, sourceRepo, subCategoryRepo, incomeCategoryRepo);
        }
        [Fact]
        public async Task Get_All_ImportRules()
        {
            var result = await _service.GetImportRulesAsync(new ImportRuleRequest());
            result.TotalCount.ShouldBe(6);
        }
        //make test for create
        //make a test for update
        //make a test for delete
    }
}

