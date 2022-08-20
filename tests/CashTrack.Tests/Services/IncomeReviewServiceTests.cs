using AutoMapper;
using CashTrack.Data.Entities;
using CashTrack.Models.IncomeReviewModels;
using CashTrack.Repositories.IncomeReviewRepository;
using CashTrack.Services.IncomeReviewService;
using CashTrack.Tests.Services.Common;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CashTrack.Tests.Services
{
    public class IncomeReviewServiceTests
    {
        private readonly IMapper _mapper;
        private readonly IncomeReviewService _service;

        public IncomeReviewServiceTests()
        {
            var IncomeMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new IncomeReviewMapper());
            });
            _mapper = IncomeMapper.CreateMapper();
            var db = new AppDbContextFactory().CreateDbContext();
            var repo = new IncomeReviewRepository(db);
            var data = GetData();
            foreach (var Income in data)
            {
                repo.Create(Income).GetAwaiter();
            }
            _service = new IncomeReviewService(repo, _mapper);
        }
        [Fact]
        public async Task Get_By_Id()
        {
            var result = await _service.GetIncomeReviewByIdAsync(1);
            result.Notes.ShouldBe("rent");
        }
        [Fact]
        public async Task Get_Count_Of_Non_Reviewed_Incomes()
        {
            var result = await _service.GetCountOfIncomeReviews();
            result.ShouldBe(3);
        }
        [Fact]
        public async Task Get_Paginated_Incomes()
        {
            var result = await _service.GetIncomeReviewsAsync(new IncomeReviewRequest());
            result.PageNumber.ShouldBe(1);
            result.ListItems.Count().ShouldBe(3);
            result.PageSize.ShouldBe(20);
            result.TotalCount.ShouldBe(3);
            result.TotalPages.ShouldBe(1);
        }
        [Fact]
        public async Task Set_Income_To_Ignore()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var repo = new IncomeReviewRepository(db);
                var service = new IncomeReviewService(repo, _mapper);
                var IncomeId = await repo.Create(new IncomeReviewEntity() { Date = DateTime.Today, Amount = 5m, IsReviewed = false });

                var result = await service.SetIncomeReviewToIgnoreAsync(IncomeId);
                result.ShouldBe(IncomeId);

            }
        }
        private List<IncomeReviewEntity> GetData()
        {
            return new List<IncomeReviewEntity>()
            {
                new IncomeReviewEntity()
                {
                    Id = 1,
                    Date = DateTime.Today.AddDays(-3),
                    Amount = 5,
                    Notes = "rent",
                    IsReviewed = false
                },
                new IncomeReviewEntity()
                {
                    Id = 2,
                    Date = DateTime.Today.AddDays(-2),
                    Amount = 15,
                    Notes = "food",
                    IsReviewed = false
                },
                new IncomeReviewEntity()
                {
                    Id = 3,
                    Date = DateTime.Today.AddDays(-1),
                    Amount = 25,
                    Notes = "gas",
                    IsReviewed = false
                },
                new IncomeReviewEntity()
                {
                    Id = 4,
                    Date = DateTime.Today.AddDays(-4),
                    Amount = 35,
                    Notes = "seen it",
                    IsReviewed = true
                },
            };
        }
    }

}
