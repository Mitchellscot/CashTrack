using AutoMapper;
using CashTrack.Data;
using CashTrack.Models.Common;
using CashTrack.Models.IncomeModels;
using CashTrack.Repositories.ExpenseRepository;
using CashTrack.Repositories.IncomeCategoryRepository;
using CashTrack.Repositories.IncomeRepository;
using CashTrack.Repositories.IncomeSourceRepository;
using CashTrack.Repositories.MerchantRepository;
using CashTrack.Repositories.SubCategoriesRepository;
using CashTrack.Services.ExpenseService;
using CashTrack.Services.IncomeService;
using CashTrack.Tests.Services.Common;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CashTrack.Tests.Services
{
    public class IncomeServiceTests
    {
        private readonly IncomeService _service;
        private readonly IMapper _mapper;
        public IncomeServiceTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<IncomeMapperProfile>();
            });
            _mapper = config.CreateMapper();
            var sharedDB = new AppDbContextFactory().CreateDbContext();
            var repo = new IncomeRepository(sharedDB);
            var sourceRepo = new IncomeSourceRepository(sharedDB);
            var categoryRepo = new IncomeCategoryRepository(sharedDB);
            _service = new IncomeService(repo, sourceRepo, _mapper, categoryRepo);
        }
        [Fact]
        public async Task Get_Income_By_Id()
        {
            var _random = new Random();
            var randomNumber = _random.Next(1, 225);
            var result = await _service.GetIncomeByIdAsync(randomNumber);

            result.Id!.Value.ShouldBe(randomNumber);
        }
        [Fact]
        public async Task Get_All_Income()
        {
            var request = new IncomeRequest()
            {
                DateOptions = DateOptions.All
            };
            var result = await _service.GetIncomeAsync(request);
            result.TotalAmount.ShouldBe(65776.36M);
            result.TotalCount.ShouldBe(225);
        }

        private IncomeService GetIncomeService(AppDbContext db)
        {
            var repo = new IncomeRepository(db);
            var sourceRepo = new IncomeSourceRepository(db);
            var categoryRepo = new IncomeCategoryRepository(db);
            var service = new IncomeService(repo, sourceRepo, _mapper, categoryRepo);
            return service;
        }
    }
}
