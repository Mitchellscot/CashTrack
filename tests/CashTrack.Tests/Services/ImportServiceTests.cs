using CashTrack.Common;
using CashTrack.Common.Extensions;
using CashTrack.Data;
using CashTrack.Data.Entities;
using CashTrack.Models.ImportCsvModels;
using CashTrack.Models.ImportRuleModels;
using CashTrack.Repositories.ExpenseRepository;
using CashTrack.Repositories.ExpenseReviewRepository;
using CashTrack.Repositories.ImportRepository;
using CashTrack.Repositories.ImportRuleRepository;
using CashTrack.Repositories.IncomeRepository;
using CashTrack.Repositories.IncomeReviewRepository;
using CashTrack.Services.ImportService;
using CashTrack.Tests.Services.Common;
using Microsoft.AspNetCore.Http;
using Shouldly;
using System;
using System.Collections.Generic;
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
            var bytes = FileUtilities.GetFileBytes(Path.Combine(FilesFolderPath, "test-bank-import.csv"));
            var request = new ImportModel()
            {
                File = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "test-bank-import", "test-bank-import.csv"),
                FileType = "Bank",
                ReturnUrl = "/"
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var incomeReviewRepo = new IncomeReviewRepository(db);
                var expenseReviewRepo = new ExpenseReviewRepository(db);
                var incomeRepo = new IncomeRepository(db);
                var expenseRepo = new ExpenseRepository(db);
                var rulesRepo = new ImportRulesRepository(db);
                var profileRepo = new ImportProfileRepository(db);
                var importService = new ImportService(incomeReviewRepo, expenseRepo, incomeRepo, new TestWebHostEnvironment(), rulesRepo, expenseReviewRepo, profileRepo);

                var result = await importService.ImportTransactions(request);
                result.ShouldBe("Added 6 Expenses And Added 1 Income");

                var verifyExpenses = await expenseReviewRepo.Find(x => true);
                verifyExpenses.Count().ShouldBe(6);
                var categorizedExpense = verifyExpenses.FirstOrDefault(x => x.Notes.IsEqualTo("rent"));
                categorizedExpense!.SuggestedCategoryId!.Value.ShouldBe(32);
                categorizedExpense!.SuggestedMerchantId!.Value.ShouldBe(3);
                var verifyIncomes = await incomeReviewRepo.Find(x => true);
                verifyIncomes.Count().ShouldBe(1);
                var categorizedIncome = verifyIncomes.FirstOrDefault(x => x.Notes.IsEqualTo("tip for guiding"));
                categorizedIncome!.SuggestedCategoryId!.Value.ShouldBe(7);
                categorizedIncome!.SuggestedSourceId!.Value.ShouldBe(3);

            }
        }
        [Fact]
        public async Task Importing_From_Credit_Card_File_Works()
        {
            var bytes = FileUtilities.GetFileBytes(Path.Combine(FilesFolderPath, "test-credit-import.csv"));
            var request = new ImportModel()
            {
                File = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "test-credit-import", "test-credit-import.csv"),
                FileType = "Credit",
                ReturnUrl = "/"
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var incomeReviewRepo = new IncomeReviewRepository(db);
                var expenseReviewRepo = new ExpenseReviewRepository(db);
                var incomeRepo = new IncomeRepository(db);
                var expenseRepo = new ExpenseRepository(db);
                var rulesRepo = new ImportRulesRepository(db);
                var profileRepo = new ImportProfileRepository(db);
                var importService = new ImportService(incomeReviewRepo, expenseRepo, incomeRepo, new TestWebHostEnvironment(), rulesRepo, expenseReviewRepo, profileRepo);

                var result = await importService.ImportTransactions(request);
                result.ShouldBe("Added 6 Expenses And Added 1 Income");

                var verifyExpenses = await expenseReviewRepo.Find(x => true);
                verifyExpenses.Count().ShouldBe(6);
                var categorizedExpense = verifyExpenses.FirstOrDefault(x => x.Notes.IsEqualTo("kaiser"));
                categorizedExpense!.SuggestedCategoryId!.Value.ShouldBe(11);
                categorizedExpense!.SuggestedMerchantId!.Value.ShouldBe(13);
                var verifyIncomes = await incomeReviewRepo.Find(x => true);
                verifyIncomes.Count().ShouldBe(1);
                var categorizedIncome = verifyIncomes.FirstOrDefault(x => x.Notes.IsEqualTo("amazon"));
                categorizedIncome!.SuggestedCategoryId!.Value.ShouldBe(9);
                categorizedIncome!.SuggestedSourceId!.Value.ShouldBe(11);

                var verifyNoReimporting = await importService.ImportTransactions(request);
                verifyNoReimporting.ShouldBe("Transactions have already been imported.");
            }
        }
        [Fact]
        public async Task Reimporting_From_Bank_File_Gives_Error()
        {
            var bytes = FileUtilities.GetFileBytes(Path.Combine(FilesFolderPath, "test-bank-import.csv"));
            var request = new ImportModel()
            {
                File = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "test-bank-import", "test-bank-import.csv"),
                FileType = "Bank",
                ReturnUrl = "/"
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var incomeReviewRepo = new IncomeReviewRepository(db);
                var expenseReviewRepo = new ExpenseReviewRepository(db);
                var incomeRepo = new IncomeRepository(db);
                var expenseRepo = new ExpenseRepository(db);
                var rulesRepo = new ImportRulesRepository(db);
                var profileRepo = new ImportProfileRepository(db);
                var importService = new ImportService(incomeReviewRepo, expenseRepo, incomeRepo, new TestWebHostEnvironment(), rulesRepo, expenseReviewRepo, profileRepo);

                var result = await importService.ImportTransactions(request);
                result.ShouldBe("Added 6 Expenses And Added 1 Income");

                var verifyNoReimporting = await importService.ImportTransactions(request);
                verifyNoReimporting.ShouldBe("Transactions have already been imported.");
            }
        }
        [Fact]
        public async Task Reimporting_From_Credit_Card_File_Gives_Error()
        {
            var bytes = FileUtilities.GetFileBytes(Path.Combine(FilesFolderPath, "test-credit-import.csv"));
            var request = new ImportModel()
            {
                File = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "test-credit-import", "test-credit-import.csv"),
                FileType = "Credit",
                ReturnUrl = "/"
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var incomeReviewRepo = new IncomeReviewRepository(db);
                var expenseReviewRepo = new ExpenseReviewRepository(db);
                var incomeRepo = new IncomeRepository(db);
                var expenseRepo = new ExpenseRepository(db);
                var rulesRepo = new ImportRulesRepository(db);
                var profileRepo = new ImportProfileRepository(db);
                var importService = new ImportService(incomeReviewRepo, expenseRepo, incomeRepo, new TestWebHostEnvironment(), rulesRepo, expenseReviewRepo, profileRepo);

                var result = await importService.ImportTransactions(request);
                result.ShouldBe("Added 6 Expenses And Added 1 Income");

                var verifyNoReimporting = await importService.ImportTransactions(request);
                verifyNoReimporting.ShouldBe("Transactions have already been imported.");
            }
        }
        [Fact]
        public async Task Empty_Bank_File_Gives_Error()
        {
            var bytes = FileUtilities.GetFileBytes(Path.Combine(FilesFolderPath, "empty-test-bank-import.csv"));
            var request = new ImportModel()
            {
                File = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "empty-test-bank-import", "empty-test-bank-import.csv"),
                FileType = "Bank",
                ReturnUrl = "/"
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var incomeReviewRepo = new IncomeReviewRepository(db);
                var expenseReviewRepo = new ExpenseReviewRepository(db);
                var incomeRepo = new IncomeRepository(db);
                var expenseRepo = new ExpenseRepository(db);
                var rulesRepo = new ImportRulesRepository(db);
                var profileRepo = new ImportProfileRepository(db);
                var importService = new ImportService(incomeReviewRepo, expenseRepo, incomeRepo, new TestWebHostEnvironment(), rulesRepo, expenseReviewRepo, profileRepo);

                var result = await importService.ImportTransactions(request);
                result.ShouldBe("No transactions imported");
            }
        }
        [Fact]
        public async Task Empty_Credit_File_Gives_Error()
        {
            var bytes = FileUtilities.GetFileBytes(Path.Combine(FilesFolderPath, "empty-test-credit-import.csv"));
            var request = new ImportModel()
            {
                File = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "empty-test-credit-import", "empty-test-credit-import.csv"),
                FileType = "Credit",
                ReturnUrl = "/"
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var incomeReviewRepo = new IncomeReviewRepository(db);
                var expenseReviewRepo = new ExpenseReviewRepository(db);
                var incomeRepo = new IncomeRepository(db);
                var expenseRepo = new ExpenseRepository(db);
                var rulesRepo = new ImportRulesRepository(db);
                var profileRepo = new ImportProfileRepository(db);
                var importService = new ImportService(incomeReviewRepo, expenseRepo, incomeRepo, new TestWebHostEnvironment(), rulesRepo, expenseReviewRepo, profileRepo);

                var result = await importService.ImportTransactions(request);
                result.ShouldBe("No transactions imported");
            }
        }
        [Fact]
        public async Task Wrong_Headers_On_Credit_File_Gives_Error()
        {
            var bytes = FileUtilities.GetFileBytes(Path.Combine(FilesFolderPath, "test-credit-import-bad-headers.csv"));
            var request = new ImportModel()
            {
                File = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "test-credit-import-bad-headers", "test-credit-import-bad-headers.csv"),
                FileType = "Credit",
                ReturnUrl = "/"
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var incomeReviewRepo = new IncomeReviewRepository(db);
                var expenseReviewRepo = new ExpenseReviewRepository(db);
                var incomeRepo = new IncomeRepository(db);
                var expenseRepo = new ExpenseRepository(db);
                var rulesRepo = new ImportRulesRepository(db);
                var profileRepo = new ImportProfileRepository(db);
                var importService = new ImportService(incomeReviewRepo, expenseRepo, incomeRepo, new TestWebHostEnvironment(), rulesRepo, expenseReviewRepo, profileRepo);

                var result = await importService.ImportTransactions(request);
                result.ShouldBe("Please inspect the csv file for the correct headers.");
            }
        }
        [Fact]
        public async Task Wrong_Headers_On_Bank_File_Gives_Error()
        {
            var bytes = FileUtilities.GetFileBytes(Path.Combine(FilesFolderPath, "test-bank-import-bad-headers.csv"));
            var request = new ImportModel()
            {
                File = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "test-bank-import-bad-headers", "test-bank-import-bad-headers.csv"),
                FileType = "Bank",
                ReturnUrl = "/"
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var incomeReviewRepo = new IncomeReviewRepository(db);
                var expenseReviewRepo = new ExpenseReviewRepository(db);
                var incomeRepo = new IncomeRepository(db);
                var expenseRepo = new ExpenseRepository(db);
                var rulesRepo = new ImportRulesRepository(db);
                var profileRepo = new ImportProfileRepository(db);
                var importService = new ImportService(incomeReviewRepo, expenseRepo, incomeRepo, new TestWebHostEnvironment(), rulesRepo, expenseReviewRepo, profileRepo);

                var result = await importService.ImportTransactions(request);
                result.ShouldBe("Please inspect the csv file for the correct headers.");
            }
        }
        [Fact]
        public async Task Bad_Formmatting_On_Bank_File_Gives_Error()
        {
            var bytes = FileUtilities.GetFileBytes(Path.Combine(FilesFolderPath, "test-bank-import-bad-formatting.csv"));
            var request = new ImportModel()
            {
                File = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "test-bank-import-bad-formatting", "test-bank-import-bad-formatting.csv"),
                FileType = "Bank",
                ReturnUrl = "/"
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var incomeReviewRepo = new IncomeReviewRepository(db);
                var expenseReviewRepo = new ExpenseReviewRepository(db);
                var incomeRepo = new IncomeRepository(db);
                var expenseRepo = new ExpenseRepository(db);
                var rulesRepo = new ImportRulesRepository(db);
                var profileRepo = new ImportProfileRepository(db);
                var importService = new ImportService(incomeReviewRepo, expenseRepo, incomeRepo, new TestWebHostEnvironment(), rulesRepo, expenseReviewRepo, profileRepo);

                var result = await importService.ImportTransactions(request);
                result.ShouldBe("No transactions imported - is the file formatted properly?");
            }
        }
        [Fact]
        public async Task Bad_Formmatting_On_Credit_File_Gives_Error()
        {
            var bytes = FileUtilities.GetFileBytes(Path.Combine(FilesFolderPath, "test-credit-import-bad-formatting.csv"));
            var request = new ImportModel()
            {
                File = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "test-credit-import-bad-formatting", "test-credit-import-bad-formatting.csv"),
                FileType = "Credit",
                ReturnUrl = "/"
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var incomeReviewRepo = new IncomeReviewRepository(db);
                var expenseReviewRepo = new ExpenseReviewRepository(db);
                var incomeRepo = new IncomeRepository(db);
                var expenseRepo = new ExpenseRepository(db);
                var rulesRepo = new ImportRulesRepository(db);
                var profileRepo = new ImportProfileRepository(db);
                var importService = new ImportService(incomeReviewRepo, expenseRepo, incomeRepo, new TestWebHostEnvironment(), rulesRepo, expenseReviewRepo, profileRepo);

                var result = await importService.ImportTransactions(request);
                result.ShouldBe("No transactions imported - is the file formatted properly?");
            }
        }
        [Fact]
        public async Task Get_Transactions_From_Bank_File()
        {
            var newFilePath = await CopyTestFileToRootForTesting("test-bank-import.csv");
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var incomeReviewRepo = new IncomeReviewRepository(db);
                var expenseReviewRepo = new ExpenseReviewRepository(db);
                var incomeRepo = new IncomeRepository(db);
                var expenseRepo = new ExpenseRepository(db);
                var rulesRepo = new ImportRulesRepository(db);
                var profileRepo = new ImportProfileRepository(db);
                var service = new ImportService(incomeReviewRepo, expenseRepo, incomeRepo, new TestWebHostEnvironment(), rulesRepo, expenseReviewRepo, profileRepo);
                var rules = await rulesRepo.Find(x => x.FileType == "Bank");
                var result = await service.GetTransactionsFromFileAsync(newFilePath, "Bank", rules);
                result.Count().ShouldBe(9);
            }
        }
        [Fact]
        public async Task Get_Transactions_From_Credit_File()
        {
            var newFilePath = await CopyTestFileToRootForTesting("test-credit-import.csv");
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var incomeReviewRepo = new IncomeReviewRepository(db);
                var expenseReviewRepo = new ExpenseReviewRepository(db);
                var incomeRepo = new IncomeRepository(db);
                var expenseRepo = new ExpenseRepository(db);
                var rulesRepo = new ImportRulesRepository(db);
                var profileRepo = new ImportProfileRepository(db);
                var service = new ImportService(incomeReviewRepo, expenseRepo, incomeRepo, new TestWebHostEnvironment(), rulesRepo, expenseReviewRepo, profileRepo);
                var rules = await rulesRepo.Find(x => x.FileType == "Credit");
                var result = await service.GetTransactionsFromFileAsync(newFilePath, "Credit", rules);
                result.Count().ShouldBe(7);
            }
        }
        [Fact]
        public async Task Filter_Transactions_From_Database()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetImportService(db);
                var ImportTransactions = new List<ImportTransaction>()
                {
                    new ImportTransaction()
                    {
                        Date = new DateTime(2012, 04, 24),
                        Amount = 42.53m,
                        Notes = "Just kidding",
                        IsIncome = false
                    },
                    new ImportTransaction()
                    {
                        Date = new DateTime(2012, 04, 24),
                        Amount = new Decimal(42.52),
                        Notes = "This works",
                        IsIncome = false
                    },
                    new ImportTransaction()
                    {
                        Date = new DateTime(2012, 04, 23),
                        Amount = new Decimal(50),
                        Notes = "Just kidding",
                        IsIncome = true
                    },
                    new ImportTransaction()
                    {
                        Date = new DateTime(2012, 04, 24),
                        Amount = new Decimal(50),
                        Notes = "This works",
                        IsIncome = true
                    },

                };
                var result = (await service.FilterTransactionsInDatabase(ImportTransactions)).ToList();
                result.Count().ShouldBe(2);
                foreach (var transaction in result)
                {
                    transaction.Notes.ShouldBe("this works");
                }
            }
        }
        [Fact]
        public async Task Filter_Transactions_From_Database_In_Review_Table()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var incomeReviewRepo = new IncomeReviewRepository(db);
                var expenseReviewRepo = new ExpenseReviewRepository(db);
                var incomeRepo = new IncomeRepository(db);
                var expenseRepo = new ExpenseRepository(db);
                var rulesRepo = new ImportRulesRepository(db);
                var profileRepo = new ImportProfileRepository(db);
                var service = new ImportService(incomeReviewRepo, expenseRepo, incomeRepo, new TestWebHostEnvironment(), rulesRepo, expenseReviewRepo, profileRepo);

                await incomeReviewRepo.Create(new IncomeReviewEntity()
                {
                    Date = new DateTime(2012, 04, 24),
                    Amount = new Decimal(50),
                    Notes = "This works"
                });

                await expenseReviewRepo.Create(new ExpenseReviewEntity()
                {
                    Date = new DateTime(2012, 04, 24),
                    Amount = new Decimal(42.52),
                    Notes = "This works"
                });
                var ImportTransactions = new List<ImportTransaction>(){
                    new ImportTransaction()
                    {
                        Date = new DateTime(2012, 04, 24),
                        Amount = new Decimal(42.52),
                        Notes = "This won't work",
                        IsIncome = false
                    },
                    new ImportTransaction()
                    {
                        Date = new DateTime(2012, 04, 24),
                        Amount = new Decimal(50),
                        Notes = "This won't work",
                        IsIncome = true
                    },
                };
                var result = (await service.FilterTransactionsInDatabase(ImportTransactions)).ToList();
                result.ShouldBeEmpty();
            }
        }
        [Fact]
        public async Task Set_Import_Rules_Sets_Properties()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var incomeReviewRepo = new IncomeReviewRepository(db);
                var expenseReviewRepo = new ExpenseReviewRepository(db);
                var incomeRepo = new IncomeRepository(db);
                var expenseRepo = new ExpenseRepository(db);
                var rulesRepo = new ImportRulesRepository(db);
                var profileRepo = new ImportProfileRepository(db);
                var service = new ImportService(incomeReviewRepo, expenseRepo, incomeRepo, new TestWebHostEnvironment(), rulesRepo, expenseReviewRepo, profileRepo);
                var rules = await rulesRepo.Find(x => true);
                var imports = new List<ImportTransaction>()
                {
                    new ImportTransaction()
                    {
                        Date = new DateTime(2012, 04, 24),
                        Amount = 25m,
                        Notes = "tip",
                        IsIncome = true
                    },
                    new ImportTransaction()
                    {
                        Date = new DateTime(2012, 04, 24),
                        Amount = 5m,
                        Notes = "amazon",
                        IsIncome = true
                    },
                    new ImportTransaction()
                    {
                        Date = new DateTime(2012, 04, 24),
                        Amount = 850m,
                        Notes = "rent",
                        IsIncome = false
                    },
                    new ImportTransaction()
                    {
                        Date = new DateTime(2012, 04, 24),
                        Amount = 1000m,
                        Notes = "Kaiser",
                        IsIncome = false
                    }
                };
                var results = service.SetImportRules(imports, rules).ToList();
                foreach (var result in results)
                {
                    result.MerchantSourceId.HasValue.ShouldBeTrue();
                    result.CategoryId.HasValue.ShouldBeTrue();
                }
            }
        }
        private async Task<string> CopyTestFileToRootForTesting(string testFileName)
        {
            //need to copy file to root so because the SUT deletes the file after reading it
            var bytes = FileUtilities.GetFileBytes(Path.Combine(FilesFolderPath, testFileName));
            var file = new FormFile(new MemoryStream(bytes), 0, bytes.Length, testFileName, testFileName);
            var newFilePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName, file.FileName);

            using (var fileStream = new FileStream(newFilePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return newFilePath;
        }
        private ImportService GetImportService(AppDbContext db)
        {
            var incomeReviewRepo = new IncomeReviewRepository(db);
            var expenseReviewRepo = new ExpenseReviewRepository(db);
            var incomeRepo = new IncomeRepository(db);
            var expenseRepo = new ExpenseRepository(db);
            var rulesRepo = new ImportRulesRepository(db);
            var profileRepo = new ImportProfileRepository(db);
            return new ImportService(incomeReviewRepo, expenseRepo, incomeRepo, new TestWebHostEnvironment(), rulesRepo, expenseReviewRepo, profileRepo);
        }
    }
}
