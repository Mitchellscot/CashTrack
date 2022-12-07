using Shouldly;
using System.Threading.Tasks;
using Xunit;
using CashTrack.Repositories.ExpenseRepository;
using CashTrack.Repositories.BudgetRepository;
using CashTrack.Repositories.SubCategoriesRepository;
using CashTrack.Tests.Services.Common;
using CashTrack.Models.ExpenseModels;
using CashTrack.Models.Common;
using System;
using System.Linq;
using CashTrack.Common.Exceptions;
using CashTrack.Data;
using System.Collections.Generic;
using CashTrack.Services.BudgetService;
using CashTrack.Models.BudgetModels;

namespace CashTrack.Tests.Services
{

    public class BudgetServiceTests
    {
        private readonly BudgetService _service;
        private readonly BudgetRepository _repo;
        public BudgetServiceTests()
        {
            var sharedDB = new AppDbContextFactory().CreateDbContext();
            _repo = new BudgetRepository(sharedDB);
            var expenseRepo = new ExpenseRepository(sharedDB);
            _service = new BudgetService(_repo, expenseRepo);
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
        private BudgetService GetBudgetService(AppDbContext db)
        {
            var expenseRepo = new ExpenseRepository(db);
            var repo = new BudgetRepository(db);
            return new BudgetService(repo, expenseRepo);
        }
    }
}
