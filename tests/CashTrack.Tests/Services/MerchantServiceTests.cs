using AutoMapper;
using CashTrack.Common.Exceptions;
using CashTrack.Data;
using CashTrack.Models.Common;
using CashTrack.Models.MerchantModels;
using CashTrack.Repositories.ExpenseRepository;
using CashTrack.Repositories.MerchantRepository;
using CashTrack.Repositories.SubCategoriesRepository;
using CashTrack.Services.MerchantService;
using CashTrack.Tests.Services.Common;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CashTrack.Tests.Services
{
    public class MerchantServiceTests
    {
        private readonly IMapper _mapper;
        private readonly MerchantService _service;

        public MerchantServiceTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MerchantMapperProfile>();
            });
            _mapper = config.CreateMapper();
            var sharedDB = new AppDbContextFactory().CreateDbContext();
            var repo = new MerchantRepository(sharedDB);
            var subCategoryRepo = new SubCategoryRepository(sharedDB);
            var expenseRepo = new ExpenseRepository(sharedDB);
            _service = new MerchantService(_mapper, repo, subCategoryRepo, expenseRepo);
        }
        [Fact]
        public async Task Get_All_Merchant_Names()
        {
            var result = await _service.GetAllMerchantNames();
            result.Length.ShouldBe(16);
        }
        [Theory]
        [InlineData("Costco")]
        [InlineData("In N Out")]
        [InlineData("Target")]
        public async Task Get_All_Merchants_By_name(string merchantName)
        {
            var result = await _service.GetMerchantByNameAsync(merchantName);
            result.Name.ShouldBe(merchantName);
        }
        [Fact]
        public async Task Throws_When_Name_Is_Invalid()
        {
            await Task.Run(() => Should.Throw<MerchantNotFoundException>(async () => await _service.GetMerchantByNameAsync("abcdefghijklmnopqrstuvwxyz")));
        }
        [Theory]
        [InlineData("Costco")]
        [InlineData("In N Out")]
        [InlineData("Target")]
        public async Task Get_All_Matching_Merchants(string merchantName)
        {
            var result = await _service.GetMatchingMerchantsAsync(merchantName);
            result[0].ShouldBe(merchantName);
        }
        [Fact]
        public async Task Deletes_A_Merchant()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetMerchantService(db);
                var result = await service.DeleteMerchantAsync(1);
                result.ShouldBeTrue();
                await Task.Run(() => Should.Throw<MerchantNotFoundException>(async () =>
                await service.DeleteMerchantAsync(1)));
            }
        }
        [Fact]
        public async Task Deletes_A_Merchant_Unassigns_Expenses()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetMerchantService(db);
                var expenseRepo = new ExpenseRepository(db);
                var result = await service.DeleteMerchantAsync(3);
                result.ShouldBeTrue();
                var unassignedExpenses = await expenseRepo.Find(x => x.Category!.Name == "Rent");
                unassignedExpenses!.FirstOrDefault()!.MerchantId.ShouldBeNull();
            }
        }
        [Fact]
        public async Task Updates_A_Merchant()
        {
            var merchant = new Merchant()
            {
                Id = 5,
                Name = "Costco",
                Notes = "updated",
                City = "Rohnert Park",
                State = "CA"
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetMerchantService(db);
                var updatedMerchantId = await service.UpdateMerchantAsync(merchant);
                updatedMerchantId.ShouldBe(5);
                var updatedMerchant = await service.GetMerchantByNameAsync("Costco");
                updatedMerchant.Notes.ShouldBe("updated");
            }
        }
        [Fact]
        public async Task Throws_On_Duplicate_Name_If_Updating()
        {
            var merchant = new Merchant()
            {
                Id = 4,
                Name = "Costco",
                Notes = "updated",
                City = "Rohnert Park",
                State = "CA"
            };

            await Task.Run(() => Should.Throw<DuplicateNameException>(async () => await _service.UpdateMerchantAsync(merchant)));
        }
        [Fact]
        public async Task Throws_On_Duplicate_Name_If_Creating()
        {
            var merchant = new Merchant()
            {
                Name = "Costco",
                Notes = "created",
                City = "Rohnert Park",
                State = "CA"
            };

            await Task.Run(() => Should.Throw<DuplicateNameException>(async () => await _service.CreateMerchantAsync(merchant)));
        }
        [Fact]
        public async Task Create_Merchant_Works()
        {
            var merchant = new Merchant()
            {
                Name = "Scott's Labor Leasing",
                Notes = "created",
                City = "Long Beach",
                State = "CA"
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetMerchantService(db);
                var repo = new MerchantRepository(db);
                var lastId = (await repo.GetCount(x => true) + 1);
                var created = await service.CreateMerchantAsync(merchant);
                created.ShouldBe(lastId);
            }
        }
        [Fact]
        public async Task Gets_Sub_Category_Totals()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var repo = new MerchantRepository(db);
                var subCategoryRepo = new SubCategoryRepository(db);
                var expenseRepo = new ExpenseRepository(db);
                var merchantService = new MerchantService(_mapper, repo, subCategoryRepo, expenseRepo);
                var subCategories = await subCategoryRepo.Find(x => true);
                var expenses = await expenseRepo.Find(x => true);
                var result = merchantService.GetExpenseCategoryTotals(subCategories, expenses);
                result.Count.ShouldBe(37);
                var rentTotal = result.Where(x => x.Key == "Rent").Select(x => x.Value).FirstOrDefault();
                rentTotal.ShouldBe(10200);
            }
        }
        [Fact]
        public async Task Gets_Expense_Category_Occurances()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var repo = new MerchantRepository(db);
                var subCategoryRepo = new SubCategoryRepository(db);
                var expenseRepo = new ExpenseRepository(db);
                var merchantService = new MerchantService(_mapper, repo, subCategoryRepo, expenseRepo);
                var subCategories = await subCategoryRepo.Find(x => true);
                var expenses = await expenseRepo.Find(x => true);
                var result = merchantService.GetExpenseCategoryOccurances(subCategories, expenses);
                result.Count.ShouldBe(38);
                var timesWeAteOut = result.Where(x => x.Key == "Dining Out").Select(x => x.Value).FirstOrDefault();
                timesWeAteOut.ShouldBe(161);
            }
        }
        [Fact]
        public async Task Get_Merchant_Details()
        {
            var result = await _service.GetMerchantDetailAsync(5);
            result.Name.ShouldBe("Costco");
            result.MostUsedCategory.ShouldBe("Groceries");
            result.RecentExpenses.ShouldNotBeEmpty();
            result.PurchaseCategoryTotals.ShouldNotBeEmpty();
            result.PurchaseCategoryOccurances.ShouldNotBeEmpty();
            result.ExpenseTotals.Average.ShouldNotBe(0);
            result.ExpenseTotals.Count.ShouldNotBe(0);
            result.ExpenseTotals.Max.ShouldNotBe(0);
            result.ExpenseTotals.Min.ShouldNotBe(0);

        }
        [Fact]
        public async Task Get_Merchant_List_Items_By_Name()
        {
            var request = new MerchantRequest()
            {
                Order = MerchantOrderBy.Name,
                Reversed = false
            };
            var result = await _service.GetMerchantsAsync(request);
            result.ListItems.FirstOrDefault()!.Name.ShouldBe("AAA");
        }
        [Fact]
        public async Task Get_Merchant_List_Items_By_Name_Reversed()
        {
            var request = new MerchantRequest()
            {
                Order = MerchantOrderBy.Name,
                Reversed = true
            };
            var result = await _service.GetMerchantsAsync(request);
            result.ListItems.FirstOrDefault()!.Name.ShouldBe("Torch of India");
        }
        [Fact]
        public async Task Get_Merchant_List_Items_By_Location()
        {
            var request = new MerchantRequest()
            {
                Order = MerchantOrderBy.Location,
                Reversed = false
            };
            var result = await _service.GetMerchantsAsync(request);
            result.ListItems.FirstOrDefault()!.Location.ShouldBeNull();
        }
        [Fact]
        public async Task Get_Merchant_List_Items_By_Location_Reversed()
        {
            var request = new MerchantRequest()
            {
                Order = MerchantOrderBy.Location,
                Reversed = true
            };
            var result = await _service.GetMerchantsAsync(request);
            result.ListItems.FirstOrDefault()!.Location.ShouldBe("Sebastopol, CA");
        }
        [Fact]
        public async Task Get_Merchant_List_Items_By_Purchases()
        {
            var request = new MerchantRequest()
            {
                Order = MerchantOrderBy.Purchases,
                Reversed = false
            };
            var result = await _service.GetMerchantsAsync(request);
            result.ListItems.FirstOrDefault()!.NumberOfExpenses.ShouldBe(1);
        }
        [Fact]
        public async Task Get_Merchant_List_Items_By_Purchases_Reversed()
        {
            var request = new MerchantRequest()
            {
                Order = MerchantOrderBy.Purchases,
                Reversed = true
            };
            var result = await _service.GetMerchantsAsync(request);
            result.ListItems.FirstOrDefault()!.NumberOfExpenses.ShouldBe(58);
        }
        [Fact]
        public async Task Get_Merchant_List_Items_By_Amount()
        {
            var request = new MerchantRequest()
            {
                Order = MerchantOrderBy.Amount,
                Reversed = false
            };
            var result = await _service.GetMerchantsAsync(request);
            result.ListItems.FirstOrDefault()!.TotalSpent.ShouldBe(50M);
        }
        [Fact]
        public async Task Get_Merchant_List_Items_By_Amount_Reversed()
        {
            var request = new MerchantRequest()
            {
                Order = MerchantOrderBy.Amount,
                Reversed = true
            };
            var result = await _service.GetMerchantsAsync(request);
            result.ListItems.FirstOrDefault()!.TotalSpent.ShouldBe(10423.47M);
        }
        [Fact]
        public async Task Get_Merchant_List_Items_By_Last_Purchase()
        {
            var request = new MerchantRequest()
            {
                Order = MerchantOrderBy.LastPurchase,
                Reversed = false
            };
            var result = await _service.GetMerchantsAsync(request);
            result.ListItems.FirstOrDefault()!.LastPurchase.ShouldBe(new DateTime(2012, 01, 20));
        }
        [Fact]
        public async Task Get_Merchant_List_Items_By_Last_Purchase_Reversed()
        {
            var request = new MerchantRequest()
            {
                Order = MerchantOrderBy.LastPurchase,
                Reversed = true
            };
            var result = await _service.GetMerchantsAsync(request);
            result.ListItems.FirstOrDefault()!.LastPurchase.ShouldBe(new DateTime(2012, 12, 30));
        }
        [Fact]
        public async Task Get_Merchant_List_Items_By_Category()
        {
            var request = new MerchantRequest()
            {
                Order = MerchantOrderBy.Category,
                Reversed = false
            };
            var result = await _service.GetMerchantsAsync(request);
            result.ListItems.FirstOrDefault()!.Category.ShouldBe("AAA");
        }
        [Fact]
        public async Task Get_Merchant_List_Items_By_Category_Reversed()
        {
            var request = new MerchantRequest()
            {
                Order = MerchantOrderBy.Category,
                Reversed = true
            };
            var result = await _service.GetMerchantsAsync(request);
            result.ListItems.FirstOrDefault()!.Category.ShouldBe("Laundry");
        }

        private MerchantService GetMerchantService(AppDbContext db)
        {
            var repo = new MerchantRepository(db);
            var subCategoryRepo = new SubCategoryRepository(db);
            var expenseRepo = new ExpenseRepository(db);
            var service = new MerchantService(_mapper, repo, subCategoryRepo, expenseRepo);
            return service;
        }
    }
}
