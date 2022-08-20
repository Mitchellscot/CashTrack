using CashTrack.Common.Exceptions;
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
        //[Fact(Skip = "Needs refactoring after UI setup")]
        //public async Task Get_Income_Categories()
        //{
        //    var request = new IncomeCategoryRequest();
        //    var result = await _service.GetIncomeCategoriesAsync(request);
        //    result.ShouldNotBeNull();
        //}
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
                var newCategory = new AddEditIncomeCategory()
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
            var newCategory = new AddEditIncomeCategory()
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
                var request = new AddEditIncomeCategory()
                {
                    Id = 1,
                    Name = "Updated Name",
                    InUse = true,
                    Notes = "Updated!"
                };
                var result = await service.UpdateIncomeCategoryAsync(request);
                result.ShouldBe(1);
                var updatedCategory = (await service.GetIncomeCategoryNames()).FirstOrDefault(x => x == "Updated Name");
                updatedCategory.ShouldNotBeNull();
            }
        }
        [Fact]
        public async Task Throws_On_Duplicate_Name_Updating()
        {
            var request = new AddEditIncomeCategory()
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
            var request = new AddEditIncomeCategory()
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
