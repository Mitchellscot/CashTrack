using CashTrack.Common.Exceptions;
using CashTrack.Common.Extensions;
using CashTrack.Data;
using CashTrack.Models.IncomeCategoryModels;
using CashTrack.Repositories.IncomeCategoryRepository;
using CashTrack.Repositories.IncomeRepository;
using CashTrack.Services.IncomeCategoryService;
using CashTrack.Tests.Services.Common;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CashTrack.Tests.Services
{
    public class IncomeCategoryTests
    {
        private IncomeCategoryService _service;

        public IncomeCategoryTests()
        {
            var db = new AppDbContextFactory().CreateDbContext();
            var repo = new IncomeCategoryRepository(db);
            var incomeRepo = new IncomeRepository(db);
            _service = new IncomeCategoryService(repo, incomeRepo);
        }
        [Fact]
        public async Task Get_Category_Detail()
        {
            var result = await _service.GetCategoryDetailAsync(10);
            result.Name.ShouldBe("Bonus");
            result.RecentIncome.ShouldNotBeEmpty();
            result.SourcePurchaseTotals.ShouldNotBeEmpty();
            result.SourcePurchaseOccurances.ShouldNotBeEmpty();
            result.IncomeTotals.Average.ShouldNotBe(0);
            result.IncomeTotals.Count.ShouldNotBe(0);
            result.IncomeTotals.Max.ShouldNotBe(0);
            result.IncomeTotals.Min.ShouldNotBe(0);
        }
        [Fact]
        public async Task Get_Income_Categories()
        {
            var request = new IncomeCategoryRequest();
            var result = await _service.GetIncomeCategoriesAsync(request);
            result.ShouldNotBeNull();
            result.TotalCount.ShouldBe(11);
            result.PageNumber.ShouldBe(1);
            result.PageSize.ShouldBe(20);
            var items = result.ListItems.ToArray();
            foreach (var item in items)
            {
                item.Name.ShouldNotBeNullOrEmpty();
            }
        }
        [Theory]
        [InlineData("Paycheck")]
        [InlineData("Tip")]
        [InlineData("Gift")]
        public async Task Get_Category_Matches(string category)
        {
            var result = await _service.GetMatchingIncomeCategoryNamesAsync(category);
            result[0].ShouldBe(category);
        }
        [Theory]
        [InlineData("Paycheck")]
        [InlineData("Tip")]
        [InlineData("Gift")]
        public async Task Get_Categories_By_name(string category)
        {
            var result = await _service.GetIncomeCategoryByNameAsync(category);
            result.Name.ShouldBe(category);
        }
        [Fact]
        public async Task Get_Categories_For_Dropdown_List()
        {
            var result = await _service.GetIncomeCategoryDropdownListAsync();
            result.Count().ShouldBe(10);
        }
        [Fact]
        public async Task Get_Category_Names()
        {
            var result = await _service.GetIncomeCategoryNames();
            result.Count().ShouldBe(10);
        }
        [Fact]
        public async Task Check_If_Category_Is_Refund()
        {
            var result = await _service.CheckIfIncomeCategoryIsRefund(9);
            result.ShouldBeTrue();
            var result2 = await _service.CheckIfIncomeCategoryIsRefund(1);
            result2.ShouldBeFalse();
        }
        [Fact]
        public async Task Create_Income_Category()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetService(db);
                var newCategory = new AddEditIncomeCategoryModal()
                {
                    Name = "new Category",
                    Notes = "New!",
                    InUse = true
                };
                var result = await service.CreateIncomeCategoryAsync(newCategory);
                result.ShouldBe(12);

            }
        }
        [Fact]
        public async Task Throws_On_Duplicate_Name_Creating()
        {
            var newCategory = new AddEditIncomeCategoryModal()
            {
                Name = "Paycheck",
                Notes = "New!",
                InUse = true
            };
            await Task.Run(() => Should.Throw<DuplicateNameException>(async () => await _service.CreateIncomeCategoryAsync(newCategory)));
        }
        [Fact]
        public async Task Throws_On_Invalid_Id_Deleting()
        {
            await Task.Run(() => Should.Throw<CategoryNotFoundException>(async () => await _service.DeleteIncomeCategoryAsync(int.MaxValue)));
        }
        [Fact]
        public async Task Deleting_Category()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetService(db);
                var result = await service.DeleteIncomeCategoryAsync(1);
                result.ShouldBeTrue();
            }
        }
        [Fact]
        public async Task Deleting_Category_Unassigns_Income_Categories()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var repo = new IncomeCategoryRepository(db);
                var incomeRepo = new IncomeRepository(db);
                var service = new IncomeCategoryService(repo, incomeRepo);
                var sampleIncome = (await incomeRepo.Find(x => x.CategoryId == 7)).FirstOrDefault();
                sampleIncome!.Category!.Name.ShouldBe("Tip");
                var result = await service.DeleteIncomeCategoryAsync(7);
                result.ShouldBeTrue();
                var reassignedIncome = await incomeRepo.Find(x => x.CategoryId == sampleIncome.CategoryId);
                reassignedIncome.FirstOrDefault()!.Category!.Name.ShouldBe("Uncategorized");
            }
        }
        [Fact]
        public async Task Updating_Category()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetService(db);
                var request = new AddEditIncomeCategoryModal()
                {
                    Id = 1,
                    Name = "Updated Category",
                    InUse = true,
                    Notes = "Updated!"
                };
                var result = await service.UpdateIncomeCategoryAsync(request);
                result.ShouldBe(1);
                var updatedCategory = (await service.GetIncomeCategoryNames()).FirstOrDefault(x => x.IsEqualTo("Updated Category"));
                updatedCategory.ShouldNotBeNull();
            }
        }
        [Fact]
        public async Task Throws_On_Duplicate_Name_Updating()
        {
            var request = new AddEditIncomeCategoryModal()
            {
                Id = 1,
                Name = "Paycheck",
                InUse = true,
                Notes = "Updated!"
            };
            await Task.Run(() => Should.Throw<DuplicateNameException>(async () => await _service.UpdateIncomeCategoryAsync(request)));
        }
        [Fact]
        public async Task Throws_On_Invalid_Id_Updating()
        {
            var request = new AddEditIncomeCategoryModal()
            {
                Id = int.MaxValue,
                Name = "Paycheck",
                InUse = true,
                Notes = "Updated!"
            };
            await Task.Run(() => Should.Throw<DuplicateNameException>(async () => await _service.UpdateIncomeCategoryAsync(request)));
        }
        private IncomeCategoryService GetService(AppDbContext db)
        {
            var repo = new IncomeCategoryRepository(db);
            var incomeRepo = new IncomeRepository(db);
            return new IncomeCategoryService(repo, incomeRepo);
        }
    }
}
