using AutoMapper;
using CashTrack.Data.Entities;
using CashTrack.Models.ExpenseReviewModels;
using CashTrack.Repositories.ExpenseReviewRepository;
using CashTrack.Services.ExpenseReviewService;
using CashTrack.Tests.Services.Common;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CashTrack.Tests.Services
{
    public class ExpenseReviewServiceTests
    {
        private readonly IMapper _mapper;
        private readonly ExpenseReviewService _service;

        public ExpenseReviewServiceTests()
        {
            var expenseMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new ExpenseReviewMapper());
            });
            _mapper = expenseMapper.CreateMapper();
            var db = new AppDbContextFactory().CreateDbContext();
            var repo = new ExpenseReviewRepository(db);
            var data = GetData();
            foreach (var expense in data)
            {
                repo.Create(expense).GetAwaiter();
            }
            _service = new ExpenseReviewService(repo, _mapper);
        }
        [Fact]
        public async Task Get_By_Id()
        {
            var result = await _service.GetExpenseReviewByIdAsync(1);
            result.Notes.ShouldBe("rent");
        }
        [Fact]
        public async Task Get_Count_Of_Non_Reviewed_Expenses()
        {
            var result = await _service.GetCountOfExpenseReviews();
            result.ShouldBe(3);
        }
        [Fact]
        public async Task Get_Paginated_Expenses()
        {
            var result = await _service.GetExpenseReviewsAsync(new ExpenseReviewRequest());
            result.PageNumber.ShouldBe(1);
            result.ListItems.Count().ShouldBe(3);
            result.PageSize.ShouldBe(20);
            result.TotalCount.ShouldBe(3);
            result.TotalPages.ShouldBe(1);
        }
        [Fact]
        public async Task Set_Expense_To_Ignore()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var repo = new ExpenseReviewRepository(db);
                var service = new ExpenseReviewService(repo, _mapper);
                var expenseId = await repo.Create(new ExpenseReviewEntity() { Date = DateTime.Today, Amount = 5m, IsReviewed = false });

                var result = await service.SetExpenseReviewToIgnoreAsync(expenseId);
                result.ShouldBe(expenseId);

            }
        }
        private List<ExpenseReviewEntity> GetData()
        {
            return new List<ExpenseReviewEntity>()
            {
                new ExpenseReviewEntity()
                {
                    Id = 1,
                    Date = DateTime.Today.AddDays(-3),
                    Amount = 5,
                    Notes = "rent",
                    IsReviewed = false
                },
                new ExpenseReviewEntity()
                {
                    Id = 2,
                    Date = DateTime.Today.AddDays(-2),
                    Amount = 15,
                    Notes = "food",
                    IsReviewed = false
                },
                new ExpenseReviewEntity()
                {
                    Id = 3,
                    Date = DateTime.Today.AddDays(-1),
                    Amount = 25,
                    Notes = "gas",
                    IsReviewed = false
                },
                new ExpenseReviewEntity()
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
