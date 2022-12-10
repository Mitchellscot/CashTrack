using Shouldly;
using System.Threading.Tasks;
using Xunit;
using CashTrack.Repositories.ExpenseRepository;
using CashTrack.Repositories.BudgetRepository;
using CashTrack.Tests.Services.Common;
using System;
using System.Linq;
using CashTrack.Common.Exceptions;
using CashTrack.Data;
using System.Collections.Generic;
using CashTrack.Services.BudgetService;
using CashTrack.Models.BudgetModels;
using AutoMapper;
using static CashTrack.Services.BudgetService.BudgetService;
using CashTrack.Services.Common;

namespace CashTrack.Tests.Services
{

    public class BudgetServiceTests
    {
        private readonly BudgetService _service;
        private readonly BudgetRepository _repo;
        private readonly IMapper _mapper;
        public BudgetServiceTests()
        {
            var budgetMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new BudgetMapperProfile());
            });
            _mapper = budgetMapper.CreateMapper();
            var sharedDB = new AppDbContextFactory().CreateDbContext();
            _repo = new BudgetRepository(sharedDB);
            var expenseRepo = new ExpenseRepository(sharedDB);
            _service = new BudgetService(_repo, expenseRepo, _mapper);
        }
        [Fact]
        public async Task Can_Get_Annual_Budget_Page()
        {
            var response = await _service.GetAnnualBudgetPageAsync(new AnnualBudgetPageRequest() { Year = 2012 });
            response.AnnualBudgetChartData.ShouldNotBeNull();
            response.AnnualSummary.ShouldNotBeNull();
            response.BudgetBreakdown.ShouldNotBeNull();
            response.MainCategoryPercentages.ShouldNotBeNull();
            response.SubCategoryPercentages.ShouldNotBeNull();
            response.TypePercentages.ShouldNotBeNull();
            response.BudgetBreakdown.ShouldNotBeNull();
        }
        [Fact]
        public async Task Can_Get_Monthly_Budget_Page()
        {
            var response = await _service.GetMonthlyBudgetPageAsync(new MonthlyBudgetPageRequest() { Year = 2012, Month = 1 });
            response.MonthlyBudgetChartData.ShouldNotBeNull();
            response.MonthlySummary.ShouldNotBeNull();
            response.BudgetBreakdown.ShouldNotBeNull();
            response.MainCategoryPercentages.ShouldNotBeNull();
            response.SubCategoryPercentages.ShouldNotBeNull();
            response.TypePercentages.ShouldNotBeNull();
            response.BudgetBreakdown.ShouldNotBeNull();
        }
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public async Task Can_Get_Budget_List_Page(int orderBy)
        {
            var response = await _service.GetBudgetListAsync(new BudgetListRequest() { Order = (BudgetOrderBy)orderBy, Reversed = false, PageNumber = 1, PageSize = 25 });
            response.ListItems.ShouldNotBeEmpty();
            response.TotalCount.ShouldBeGreaterThan(1);
            response.TotalPages.ShouldBeGreaterThan(1);
        }
        //[Fact(Skip = "Data is in the year 1999")]
        //public async Task Can_Get_Category_Averages_And_Totals()
        //{
        //    var response = await _service.GetCategoryAveragesAndTotalsAsync(1);
        //}
        [Fact]
        public async Task Can_Get_Annual_Budget_Years()
        {
            var response = await _service.GetAnnualBudgetYearsAsync();
            response.ShouldContain(2012);
        }
        [Fact]
        public async Task Update_Budget()
        {
            var request = new AddEditBudgetAllocation()
            {
                Id = 1,
                Type = BudgetType.Want,
                Year = 1999,
                Amount = 300,
                Month = 5,
                SubCategoryId = 5
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetBudgetService(db);
                var result = await service.UpdateBudgetAsync(request);
                var budget = db.Budgets.Single(x => x.Year == 1999);
                budget.Id.ShouldBe(1);
                budget.Amount.ShouldBe(300);
                budget.BudgetType.ShouldBe(BudgetType.Want);
            }
        }
        [Fact]
        public async Task Create_Expense_Budget_For_Given_Month()
        {
            var request = new AddEditBudgetAllocation()
            {
                Type = BudgetType.Need,
                Year = 1999, //test year
                Amount = 500,
                Month = 4,
                SubCategoryId = 1
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetBudgetService(db);
                var result = await service.CreateBudgetItemAsync(request);
                var budget = db.Budgets.Single(x => x.Year == 1999);
                budget.Id.ShouldBe(result);
                budget.Amount.ShouldBe(500);
            }
        }
        [Fact]
        public async Task Create_Income_Budget_For_Given_Month()
        {
            var request = new AddEditBudgetAllocation()
            {
                Type = BudgetType.Income,
                Year = 1999, //test year
                Amount = 500,
                Month = 4,
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetBudgetService(db);
                var result = await service.CreateBudgetItemAsync(request);
                var budget = db.Budgets.Single(x => x.Year == 1999);
                budget.Id.ShouldBe(result);
                budget.Amount.ShouldBe(500);
            }
        }
        [Fact]
        public async Task Create_Expense_Budget_For_Given_Month_by_Weekly_Amount()
        {
            var request = new AddEditBudgetAllocation()
            {
                Type = BudgetType.Need,
                Year = 1999, //test year
                Amount = 25,
                TimeSpan = AllocationTimeSpan.Week,
                Month = 4,
                SubCategoryId = 1
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetBudgetService(db);
                var result = await service.CreateBudgetItemAsync(request);
                var budget = db.Budgets.Single(x => x.Year == 1999);
                budget.Amount.ShouldBe(108);
            }
        }
        [Fact]
        public async Task Create_Multiple_Income_Budgets()
        {
            var request = new AddEditBudgetAllocation()
            {
                Type = BudgetType.Income,
                Year = 1999, //test year
                Amount = 12000,
                TimeSpan = AllocationTimeSpan.Year,
                Month = 0
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetBudgetService(db);
                var result = await service.CreateBudgetItemAsync(request);
                result.ShouldBe(12);
                var budgets = db.Budgets.Where(x => x.Year == 1999).ToList();

                foreach (var budget in budgets)
                {
                    budget.Amount.ShouldBe(1000);
                    budget.BudgetType.ShouldBe(BudgetType.Income);
                }
            }
        }
        [Fact]
        public async Task Create_Multiple_Expense_Budgets()
        {
            var request = new AddEditBudgetAllocation()
            {
                Type = BudgetType.Need,
                Year = 1999, //test year
                Amount = 12000,
                TimeSpan = AllocationTimeSpan.Year,
                SubCategoryId = 1,
                Month = 0
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetBudgetService(db);
                var result = await service.CreateBudgetItemAsync(request);
                result.ShouldBe(12);
                var budgets = db.Budgets.Where(x => x.Year == 1999).ToList();

                foreach (var budget in budgets)
                {
                    budget.Amount.ShouldBe(1000);
                }
            }
        }
        [Fact]
        public async Task Create_Multiple_Expense_Budgets_Every_Week_Of_The_Year()
        {
            var request = new AddEditBudgetAllocation()
            {
                Type = BudgetType.Need,
                Year = 1999, //test year
                Amount = 25,
                TimeSpan = AllocationTimeSpan.Week,
                Month = 0,
                SubCategoryId = 1
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetBudgetService(db);
                var result = await service.CreateBudgetItemAsync(request);
                result.ShouldBe(12);
                var budgets = db.Budgets.Where(x => x.Year == 1999).ToList();

                foreach (var budget in budgets)
                {
                    budget.Amount.ShouldBe(108);
                }
            }
        }
        [Fact]
        public async Task Throws_When_Amount_is_Invalid()
        {
            var request = new AddEditBudgetAllocation()
            {
                Type = BudgetType.Need,
                Year = 1999, //test year
                Amount = 0,
                Month = 13,
                SubCategoryId = 1
            };
            await Task.Run(() => Should.Throw<ArgumentOutOfRangeException>(async () => await _service.CreateBudgetItemAsync(request)));
        }
        [Fact]
        public async Task Throws_When_Month_is_Invalid()
        {
            var request = new AddEditBudgetAllocation()
            {
                Type = BudgetType.Need,
                Year = 1999, //test year
                Amount = 1,
                Month = int.MaxValue,
                SubCategoryId = 1
            };
            await Task.Run(() => Should.Throw<ArgumentOutOfRangeException>(async () => await _service.CreateBudgetItemAsync(request)));
        }
        [Fact]
        public async Task Throws_When_Category_is_Missing_On_Expense()
        {
            var request = new AddEditBudgetAllocation()
            {
                Type = BudgetType.Need,
                Year = 1999, //test year
                Amount = 1,
                Month = int.MaxValue
            };
            await Task.Run(() => Should.Throw<ArgumentException>(async () => await _service.CreateBudgetItemAsync(request)));
        }
        [Fact]
        public async Task Delete_A_Budget_By_Id()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetBudgetService(db);

                var result = await service.DeleteBudgetAsync(1);

                result.ShouldBeTrue();
            }
        }
        [Fact]
        public async Task Delete_Budget_Throws_With_Invalid_Id()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetBudgetService(db);
                await Task.Run(() => Should.Throw<BudgetNotFoundException>(async () => await service.DeleteBudgetAsync(int.MaxValue)));
            }
        }
        //private method tests
        [Fact]
        public async Task Can_Get_Monthly_Expense_Data()
        {
            var budgets = await _repo.FindWithMainCategories(x => x.Year == 2012);
            var mainCategoryLabels = budgets.Where(x => x.SubCategoryId != null && x.Amount > 0).Select(x => x.SubCategory?.MainCategory.Name).OrderBy(x => x).Distinct().ToArray();
            var response = ChartUtilities.GetMonthlyBudgetExpenseData(budgets, true, true, mainCategoryLabels, true);
            response.Count.ShouldBe(mainCategoryLabels.Length + 2);
            response.Sum(x => x.DataSet.Sum()).ShouldBe(22876);
        }
        [Fact]
        public async Task Can_Get_Budget_Breakdown()
        {
            var budgets = await _repo.FindWithMainCategories(x => x.Year == 2012);
            var income = budgets.Where(x => x.BudgetType == BudgetType.Income).Sum(x => x.Amount);
            var response = _service.GetBudgetBreakdown(budgets, income);
            response.Count.ShouldBe(18);
            var x = response.Sum(x => x.Amount);
            response.Select(x => x.Amount).Sum().ShouldBe(90476);
        }

        [Fact]
        public async Task Can_Get_Main_Category_Percentages()
        {
            var budgets = await _repo.FindWithMainCategories(x => x.Year == 2012);
            var income = budgets.Where(x => x.BudgetType == BudgetType.Income).Sum(x => x.Amount);
            var response = _service.GetMainCategoryPercentages(budgets, income);
            response.Values.Count.ShouldBeGreaterThan(1);
            response.Values.Sum().ShouldBe(100);
        }
        [Fact]
        public async Task Can_Get_Sub_Category_Percentages()
        {
            var budgets = await _repo.Find(x => x.Year == 2012);
            var income = budgets.Where(x => x.BudgetType == BudgetType.Income).Sum(x => x.Amount);
            var response = _service.GetSubCategoryPercentages(budgets, income);
            response.Values.Count.ShouldBeGreaterThan(1);
            response.Values.Sum().ShouldBe(100);
        }
        [Fact]
        public async Task Can_Get_Annual_Unallocated_Data()
        {
            var budgets = await _repo.Find(x => x.Year == 2012);
            var response = _service.GetAnnualUnallocatedData(budgets);
            response.ToList().Count.ShouldBeGreaterThan(1);
            response.Sum().ShouldBe(32724);
        }
        [Fact]
        public async Task Can_Get_Annual_Savings_Data()
        {
            var budgets = await _repo.Find(x => x.Year == 2012);
            var response = _service.GetAnnualSavingsData(budgets);
            response.ToList().Count.ShouldBeGreaterThan(1);
            response.ToList().Sum().ShouldBe(12000);
        }
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task Can_Get_Annual_Data(int budgetType)
        {
            var budgets = await _repo.Find(x => x.Year == 2012);
            var response = _service.GetAnnualData(budgets, (BudgetType)budgetType);
            response.ToList().Count.ShouldBeGreaterThan(1);
        }
        private BudgetService GetBudgetService(AppDbContext db)
        {
            var expenseRepo = new ExpenseRepository(db);
            var repo = new BudgetRepository(db);
            return new BudgetService(repo, expenseRepo, _mapper);
        }
    }
}
