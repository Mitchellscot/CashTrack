using CashTrack.Services.ExpenseService;
using Shouldly;
using System.Threading.Tasks;
using Xunit;
using CashTrack.Repositories.ExpenseRepository;
using CashTrack.Repositories.IncomeRepository;
using CashTrack.Repositories.MerchantRepository;
using AutoMapper;
using CashTrack.Repositories.SubCategoriesRepository;

using CashTrack.IntegrationTests.Services.Common;
using CashTrack.IntegrationTests.Common;
using CashTrack.Models.ExpenseModels;
using CashTrack.Models.Common;
using System;
using System.Linq;

namespace CashTrack.IntegrationTests.Services
{

    public class ExpenseServiceTests
    {
        private readonly ExpenseService _service;
        private readonly IMapper mapper;

        public ExpenseServiceTests()
        { 
            var expenseMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new ExpenseMapperProfile());
            });
            //shared DB context should only be used for GET requests
            //anything that modifies the data should use a fresh dbcontext (see Delete_An_Expense_By_Id)
            var sharedDB = new AppDbContextFactory().CreateDbContext();
            var repo = new ExpenseRepository(sharedDB);
            var incomerepo = new IncomeRepository(sharedDB);
            var merchantRepo = new MerchantRepository(sharedDB);
            var subCategoryRepo = new SubCategoryRepository(sharedDB);
            mapper = expenseMapper.CreateMapper();
            _service = new ExpenseService(repo, incomerepo, merchantRepo, mapper, subCategoryRepo);
        }
        [Fact]
        public async Task Delete_An_Expense_By_Id()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var repo = new ExpenseRepository(db);
                var incomerepo = new IncomeRepository(db);
                var merchantRepo = new MerchantRepository(db);
                var subCategoryRepo = new SubCategoryRepository(db);
                var service = new ExpenseService(repo, incomerepo, merchantRepo, mapper, subCategoryRepo);

                var result = await service.DeleteExpenseAsync(1);

                result.ShouldBeTrue();
            }
        }

        [Theory]
        [ExpenseIdData]
        public async Task Get_Expense_By_Id(int id)
        {
            var result = await _service.GetExpenseByIdAsync(id);

            result.Id!.Value.ShouldBe(id);
        }
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        public async Task Get_Expenses_Returns_Paginated_Respose(int dateOptions)
        {
            var request = new ExpenseRequest()
            {
                DateOptions = (DateOptions)dateOptions,
                BeginDate = new DateTime(2020, 04, 24),
                EndDate = new DateTime(2021, 04, 24),
                Query = "birthday"
            };
            var result = await _service.GetExpensesAsync(request);
            result.ListItems.Count().ShouldBeGreaterThan(1);
        }
        [Fact]
        public async Task Create_An_Expense()
        {
            var expense = new Expense()
            {
                Amount = 4.24M,
                Date = new DateTime(1984, 04, 24),
                Notes = "happy birthday",
                MerchantId = 85,
                SubCategoryId = 1,
                ExcludeFromStatistics = true,
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var repo = new ExpenseRepository(db);
                var incomerepo = new IncomeRepository(db);
                var merchantRepo = new MerchantRepository(db);
                var subCategoryRepo = new SubCategoryRepository(db);
                var service = new ExpenseService(repo, incomerepo, merchantRepo, mapper, subCategoryRepo);
                var expectedId = (repo.GetCount(x => true).Result) + 1;
                var result = await service.CreateExpenseAsync(expense);

                result.ShouldBe(expectedId);
            }
        }
    }

}
