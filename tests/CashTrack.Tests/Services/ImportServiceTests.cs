using CashTrack.Common;
using CashTrack.Data;
using CashTrack.Models.ImportCsvModels;
using CashTrack.Repositories.ExpenseRepository;
using CashTrack.Repositories.ExpenseReviewRepository;
using CashTrack.Repositories.ImportRuleRepository;
using CashTrack.Repositories.IncomeRepository;
using CashTrack.Repositories.IncomeReviewRepository;
using CashTrack.Services.ImportService;
using CashTrack.Tests.Services.Common;
using Microsoft.AspNetCore.Http;
using Shouldly;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CashTrack.Tests.Services
{
    public class ImportServiceTests
    {
        private string FilesFolderPath;
        public ImportServiceTests()
        {
            FilesFolderPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName, "Files");
        }
        [Fact]
        public async Task Importing_From_Bank_File_Works()
        {
            var bytes = FileUtils.GetFileBytes(Path.Combine(FilesFolderPath, "test-bank-import.csv"));
            var request = new ImportModel()
            {
                File = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "test-bank-import", "test-bank-import.csv"),
                FileType = CsvFileType.Bank,
                ReturnUrl = "/"
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var incomeReviewRepo = new IncomeReviewRepository(db);
                var expenseReviewRepo = new ExpenseReviewRepository(db);
                var incomeRepo = new IncomeRepository(db);
                var expenseRepo = new ExpenseRepository(db);
                var rulesRepo = new ImportRulesRepository(db);
                var importService = new ImportService(incomeReviewRepo, expenseRepo, incomeRepo, new TestWebHostEnvironment(), rulesRepo, expenseReviewRepo);

                var result = await importService.ImportTransactions(request);
                result.ShouldBe("Added 6 Expenses And Added 1 Income");

                var verifyExpenses = await expenseReviewRepo.Find(x => true);
                verifyExpenses.Count().ShouldBe(6);
                var categorizedExpense = verifyExpenses.FirstOrDefault(x => x.Notes == "rent");
                categorizedExpense!.SuggestedCategoryId!.Value.ShouldBe(32);
                categorizedExpense!.SuggestedMerchantId!.Value.ShouldBe(3);
                var verifyIncomes = await incomeReviewRepo.Find(x => true);
                verifyIncomes.Count().ShouldBe(1);
                var categorizedIncome = verifyIncomes.FirstOrDefault(x => x.Notes == "tip for guiding");
                categorizedIncome!.SuggestedCategoryId!.Value.ShouldBe(7);
                categorizedIncome!.SuggestedSourceId!.Value.ShouldBe(3);

            }
        }
        [Fact]
        public async Task Importing_From_Credit_Card_File_Works()
        {
            var bytes = FileUtils.GetFileBytes(Path.Combine(FilesFolderPath, "test-credit-import.csv"));
            var request = new ImportModel()
            {
                File = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "test-credit-import", "test-credit-import.csv"),
                FileType = CsvFileType.Credit,
                ReturnUrl = "/"
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var incomeReviewRepo = new IncomeReviewRepository(db);
                var expenseReviewRepo = new ExpenseReviewRepository(db);
                var incomeRepo = new IncomeRepository(db);
                var expenseRepo = new ExpenseRepository(db);
                var rulesRepo = new ImportRulesRepository(db);
                var importService = new ImportService(incomeReviewRepo, expenseRepo, incomeRepo, new TestWebHostEnvironment(), rulesRepo, expenseReviewRepo);

                var result = await importService.ImportTransactions(request);
                result.ShouldBe("Added 6 Expenses And Added 1 Income");

                var verifyExpenses = await expenseReviewRepo.Find(x => true);
                verifyExpenses.Count().ShouldBe(6);
                var categorizedExpense = verifyExpenses.FirstOrDefault(x => x.Notes == "kaiser");
                categorizedExpense!.SuggestedCategoryId!.Value.ShouldBe(11);
                categorizedExpense!.SuggestedMerchantId!.Value.ShouldBe(13);
                var verifyIncomes = await incomeReviewRepo.Find(x => true);
                verifyIncomes.Count().ShouldBe(1);
                var categorizedIncome = verifyIncomes.FirstOrDefault(x => x.Notes == "amazon");
                categorizedIncome!.SuggestedCategoryId!.Value.ShouldBe(9);
                categorizedIncome!.SuggestedSourceId!.Value.ShouldBe(11);

                var verifyNoReimporting = await importService.ImportTransactions(request);
                verifyNoReimporting.ShouldBe("Transactions have already been imported.");
            }
        }
        [Fact]
        public async Task Reimporting_From_Bank_File_Gives_Error()
        {
            var bytes = FileUtils.GetFileBytes(Path.Combine(FilesFolderPath, "test-bank-import.csv"));
            var request = new ImportModel()
            {
                File = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "test-bank-import", "test-bank-import.csv"),
                FileType = CsvFileType.Bank,
                ReturnUrl = "/"
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var incomeReviewRepo = new IncomeReviewRepository(db);
                var expenseReviewRepo = new ExpenseReviewRepository(db);
                var incomeRepo = new IncomeRepository(db);
                var expenseRepo = new ExpenseRepository(db);
                var rulesRepo = new ImportRulesRepository(db);
                var importService = new ImportService(incomeReviewRepo, expenseRepo, incomeRepo, new TestWebHostEnvironment(), rulesRepo, expenseReviewRepo);

                var result = await importService.ImportTransactions(request);
                result.ShouldBe("Added 6 Expenses And Added 1 Income");

                var verifyNoReimporting = await importService.ImportTransactions(request);
                verifyNoReimporting.ShouldBe("Transactions have already been imported.");
            }
        }
        [Fact]
        public async Task Reimporting_From_Credit_Card_File_Gives_Error()
        {
            var bytes = FileUtils.GetFileBytes(Path.Combine(FilesFolderPath, "test-credit-import.csv"));
            var request = new ImportModel()
            {
                File = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "test-credit-import", "test-credit-import.csv"),
                FileType = CsvFileType.Credit,
                ReturnUrl = "/"
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var incomeReviewRepo = new IncomeReviewRepository(db);
                var expenseReviewRepo = new ExpenseReviewRepository(db);
                var incomeRepo = new IncomeRepository(db);
                var expenseRepo = new ExpenseRepository(db);
                var rulesRepo = new ImportRulesRepository(db);
                var importService = new ImportService(incomeReviewRepo, expenseRepo, incomeRepo, new TestWebHostEnvironment(), rulesRepo, expenseReviewRepo);

                var result = await importService.ImportTransactions(request);
                result.ShouldBe("Added 6 Expenses And Added 1 Income");

                var verifyNoReimporting = await importService.ImportTransactions(request);
                verifyNoReimporting.ShouldBe("Transactions have already been imported.");
            }
        }
        [Fact]
        public async Task Empty_Bank_File_Gives_Error()
        {
            var bytes = FileUtils.GetFileBytes(Path.Combine(FilesFolderPath, "empty-test-bank-import.csv"));
            var request = new ImportModel()
            {
                File = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "empty-test-bank-import", "empty-test-bank-import.csv"),
                FileType = CsvFileType.Bank,
                ReturnUrl = "/"
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var incomeReviewRepo = new IncomeReviewRepository(db);
                var expenseReviewRepo = new ExpenseReviewRepository(db);
                var incomeRepo = new IncomeRepository(db);
                var expenseRepo = new ExpenseRepository(db);
                var rulesRepo = new ImportRulesRepository(db);
                var importService = new ImportService(incomeReviewRepo, expenseRepo, incomeRepo, new TestWebHostEnvironment(), rulesRepo, expenseReviewRepo);

                var result = await importService.ImportTransactions(request);
                result.ShouldBe("No transactions imported - is the file empty?");
            }
        }
        [Fact]
        public async Task Empty_Credit_File_Gives_Error()
        {
            var bytes = FileUtils.GetFileBytes(Path.Combine(FilesFolderPath, "empty-test-credit-import.csv"));
            var request = new ImportModel()
            {
                File = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "empty-test-credit-import", "empty-test-credit-import.csv"),
                FileType = CsvFileType.Credit,
                ReturnUrl = "/"
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var incomeReviewRepo = new IncomeReviewRepository(db);
                var expenseReviewRepo = new ExpenseReviewRepository(db);
                var incomeRepo = new IncomeRepository(db);
                var expenseRepo = new ExpenseRepository(db);
                var rulesRepo = new ImportRulesRepository(db);
                var importService = new ImportService(incomeReviewRepo, expenseRepo, incomeRepo, new TestWebHostEnvironment(), rulesRepo, expenseReviewRepo);

                var result = await importService.ImportTransactions(request);
                result.ShouldBe("No transactions imported - is the file empty?");
            }
        }
        [Fact]
        public async Task Wrong_Headers_On_Credit_File_Gives_Error()
        {
            var bytes = FileUtils.GetFileBytes(Path.Combine(FilesFolderPath, "test-credit-import-bad-headers.csv"));
            var request = new ImportModel()
            {
                File = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "test-credit-import-bad-headers", "test-credit-import-bad-headers.csv"),
                FileType = CsvFileType.Credit,
                ReturnUrl = "/"
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var incomeReviewRepo = new IncomeReviewRepository(db);
                var expenseReviewRepo = new ExpenseReviewRepository(db);
                var incomeRepo = new IncomeRepository(db);
                var expenseRepo = new ExpenseRepository(db);
                var rulesRepo = new ImportRulesRepository(db);
                var importService = new ImportService(incomeReviewRepo, expenseRepo, incomeRepo, new TestWebHostEnvironment(), rulesRepo, expenseReviewRepo);

                var result = await importService.ImportTransactions(request);
                result.ShouldBe("Please inspect the csv file for the correct headers.");
            }
        }
        [Fact]
        public async Task Wrong_Headers_On_Bank_File_Gives_Error()
        {
            var bytes = FileUtils.GetFileBytes(Path.Combine(FilesFolderPath, "test-bank-import-bad-headers.csv"));
            var request = new ImportModel()
            {
                File = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "test-bank-import-bad-headers", "test-bank-import-bad-headers.csv"),
                FileType = CsvFileType.Bank,
                ReturnUrl = "/"
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var incomeReviewRepo = new IncomeReviewRepository(db);
                var expenseReviewRepo = new ExpenseReviewRepository(db);
                var incomeRepo = new IncomeRepository(db);
                var expenseRepo = new ExpenseRepository(db);
                var rulesRepo = new ImportRulesRepository(db);
                var importService = new ImportService(incomeReviewRepo, expenseRepo, incomeRepo, new TestWebHostEnvironment(), rulesRepo, expenseReviewRepo);

                var result = await importService.ImportTransactions(request);
                result.ShouldBe("Please inspect the csv file for the correct headers.");
            }
        }
        private ImportService GetImportService(AppDbContext db)
        {
            var incomeReviewRepo = new IncomeReviewRepository(db);
            var expenseReviewRepo = new ExpenseReviewRepository(db);
            var incomeRepo = new IncomeRepository(db);
            var expenseRepo = new ExpenseRepository(db);
            var rulesRepo = new ImportRulesRepository(db);
            return new ImportService(incomeReviewRepo, expenseRepo, incomeRepo, new TestWebHostEnvironment(), rulesRepo, expenseReviewRepo);
        }
    }
}
