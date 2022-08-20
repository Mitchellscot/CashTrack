using AutoMapper;
using CashTrack.Data;
using CashTrack.Models.MerchantModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Repositories.ExpenseRepository;
using CashTrack.Repositories.MerchantRepository;
using CashTrack.Repositories.SubCategoriesRepository;
using CashTrack.Services.SubCategoryService;
using CashTrack.Tests.Services.Common;
using Shouldly;
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
            _service = new SubCategoryService(expenseRepo, repo, _mapper );
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
        private SubCategoryService GetCategoryService(AppDbContext db)
        {
            var repo = new SubCategoryRepository(db);
            var expenseRepo = new ExpenseRepository(db);
            return new SubCategoryService(expenseRepo, repo, _mapper);
        }
    }
}
