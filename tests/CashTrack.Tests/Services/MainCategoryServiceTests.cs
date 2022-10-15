using CashTrack.Common.Exceptions;
using CashTrack.Data;
using CashTrack.Models.MainCategoryModels;
using CashTrack.Repositories.IncomeCategoryRepository;
using CashTrack.Repositories.IncomeRepository;
using CashTrack.Repositories.MainCategoriesRepository;
using CashTrack.Repositories.SubCategoriesRepository;
using CashTrack.Services.MainCategoriesService;
using CashTrack.Tests.Services.Common;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CashTrack.Tests.Services
{
    public class MainCategoryServiceTests
    {
        private readonly MainCategoriesService _service;

        public MainCategoryServiceTests()
        {
            var db = new AppDbContextFactory().CreateDbContext();
            var repo = new MainCategoriesRepository(db);
            var subCategoryRepo = new SubCategoryRepository(db);
            var incomeRepo = new IncomeRepository(db);
            _service = new MainCategoriesService(repo, subCategoryRepo, incomeRepo);
        }
        [Fact]
        public async Task Get_Main_Categories()
        {
            var request = new MainCategoryRequest();
            var result = await _service.GetMainCategoriesAsync(request);
            result.MainCategories.ShouldNotBeNull();
            result.TotalMainCategories.ShouldBeGreaterThan(0);
            result.CategoryPurchaseOccurances.ShouldNotBeEmpty();
            result.MainCategoryPercentages.ShouldNotBeEmpty();
            result.SubCategoryPercentages.ShouldNotBeEmpty();
            result.MainCategoryChartData.MainCategoryNames.Length.ShouldBeGreaterThan(0);
            result.MainCategoryChartData.SubCategoryData.Count.ShouldBeGreaterThan(0);
            result.SavingsPercentages.ShouldNotBeEmpty();
            result.ShouldNotBeNull();
        }
        [Fact]
        public async Task Get_By_SubCategory_Id()
        {
            var result = await _service.GetMainCategoryNameBySubCategoryIdAsync(1);
            result.ShouldBe("Insurance");
        }
        [Fact]
        public async Task Throws_With_Invalid_SubCategory_Id()
        {
            await Task.Run(() => Should.Throw<CategoryNotFoundException>(async () => await _service.GetMainCategoryNameBySubCategoryIdAsync(int.MaxValue)));
        }
        [Fact]
        public async Task Get_Categories_For_Dropdown_List()
        {
            var result = await _service.GetMainCategoriesForDropdownListAsync();
            result.Count().ShouldBe(16);
        }
        [Fact]
        public async Task Creates_Main_Category()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetService(db);
                var category = new AddEditMainCategoryModal()
                {
                    Name = "category"
                };
                var result = await service.CreateMainCategoryAsync(category);
                result.ShouldBe(17);
            }
        }
        [Fact]
        public async Task Throws_DuplicateName_Error_When_Creating()
        {
            var category = new AddEditMainCategoryModal()
            {
                Name = "Food"
            };
            await Task.Run(() => Should.Throw<DuplicateNameException>(async () => await _service.CreateMainCategoryAsync(category)));
        }
        [Fact]
        public async Task Deletes_Main_Category()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetService(db);
                var result = await service.DeleteMainCategoryAsync(1);
                result.ShouldBeTrue();
            }
        }
        [Fact]
        public async Task Throws_When_Id_Is_Invalid_When_Deleting()
        {
            await Task.Run(() => Should.Throw<CategoryNotFoundException>(async () => await _service.DeleteMainCategoryAsync(int.MaxValue)));
        }
        [Fact]
        public async Task Deleting_Main_Category_Unassigns_SubCategories()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var repo = new MainCategoriesRepository(db);
                var subCategoryRepo = new SubCategoryRepository(db);
                var incomeRepo = new IncomeRepository(db);
                var service = new MainCategoriesService(repo, subCategoryRepo, incomeRepo);

                var subcategories = await subCategoryRepo.Find(x => x.MainCategoryId == 1);
                var sampleSubCategory = subcategories.FirstOrDefault();

                var result = await service.DeleteMainCategoryAsync(1);
                result.ShouldBeTrue();
                sampleSubCategory!.MainCategoryId.ShouldBe(12);
            }
        }
        [Fact]
        public async Task Updates_Main_Category()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetService(db);
                var category = new AddEditMainCategoryModal() { Id = 1, Name = "Blah blah blah" };

                var result = await service.UpdateMainCategoryAsync(category);
                result.ShouldBe(1);
                //I don't really have a wait to just get ONE so this will do for now...
                var updatedCategory = (await service.GetMainCategoriesForDropdownListAsync()).FirstOrDefault(x => x.Category == "Blah blah blah");
                updatedCategory.ShouldNotBeNull();
            }
        }
        [Fact]
        public async Task Throws_DuplicateName_Error_When_Updating()
        {
            var category = new AddEditMainCategoryModal()
            {
                Id = 1,
                Name = "Food"
            };
            await Task.Run(() => Should.Throw<DuplicateNameException>(async () => await _service.UpdateMainCategoryAsync(category)));
        }
        [Fact]
        public async Task Throws_When_Id_Is_Invalid_When_Updating()
        {
            var category = new AddEditMainCategoryModal()
            {
                Id = int.MaxValue,
                Name = "blah blah"
            };
            await Task.Run(() => Should.Throw<CategoryNotFoundException>(async () => await _service.UpdateMainCategoryAsync(category)));
        }
        private MainCategoriesService GetService(AppDbContext db)
        {
            var repo = new MainCategoriesRepository(db);
            var subCategoryRepo = new SubCategoryRepository(db);
            var incomeRepo = new IncomeRepository(db);
            return new MainCategoriesService(repo, subCategoryRepo, incomeRepo);
        }
    }
}
