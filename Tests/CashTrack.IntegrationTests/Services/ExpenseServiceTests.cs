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
using CashTrack.Common.Exceptions;
using CashTrack.Data;
using System.Collections.Generic;
using CashTrack.Models.IncomeModels;
using AngleSharp.Dom;

namespace CashTrack.IntegrationTests.Services
{

    public class ExpenseServiceTests
    {
        private readonly ExpenseService _service;
        private readonly IMapper _mapper;

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
            _mapper = expenseMapper.CreateMapper();
            _service = new ExpenseService(repo, incomerepo, merchantRepo, _mapper, subCategoryRepo);
        }
        [Fact]
        public async Task Delete_An_Expense_By_Id()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetExepenseService(db);

                var result = await service.DeleteExpenseAsync(1);

                result.ShouldBeTrue();
            }
        }
        [Fact]
        public async Task Get_Expense_By_Id()
        {
            var _random = new Random();
            var randomNumber = _random.Next(1, 462);
            var result = await _service.GetExpenseByIdAsync(randomNumber);

            result.Id!.Value.ShouldBe(randomNumber);
        }
        [Fact]
        public async Task Get_Refund_Expense_By_Id()
        {
            var _random = new Random();
            var randomNumber = _random.Next(1, 462);
            var result = await _service.GetExpenseRefundByIdAsync(randomNumber);

            result.Id.ShouldBe(randomNumber);
        }
        [Fact]
        public async Task Get_All_Expenses()
        {
            var request = new ExpenseRequest()
            {
                DateOptions = DateOptions.All,
            };
            var result = await _service.GetExpensesAsync(request);
            result.TotalAmount.ShouldBe(31543.40m);
            result.TotalCount.ShouldBe(462);
        }
        [Fact]
        public async Task Get_Expenses_Specific_Date()
        {
            var request = new ExpenseRequest()
            {
                DateOptions = DateOptions.SpecificDate,
                BeginDate = new DateTime(2012, 04, 24)
            };
            var result = await _service.GetExpensesAsync(request);
            result.TotalAmount.ShouldBe(72.98m);
            result.TotalCount.ShouldBe(2);
        }
        [Fact]
        public async Task Get_Expenses_Specific_Month_And_Year()
        {
            var request = new ExpenseRequest()
            {
                DateOptions = DateOptions.SpecificMonthAndYear,
                BeginDate = new DateTime(2012, 04, 24)
            };
            var result = await _service.GetExpensesAsync(request);
            result.TotalAmount.ShouldBe(2026.07m);
            result.TotalCount.ShouldBe(35);
        }
        [Fact]
        public async Task Get_Expenses_Specific_Quarter()
        {
            var request = new ExpenseRequest()
            {
                DateOptions = DateOptions.SpecificQuarter,
                BeginDate = new DateTime(2012, 04, 24)
            };
            var result = await _service.GetExpensesAsync(request);
            result.TotalAmount.ShouldBe(7467.72m);
            result.TotalCount.ShouldBe(115);
        }
        [Fact]
        public async Task Get_Expenses_Specific_Year()
        {
            var request = new ExpenseRequest()
            {
                DateOptions = DateOptions.SpecificYear,
                BeginDate = new DateTime(2012, 04, 24)
            };
            var result = await _service.GetExpensesAsync(request);
            result.TotalAmount.ShouldBe(31543.40m);
            result.TotalCount.ShouldBe(462);
        }
        [Fact]
        public async Task Get_Expenses_Between_Two_Dates()
        {
            var request = new ExpenseRequest()
            {
                DateOptions = DateOptions.DateRange,
                BeginDate = new DateTime(2012, 03, 05),
                EndDate = new DateTime(2012, 04, 24)
            };
            var result = await _service.GetExpensesAsync(request);
            result.TotalAmount.ShouldBe(2477.17m);
            result.TotalCount.ShouldBe(58);
        }
        [Fact]
        public async Task Get_Expenses_Last_30_Days()
        {
            var testDate = DateTime.Today.AddDays(-29);
            var expense = new Expense()
            {
                Amount = 4.24M,
                Date = testDate,
                Notes = "",
                MerchantId = 5,
                SubCategoryId = 1,
                ExcludeFromStatistics = true,
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetExepenseService(db);

                var createExpense = await service.CreateExpenseAsync(expense);
                var request = new ExpenseRequest()
                {
                    DateOptions = DateOptions.Last30Days
                };

                var result = await service.GetExpensesAsync(request);

                result.ListItems.FirstOrDefault()!.Date.Date.ShouldBe(testDate.Date);
            }
        }
        [Fact]
        public async Task Get_Expenses_Current_Month()
        {
            var testDate = DateTime.Today;
            var expense = new Expense()
            {
                Amount = 4.24M,
                Date = testDate,
                Notes = "",
                MerchantId = 5,
                SubCategoryId = 1,
                ExcludeFromStatistics = true,
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetExepenseService(db);

                var createExpense = await service.CreateExpenseAsync(expense);
                var request = new ExpenseRequest()
                {
                    DateOptions = DateOptions.CurrentMonth
                };

                var result = await service.GetExpensesAsync(request);

                result.ListItems.FirstOrDefault()!.Date.Month.ShouldBe(testDate.Date.Month);
            }
        }
        [Fact]
        public async Task Get_Expenses_Current_Quarter()
        {
            var testDate = DateTime.Today;
            var expense = new Expense()
            {
                Amount = 4.24M,
                Date = testDate,
                Notes = "",
                MerchantId = 5,
                SubCategoryId = 1,
                ExcludeFromStatistics = true,
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetExepenseService(db);

                var createExpense = await service.CreateExpenseAsync(expense);
                var request = new ExpenseRequest()
                {
                    DateOptions = DateOptions.CurrentQuarter
                };

                var result = await service.GetExpensesAsync(request);

                result.ListItems.FirstOrDefault()!.Date.Month.ShouldBe(testDate.Date.Month);
            }
        }
        [Fact]
        public async Task Get_Expenses_Current_Year()
        {
            var rando = new Random();
            var currentYear = DateTime.Today.Year;
            var testDate = new DateTime(currentYear, rando.Next(1, 12), rando.Next(1, 27));
            var expense = new Expense()
            {
                Amount = 4.24M,
                Date = testDate,
                Notes = "",
                MerchantId = 5,
                SubCategoryId = 1,
                ExcludeFromStatistics = true,
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetExepenseService(db);

                var createExpense = await service.CreateExpenseAsync(expense);
                var request = new ExpenseRequest()
                {
                    DateOptions = DateOptions.CurrentYear
                };

                var result = await service.GetExpensesAsync(request);

                result.ListItems.FirstOrDefault()!.Date.Date.ShouldBe(testDate.Date);
            }
        }
        [Fact]
        public async Task Get_Expenses_By_Notes()
        {
            const string testnotes = "abcdefghijklmnopqrstuvwxyz";
            var expense = new Expense()
            {
                Amount = 4.24M,
                Date = DateTime.Today,
                Notes = testnotes,
                MerchantId = 5,
                SubCategoryId = 1,
                ExcludeFromStatistics = true,
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetExepenseService(db);

                var createExpense = await service.CreateExpenseAsync(expense);
                var request = new ExpenseRequest()
                {
                    Query = testnotes
                };

                var result = await service.GetExpensesByNotesAsync(request);

                result.ListItems.FirstOrDefault()!.Notes.ShouldBe(testnotes);
            }
        }
        [Fact]
        public async Task Get_Expenses_By_SubCategory_Id()
        {
            var request = new ExpenseRequest()
            {
                Query = "32"
            };
            var result = await _service.GetExpensesBySubCategoryIdAsync(request);
            result.TotalAmount.ShouldBe(10200m);
            result.TotalCount.ShouldBe(12);
            result.ListItems.FirstOrDefault()!.SubCategoryId.ShouldBe(32);
        }
        [Fact]
        public async Task Get_Expenses_By_Main_Category()
        {
            var request = new ExpenseRequest()
            {
                Query = "4"
            };
            var result = await _service.GetExpensesByMainCategoryAsync(request);
            result.TotalAmount.ShouldBe(5474.54M);
            result.TotalCount.ShouldBe(225);
            result.ListItems.FirstOrDefault()!.MainCategory.ShouldBe("Food");
        }
        [Fact]
        public async Task Get_Expenses_By_Date_Without_Pagination()
        {
            var date = new DateTime(2012, 04, 24);
            var result = await _service.GetExpensesByDateWithoutPaginationAsync(date);
            result.FirstOrDefault()!.Date.Date.ShouldBe(date.Date);
            result.Sum(x => x.Amount).ShouldBe(72.98m);
            result.Count().ShouldBe(2);
        }
        [Fact]
        public async Task Can_Refund_Expenses()
        {
            var expenses = new List<ExpenseRefund>()
            {
                new ExpenseRefund()
                {
                    Id = 2,
                    OriginalAmount = 850M,
                    RefundAmount = 14m,
                    Date = new DateTime(2012, 01, 03),
                    Merchant = "CA Apartments",
                    Category = "Rent"
                },
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var repo = new ExpenseRepository(db);
                var incomerepo = new IncomeRepository(db);
                var merchantRepo = new MerchantRepository(db);
                var subCategoryRepo = new SubCategoryRepository(db);
                var service = new ExpenseService(repo, incomerepo, merchantRepo, _mapper, subCategoryRepo);

                var result = await service.RefundExpensesAsync(expenses, 86);
                result.ShouldBeTrue();

                var updatedIncome = await incomerepo.FindById(86);
                updatedIncome.RefundNotes.ShouldBe($"Applied refund for the amount of 14 to an expense on 1/3/2012. ");
                var refundDate = updatedIncome.Date.Date.ToShortDateString();
                var updatedExpense = await repo.FindById(2);
                updatedExpense.RefundNotes.ShouldBe($"Original Amount: 850 - Refunded Amount: 14 - Date Refunded: {refundDate}");
                updatedExpense.Amount.ShouldBe(836m);

            };

        }
        [Fact]
        public async Task Get_Expenses_By_Merchant()
        {
            var request = new ExpenseRequest()
            {
                Query = "Costco"
            };
            var result = await _service.GetExpensesByMerchantAsync(request);
            result.TotalAmount.ShouldBe(3617.19M);
            result.TotalCount.ShouldBe(38);
            result.ListItems.FirstOrDefault()!.MerchantId.ShouldBe(5);
        }
        [Fact]
        public async Task Get_Expenses_By_Amount()
        {
            var request = new AmountSearchRequest()
            {
                Query = 850
            };
            var result = await _service.GetExpensesByAmountAsync(request);
            result.TotalAmount.ShouldBe(10200m);
            result.TotalCount.ShouldBe(12);
        }
        [Fact]
        public async Task Create_An_Expense()
        {
            var expense = new Expense()
            {
                Amount = 4.24M,
                Date = new DateTime(2012, 04, 24),
                Notes = "hi",
                MerchantId = 5,
                SubCategoryId = 1,
                ExcludeFromStatistics = true,
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var repo = new ExpenseRepository(db);
                var incomerepo = new IncomeRepository(db);
                var merchantRepo = new MerchantRepository(db);
                var subCategoryRepo = new SubCategoryRepository(db);
                var service = new ExpenseService(repo, incomerepo, merchantRepo, _mapper, subCategoryRepo);
                var expectedId = (repo.GetCount(x => true).Result) + 1;
                var result = await service.CreateExpenseAsync(expense);

                result.ShouldBe(expectedId);
            }
        }
        [Fact]
        public async Task Create_An_Expense_From_Split()
        {
            var expense = new ExpenseSplit()
            {
                Amount = 4.24M,
                Date = new DateTime(2012, 04, 24),
                Notes = "hi",
                Merchant = "Costco",
                SubCategoryId = 1
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var repo = new ExpenseRepository(db);
                var incomerepo = new IncomeRepository(db);
                var merchantRepo = new MerchantRepository(db);
                var subCategoryRepo = new SubCategoryRepository(db);
                var service = new ExpenseService(repo, incomerepo, merchantRepo, _mapper, subCategoryRepo);
                var expectedId = (repo.GetCount(x => true).Result) + 1;
                var result = await service.CreateExpenseFromSplitAsync(expense);

                result.ShouldBe(expectedId);
            }
        }
        [Fact]
        public async Task Update_An_Expense()
        {
            const string testnotes = "blah blah blah";
            var expense = new Expense()
            {
                Id = 1,
                Amount = 4.24M,
                Date = new DateTime(1984, 04, 24),
                Notes = testnotes,
                MerchantId = 5,
                SubCategoryId = 1,
                ExcludeFromStatistics = true,
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetExepenseService(db);

                var result = await service.UpdateExpenseAsync(expense);
                var verifyResult = await service.GetExpenseByIdAsync(expense.Id.Value);
                verifyResult.Notes.ShouldBe(testnotes);
            }
        }
        [Fact]
        public async Task Update_An_Expense_Throws_With_No_Id()
        {
            var expense = new Expense()
            {
                Amount = 4.24M,
                Date = new DateTime(1984, 04, 24),
                Notes = "blah blah blah",
                MerchantId = 5,
                SubCategoryId = 1,
                ExcludeFromStatistics = true,
            };
            await Task.Run(() =>
            Should.Throw<ArgumentException>(async () => await _service.UpdateExpenseAsync(expense)).Message.ShouldBe("Need an id to update an expense")
            );
        }
        [Fact]
        public async Task Update_An_Expense_Throws_With_No_Category()
        {
            var expense = new Expense()
            {
                Id = 1,
                Amount = 4.24M,
                Date = new DateTime(1984, 04, 24),
                Notes = "blah blah blah",
                MerchantId = 5,
                ExcludeFromStatistics = true,
            };
            await Task.Run(() =>
            Should.Throw<CategoryNotFoundException>(async () => await _service.UpdateExpenseAsync(expense))
            );
        }
        [Fact]
        public async Task Update_An_Expense_Throws_With_Invalid_Id()
        {
            var expense = new Expense()
            {
                Id = int.MaxValue,
                Amount = 4.24M,
                Date = new DateTime(1984, 04, 24),
                Notes = "blah blah blah",
                MerchantId = 5,
                SubCategoryId = 1,
                ExcludeFromStatistics = true,
            };
            await Task.Run(() =>
            Should.Throw<ExpenseNotFoundException>(async () => await _service.UpdateExpenseAsync(expense))
            );
        }
        private ExpenseService GetExepenseService(AppDbContext db)
        {
            var repo = new ExpenseRepository(db);
            var incomerepo = new IncomeRepository(db);
            var merchantRepo = new MerchantRepository(db);
            var subCategoryRepo = new SubCategoryRepository(db);
            var service = new ExpenseService(repo, incomerepo, merchantRepo, _mapper, subCategoryRepo);
            return service;
        }
    }
}
