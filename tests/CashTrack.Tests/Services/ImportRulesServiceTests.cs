using CashTrack.Common.Exceptions;
using CashTrack.Data;
using CashTrack.Models.Common;
using CashTrack.Models.ImportRuleModels;
using CashTrack.Models.MerchantModels;
using CashTrack.Repositories.ImportRuleRepository;
using CashTrack.Repositories.IncomeCategoryRepository;
using CashTrack.Repositories.IncomeSourceRepository;
using CashTrack.Repositories.MerchantRepository;
using CashTrack.Repositories.SubCategoriesRepository;
using CashTrack.Services.ImportRulesService;
using CashTrack.Tests.Services.Common;
using Shouldly;
using System.Linq;
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
        [Fact]
        public async Task Create_Import_Rules()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var repo = new ImportRulesRepository(db);
                var merchantRepo = new MerchantRepository(db);
                var sourceRepo = new IncomeSourceRepository(db);
                var subCategoryRepo = new SubCategoryRepository(db);
                var incomeCategoryRepo = new IncomeCategoryRepository(db);
                var service = new ImportRulesService(repo, merchantRepo, sourceRepo, subCategoryRepo, incomeCategoryRepo);

                var newRule = new AddEditImportRule()
                {
                    FileType = (int)CsvFileType.Bank,
                    RuleType = (int)RuleType.Assignment,
                    TransactionType = (int)TransactionType.Expense,
                    Rule = "blah blah",
                    MerchantSourceId = 1,
                    CategoryId = 1
                };
                var result = await service.CreateImportRuleAsync(newRule);
                var verfiyResult = await repo.FindById(result);
                verfiyResult.Rule.ShouldBe("blah blah");
            }
        }
        [Fact]
        public async Task Update_Import_Rule()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var repo = new ImportRulesRepository(db);
                var merchantRepo = new MerchantRepository(db);
                var sourceRepo = new IncomeSourceRepository(db);
                var subCategoryRepo = new SubCategoryRepository(db);
                var incomeCategoryRepo = new IncomeCategoryRepository(db);
                var service = new ImportRulesService(repo, merchantRepo, sourceRepo, subCategoryRepo, incomeCategoryRepo);

                var newRule = new AddEditImportRule()
                {
                    Id = 1,
                    FileType = (int)CsvFileType.Bank,
                    RuleType = (int)RuleType.Assignment,
                    TransactionType = (int)TransactionType.Expense,
                    Rule = "blah blah",
                    MerchantSourceId = 1,
                    CategoryId = 1
                };
                var result = await service.UpdateImportRuleAsync(newRule);
                var verfiyResult = await repo.FindById(result);
                verfiyResult.Rule.ShouldBe("blah blah");
            }
        }
        [Fact]
        public async Task Update_Filter_Rule_Leaves_Category_Null()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var repo = new ImportRulesRepository(db);
                var merchantRepo = new MerchantRepository(db);
                var sourceRepo = new IncomeSourceRepository(db);
                var subCategoryRepo = new SubCategoryRepository(db);
                var incomeCategoryRepo = new IncomeCategoryRepository(db);
                var service = new ImportRulesService(repo, merchantRepo, sourceRepo, subCategoryRepo, incomeCategoryRepo);

                var newRule = new AddEditImportRule()
                {
                    Id = 1,
                    FileType = (int)CsvFileType.Bank,
                    RuleType = (int)RuleType.Filter,
                    TransactionType = (int)TransactionType.Expense,
                    Rule = "blah blah",
                    MerchantSourceId = 1,
                    CategoryId = 1
                };
                var result = await service.UpdateImportRuleAsync(newRule);
                var verfiyResult = await repo.FindById(result);
                verfiyResult.Rule.ShouldBe("blah blah");
                verfiyResult.MerchantSourceId.ShouldBe(null);
                verfiyResult.CategoryId.ShouldBe(null);
            }
        }
        [Fact]
        public async Task Creating_Filter_Rules_Leaves_Category_Null()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var repo = new ImportRulesRepository(db);
                var merchantRepo = new MerchantRepository(db);
                var sourceRepo = new IncomeSourceRepository(db);
                var subCategoryRepo = new SubCategoryRepository(db);
                var incomeCategoryRepo = new IncomeCategoryRepository(db);
                var service = new ImportRulesService(repo, merchantRepo, sourceRepo, subCategoryRepo, incomeCategoryRepo);

                var newRule = new AddEditImportRule()
                {
                    FileType = (int)CsvFileType.Bank,
                    RuleType = (int)RuleType.Filter,
                    TransactionType = (int)TransactionType.Expense,
                    Rule = "blah blah",
                    MerchantSourceId = 1,
                    CategoryId = 1
                };
                var result = await service.CreateImportRuleAsync(newRule);
                var verfiyResult = await repo.FindById(result);
                verfiyResult.Rule.ShouldBe("blah blah");
                verfiyResult.MerchantSourceId.ShouldBe(null);
                verfiyResult.CategoryId.ShouldBe(null);
            }
        }
        [Fact]
        public async Task Delete_Rule_Throws_With_Invalid_Id()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetService(db);
                await Task.Run(() => Should.Throw<ImportRuleNotFoundException>(async () => await service.DeleteImportRuleAsync(int.MaxValue)));
            }
        }
        [Fact]
        public async Task Update_Rule_Throws_With_Invalid_Id()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetService(db);
                var rule = new AddEditImportRule()
                { Id = int.MaxValue, CategoryId = 1, TransactionType = 1, RuleType = 1, Rule = "adsfasdf" };
                await Task.Run(() => Should.Throw<ImportRuleNotFoundException>(async () => await service.UpdateImportRuleAsync(rule)));
            }
        }
        [Fact]
        public async Task Get_Import_Rules_By_Rule_Type()
        {
            var request = new ImportRuleRequest()
            {
                OrderBy = ImportRuleOrderBy.RuleType,
                Reversed = false,
            };
            var result = await _service.GetImportRulesAsync(request);
            result.ListItems.FirstOrDefault()!.RuleType.ShouldBe("Assignment");
        }
        [Fact]
        public async Task Get_Import_Rules_By_Rule_Type_Reversed()
        {
            var request = new ImportRuleRequest()
            {
                OrderBy = ImportRuleOrderBy.RuleType,
                Reversed = true,
            };
            var result = await _service.GetImportRulesAsync(request);
            result.ListItems.FirstOrDefault()!.RuleType.ShouldBe("Filter");
        }
        [Fact]
        public async Task Get_Import_Rules_By_File_Type()
        {
            var request = new ImportRuleRequest()
            {
                OrderBy = ImportRuleOrderBy.FileType,
                Reversed = false,
            };
            var result = await _service.GetImportRulesAsync(request);
            result.ListItems.FirstOrDefault()!.FileType.ShouldBe("Bank");
        }
        [Fact]
        public async Task Get_Import_Rules_By_File_Type_Reversed()
        {
            var request = new ImportRuleRequest()
            {
                OrderBy = ImportRuleOrderBy.FileType,
                Reversed = true,
            };
            var result = await _service.GetImportRulesAsync(request);
            result.ListItems.FirstOrDefault()!.FileType.ShouldBe("Credit");
        }
        [Fact]
        public async Task Get_Import_Rules_By_Transaction_Type()
        {
            var request = new ImportRuleRequest()
            {
                OrderBy = ImportRuleOrderBy.TransactionType,
                Reversed = false,
            };
            var result = await _service.GetImportRulesAsync(request);
            result.ListItems.FirstOrDefault()!.TransactionType.ShouldBe("Expense");
        }
        [Fact]
        public async Task Get_Import_Rules_By_Transaction_Type_Reversed()
        {
            var request = new ImportRuleRequest()
            {
                OrderBy = ImportRuleOrderBy.TransactionType,
                Reversed = true,
            };
            var result = await _service.GetImportRulesAsync(request);
            result.ListItems.FirstOrDefault()!.TransactionType.ShouldBe("Income");
        }
        [Fact]
        public async Task Get_Import_Rules_By_Rule()
        {
            var request = new ImportRuleRequest()
            {
                OrderBy = ImportRuleOrderBy.Rule,
                Reversed = false,
            };
            var result = await _service.GetImportRulesAsync(request);
            result.ListItems.FirstOrDefault()!.Rule.ShouldBe("amazon");
        }
        [Fact]
        public async Task Get_Import_Rules_By_Rule_Reversed()
        {
            var request = new ImportRuleRequest()
            {
                OrderBy = ImportRuleOrderBy.Rule,
                Reversed = true,
            };
            var result = await _service.GetImportRulesAsync(request);
            result.ListItems.FirstOrDefault()!.Rule.ShouldBe("tip");
        }
        [Fact]
        public async Task Get_Import_Rules_By_Category()
        {
            var request = new ImportRuleRequest()
            {
                OrderBy = ImportRuleOrderBy.Category,
                Reversed = false,
            };
            var result = await _service.GetImportRulesAsync(request);
            result.ListItems.FirstOrDefault()!.Category.ShouldBe(null);
        }
        [Fact]
        public async Task Get_Import_Rules_By_Category_Reversed()
        {
            var request = new ImportRuleRequest()
            {
                OrderBy = ImportRuleOrderBy.Category,
                Reversed = true,
            };
            var result = await _service.GetImportRulesAsync(request);
            result.ListItems.FirstOrDefault()!.Category.ShouldBe("Tip");
        }
        private ImportRulesService GetService(AppDbContext db)
        {
            var repo = new ImportRulesRepository(db);
            var merchantRepo = new MerchantRepository(db);
            var sourceRepo = new IncomeSourceRepository(db);
            var subCategoryRepo = new SubCategoryRepository(db);
            var incomeCategoryRepo = new IncomeCategoryRepository(db);
            return new ImportRulesService(repo, merchantRepo, sourceRepo, subCategoryRepo, incomeCategoryRepo);
        }
    }
}

