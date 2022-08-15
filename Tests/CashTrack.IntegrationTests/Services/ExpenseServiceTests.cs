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

namespace CashTrack.IntegrationTests.Services
{

    public class ExpenseServiceTests
    {
        [Fact]
        public async Task TestThisOutAgain()
        {
            //auto mapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new ExpenseMapperProfile());
            });
            var mapper = mockMapper.CreateMapper();
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var repo = new ExpenseRepository(db);
                var incomerepo = new IncomeRepository(db);
                var merchantRepo = new MerchantRepository(db);
                var subCategoryRepo = new SubCategoryRepository(db);
                var service = new ExpenseService(repo, incomerepo, merchantRepo, mapper, subCategoryRepo);

                // Act
                var result = await service.DeleteExpenseAsync(1);

                // Assert
                result.ShouldBeTrue();
            }
        }

        [Fact]
        public async Task TestThisOut()
        {
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new ExpenseMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var repo = new ExpenseRepository(db);
                var incomerepo = new IncomeRepository(db);
                var merchantRepo = new MerchantRepository(db);
                var subCategoryRepo = new SubCategoryRepository(db);
                var service = new ExpenseService(repo, incomerepo, merchantRepo, mapper, subCategoryRepo);

                // Act
                var result = await service.GetExpenseByIdAsync(1);

                // Assert
                result.Id!.Value.ShouldBe(1);
            }
        }
    }

}
