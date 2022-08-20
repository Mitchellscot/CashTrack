using AutoMapper;
using CashTrack.Common.Exceptions;
using CashTrack.Data;
using CashTrack.Models.Common;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Repositories.ExpenseRepository;
using CashTrack.Repositories.SubCategoriesRepository;
using CashTrack.Services.SubCategoryService;
using CashTrack.Tests.Services.Common;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CashTrack.Tests.Services
{
    public class SubCategoryServiceTests
    {
        private readonly IMapper _mapper;
        private readonly SubCategoryService _service;

        public SubCategoryServiceTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<SubCategoryMapperProfile>();
            });
            _mapper = config.CreateMapper();
            var db = new AppDbContextFactory().CreateDbContext();
            var repo = new SubCategoryRepository(db);
            var expenseRepo = new ExpenseRepository(db);
            _service = new SubCategoryService(expenseRepo, repo, _mapper);
        }
        [Fact]
        public async Task Get_SubCategory_Detail()
        {
            //TODO!!!!
            await Task.Run(() => Should.Throw<NotImplementedException>(async () => await _service.GetSubCategoryDetailsAsync(1)));
        }
        [Fact]
        public async Task Get_All_Categories_For_Dropdown()
        {
            var result = await _service.GetSubCategoryDropdownListAsync();
            result.Length.ShouldBe(31);
        }
        [Theory]
        [InlineData("Groceries")]
        [InlineData("Rent")]
        [InlineData("Books")]
        public async Task Get_Categories_By_name(string category)
        {
            var result = await _service.GetSubCategoryByNameAsync(category);
            result.Name.ShouldBe(category);
        }
        [Theory]
        [InlineData("Groceries")]
        [InlineData("Rent")]
        [InlineData("Books")]
        public async Task Get_Category_Matches(string category)
        {
            var result = await _service.GetMatchingSubCategoryNamesAsync(category);
            result[0].ShouldBe(category);
        }
        [Fact]
        public async Task Creates_SubCategory()
        {
            var category = new SubCategory()
            {
                Name = "Category",
                Notes = "created",
                MainCategoryId = 1
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetCategoryService(db);
                var repo = new SubCategoryRepository(db);
                var lastId = (await repo.GetCount(x => true) + 1);
                var created = await service.CreateSubCategoryAsync(category);
                created.ShouldBe(lastId);
            }
        }
        [Fact]
        public async Task Throws_On_Duplicate_Names_When_Creating()
        {
            var category = new SubCategory()
            {
                Name = "Groceries",
                Notes = "created",
                MainCategoryId = 1
            };
            await Task.Run(() => Should.Throw<DuplicateNameException>(async () => await _service.CreateSubCategoryAsync(category)));
        }
        [Fact]
        public async Task Throws_If_No_Main_Category()
        {
            var category = new SubCategory()
            {
                Name = "xyz",
                Notes = "created",
                MainCategoryId = 0
            };
            await Task.Run(() => Should.Throw<CategoryNotFoundException>(async () => await _service.CreateSubCategoryAsync(category)));
        }
        [Fact]
        public async Task Updates_SubCategory()
        {
            var category = new SubCategory()
            {
                Id = 1,
                Name = "Category",
                Notes = "updated",
                MainCategoryId = 1
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetCategoryService(db);
                var repo = new SubCategoryRepository(db);

                var updated = await service.UpdateSubCategoryAsync(category);

                updated.ShouldBe(category.Id.Value);
                var updatedCategory = await repo.FindById(category.Id.Value);
                updatedCategory.Name.ShouldBe(category.Name);
            }
        }
        [Fact]
        public async Task Throws_On_Duplicate_Names_When_Updating()
        {
            var category = new SubCategory()
            {
                Id = 1,
                Name = "Groceries",
                Notes = "created",
                MainCategoryId = 1
            };
            await Task.Run(() => Should.Throw<DuplicateNameException>(async () => await _service.UpdateSubCategoryAsync(category)));
        }
        [Fact]
        public async Task Throws_On_Invalid_Id_When_Updating()
        {
            var category = new SubCategory()
            {
                Id = int.MaxValue,
                Name = "category",
                Notes = "created",
                MainCategoryId = 1
            };
            await Task.Run(() => Should.Throw<CategoryNotFoundException>(async () => await _service.UpdateSubCategoryAsync(category)));
        }
        [Fact]
        public async Task Deletes_SubCategory()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetCategoryService(db);
                var deleted = await service.DeleteSubCategoryAsync(1);
                deleted.ShouldBeTrue();
            }
        }
        [Fact]
        public async Task Deleting_SubCategory_Unassigns_Expenses()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var repo = new SubCategoryRepository(db);
                var expenseRepo = new ExpenseRepository(db);
                var service = new SubCategoryService(expenseRepo, repo, _mapper);
                var sampleExpense = (await expenseRepo.Find(x => x.CategoryId == 32)).FirstOrDefault();
                sampleExpense!.Category!.Name.ShouldBe("Rent");
                var deleted = await service.DeleteSubCategoryAsync(32);
                deleted.ShouldBeTrue();
                var reassignedExpenses = await expenseRepo.Find(x => x.CategoryId == sampleExpense.CategoryId);
                reassignedExpenses.FirstOrDefault()!.Category!.Name.ShouldBe("Uncategorized");
            }
        }
        [Fact]
        public async Task Throws_On_Invalid_Id_When_Deleting()
        {
            await Task.Run(() => Should.Throw<CategoryNotFoundException>(async () => await _service.DeleteSubCategoryAsync(int.MaxValue)));
        }
        [Fact]
        public async Task Gets_SubCategories_By_Name()
        {
            var request = new SubCategoryRequest()
            {
                Order = SubCategoryOrderBy.Name,
                Reversed = false
            };
            var result = await _service.GetSubCategoriesAsync(request);
            result.ListItems!.FirstOrDefault()!.Name.ShouldBe("AAA");
        }
        [Fact]
        public async Task Gets_SubCategories_By_Name_Reversed()
        {
            var request = new SubCategoryRequest()
            {
                Order = SubCategoryOrderBy.Name,
                Reversed = true
            };
            var result = await _service.GetSubCategoriesAsync(request);
            result.ListItems!.FirstOrDefault()!.Name.ShouldBe("Travel Misc");
        }
        [Fact]
        public async Task Gets_SubCategories_By_Main_Category()
        {
            var request = new SubCategoryRequest()
            {
                Order = SubCategoryOrderBy.MainCategory,
                Reversed = false
            };
            var result = await _service.GetSubCategoriesAsync(request);
            result.ListItems!.FirstOrDefault()!.MainCategoryName.ShouldBe("Career");
        }
        [Fact]
        public async Task Gets_SubCategories_By_Main_Category_Reversed()
        {
            var request = new SubCategoryRequest()
            {
                Order = SubCategoryOrderBy.MainCategory,
                Reversed = true
            };
            var result = await _service.GetSubCategoriesAsync(request);
            result.ListItems!.FirstOrDefault()!.MainCategoryName.ShouldBe("Vacation");
        }
        [Fact]
        public async Task Gets_SubCategories_By_Main_Purchases()
        {
            var request = new SubCategoryRequest()
            {
                Order = SubCategoryOrderBy.Purchases,
                Reversed = false
            };
            var result = await _service.GetSubCategoriesAsync(request);
            result.ListItems!.FirstOrDefault()!.Purchases.ShouldBe(1);
        }
        [Fact]
        public async Task Gets_SubCategories_By_Main_Purchases_Reversed()
        {
            var request = new SubCategoryRequest()
            {
                Order = SubCategoryOrderBy.Purchases,
                Reversed = true
            };
            var result = await _service.GetSubCategoriesAsync(request);
            result.ListItems!.FirstOrDefault()!.Purchases.ShouldBe(161);
        }
        [Fact]
        public async Task Gets_SubCategories_By_Amount()
        {
            var request = new SubCategoryRequest()
            {
                Order = SubCategoryOrderBy.Amount,
                Reversed = false
            };
            var result = await _service.GetSubCategoriesAsync(request);
            result.ListItems!.FirstOrDefault()!.Amount.ShouldBe(4.32m);
        }
        [Fact]
        public async Task Gets_SubCategories_By_Amount_Reversed()
        {
            var request = new SubCategoryRequest()
            {
                Order = SubCategoryOrderBy.Amount,
                Reversed = true
            };
            var result = await _service.GetSubCategoriesAsync(request);
            result.ListItems!.FirstOrDefault()!.Amount.ShouldBe(10200m);
        }
        [Fact]
        public async Task Gets_SubCategories_By_LastPurchase()
        {
            var request = new SubCategoryRequest()
            {
                Order = SubCategoryOrderBy.LastPurchase,
                Reversed = false
            };
            var result = await _service.GetSubCategoriesAsync(request);
            result.ListItems!.FirstOrDefault()!.LastPurchase.ShouldBe(new System.DateTime(2012, 02, 07));
        }
        [Fact]
        public async Task Gets_SubCategories_By_LastPurchase_Reversed()
        {
            var request = new SubCategoryRequest()
            {
                Order = SubCategoryOrderBy.LastPurchase,
                Reversed = true
            };
            var result = await _service.GetSubCategoriesAsync(request);
            result.ListItems!.FirstOrDefault()!.LastPurchase.ShouldBe(new System.DateTime(2012, 12, 30));
        }
        [Fact]
        public async Task Gets_SubCategories_By_In_Use()
        {
            var request = new SubCategoryRequest()
            {
                Order = SubCategoryOrderBy.InUse,
                Reversed = false
            };
            var result = await _service.GetSubCategoriesAsync(request);
            result.ListItems!.FirstOrDefault()!.InUse.ShouldBeFalse();
        }
        [Fact]
        public async Task Gets_SubCategories_By_In_Use_Reversed()
        {
            var request = new SubCategoryRequest()
            {
                Order = SubCategoryOrderBy.InUse,
                Reversed = true
            };
            var result = await _service.GetSubCategoriesAsync(request);
            result.ListItems!.FirstOrDefault()!.InUse.ShouldBeTrue();
        }
        private SubCategoryService GetCategoryService(AppDbContext db)
        {
            var repo = new SubCategoryRepository(db);
            var expenseRepo = new ExpenseRepository(db);
            return new SubCategoryService(expenseRepo, repo, _mapper);
        }
    }
}
