using AutoMapper;
using CashTrack.Common.Exceptions;
using CashTrack.Data;
using CashTrack.Models.Common;
using CashTrack.Models.ExpenseModels;
using CashTrack.Models.IncomeModels;
using CashTrack.Repositories.IncomeCategoryRepository;
using CashTrack.Repositories.IncomeRepository;
using CashTrack.Repositories.IncomeSourceRepository;
using CashTrack.Services.IncomeService;
using CashTrack.Tests.Services.Common;
using Shouldly;
using System;
using System.Linq;
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
            _service = new IncomeService(repo, sourceRepo, _mapper, categoryRepo, new TestWebHostEnvironment());
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
        [Fact]
        public async Task Get_Income_Specific_Date()
        {
            var request = new IncomeRequest()
            {
                DateOptions = DateOptions.SpecificDate,
                BeginDate = new DateTime(2012, 04, 27)
            };
            var result = await _service.GetIncomeAsync(request);
            result.TotalAmount.ShouldBe(75m);
            result.TotalCount.ShouldBe(2);
        }
        [Fact]
        public async Task Get_Income_Specific_Month_And_Year()
        {
            var request = new IncomeRequest()
            {
                DateOptions = DateOptions.SpecificMonthAndYear,
                BeginDate = new DateTime(2012, 04, 24)
            };
            var result = await _service.GetIncomeAsync(request);
            result.TotalAmount.ShouldBe(4389.24M);
            result.TotalCount.ShouldBe(21);
        }
        [Fact]
        public async Task Get_Income_Specific_Quarter()
        {
            var request = new IncomeRequest()
            {
                DateOptions = DateOptions.SpecificQuarter,
                BeginDate = new DateTime(2012, 04, 24)
            };
            var result = await _service.GetIncomeAsync(request);
            result.TotalAmount.ShouldBe(14751.98M);
            result.TotalCount.ShouldBe(61);
        }
        [Fact]
        public async Task Get_Income_Specific_Year()
        {
            var request = new IncomeRequest()
            {
                DateOptions = DateOptions.SpecificYear,
                BeginDate = new DateTime(2012, 04, 24)
            };
            var result = await _service.GetIncomeAsync(request);
            result.TotalAmount.ShouldBe(65776.36M);
            result.TotalCount.ShouldBe(225);
        }
        [Fact]
        public async Task Get_Income_Between_Two_Dates()
        {
            var request = new IncomeRequest()
            {
                DateOptions = DateOptions.DateRange,
                BeginDate = new DateTime(2012, 03, 05),
                EndDate = new DateTime(2012, 04, 24)
            };
            var result = await _service.GetIncomeAsync(request);
            result.TotalAmount.ShouldBe(8117.01M);
            result.TotalCount.ShouldBe(30);
        }
        [Fact]
        public async Task Get_Income_Last_30_Days()
        {
            var testDate = DateTime.Today.AddDays(-29);
            var income = new Income()
            {
                Amount = 4.24M,
                Date = testDate,
                Notes = "hi",
                SourceId = 5,
                CategoryId = 1,
                IsRefund = false
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetIncomeService(db);

                var createIncome = await service.CreateIncomeAsync(income);
                var request = new IncomeRequest()
                {
                    DateOptions = DateOptions.Last30Days
                };

                var result = await service.GetIncomeAsync(request);

                result.ListItems.FirstOrDefault()!.Date.Date.ShouldBe(testDate.Date);
            }
        }
        [Fact]
        public async Task Get_Income_Current_Month()
        {
            var testDate = DateTime.Today;
            var income = new Income()
            {
                Amount = 4.24M,
                Date = testDate,
                Notes = "hi",
                SourceId = 5,
                CategoryId = 1,
                IsRefund = false
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetIncomeService(db);

                var createIncome = await service.CreateIncomeAsync(income);
                var request = new IncomeRequest()
                {
                    DateOptions = DateOptions.CurrentMonth
                };

                var result = await service.GetIncomeAsync(request);

                result.ListItems.FirstOrDefault()!.Date.Month.ShouldBe(testDate.Date.Month);
            }
        }
        [Fact]
        public async Task Get_Income_Current_Quarter()
        {
            var testDate = DateTime.Today;
            var income = new Income()
            {
                Amount = 4.24M,
                Date = testDate,
                Notes = "hi",
                SourceId = 5,
                CategoryId = 1,
                IsRefund = false
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetIncomeService(db);

                var createIncome = await service.CreateIncomeAsync(income);
                var request = new IncomeRequest()
                {
                    DateOptions = DateOptions.CurrentQuarter
                };

                var result = await service.GetIncomeAsync(request);

                result.ListItems.FirstOrDefault()!.Date.Month.ShouldBe(testDate.Date.Month);
            }
        }
        [Fact]
        public async Task Get_Income_Current_Year()
        {
            var testDate = DateTime.Today;
            var income = new Income()
            {
                Amount = 4.24M,
                Date = testDate,
                Notes = "hi",
                SourceId = 5,
                CategoryId = 1,
                IsRefund = false
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetIncomeService(db);

                var createIncome = await service.CreateIncomeAsync(income);
                var request = new IncomeRequest()
                {
                    DateOptions = DateOptions.CurrentYear
                };

                var result = await service.GetIncomeAsync(request);

                result.ListItems.FirstOrDefault()!.Date.Date.ShouldBe(testDate.Date);
            }
        }
        [Fact]
        public async Task Get_Income_By_Notes()
        {
            const string testnotes = "abcdefghijklmnopqrstuvwxyz";
            var income = new Income()
            {
                Amount = 4.24M,
                Date = DateTime.Today,
                Notes = testnotes,
                SourceId = 5,
                CategoryId = 1,
                IsRefund = false
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetIncomeService(db);

                var createExpense = await service.CreateIncomeAsync(income);
                var request = new IncomeRequest()
                {
                    Query = testnotes
                };

                var result = await service.GetIncomeByNotesAsync(request);

                result.ListItems.FirstOrDefault()!.Notes.ShouldBe(testnotes);
            }
        }
        [Fact]
        public async Task Get_Income_By_Category_Id()
        {
            var request = new IncomeRequest()
            {
                Query = "7"
            };
            var result = await _service.GetIncomeByIncomeCategoryIdAsync(request);
            result.TotalAmount.ShouldBe(9672.5M);
            result.TotalCount.ShouldBe(172);
            result.ListItems.FirstOrDefault()!.CategoryId.ShouldBe(7);
        }
        [Fact]
        public async Task Get_Income_By_Source()
        {
            var request = new IncomeRequest()
            {
                Query = "SCT Tips"
            };
            var result = await _service.GetIncomeBySourceAsync(request);
            result.TotalAmount.ShouldBe(9672.5M);
            result.TotalCount.ShouldBe(172);
            result.ListItems.FirstOrDefault()!.SourceId.ShouldBe(3);
        }
        [Fact]
        public async Task Get_Income_By_Amount()
        {
            var request = new AmountSearchRequest()
            {
                Query = 20
            };
            var result = await _service.GetIncomeByAmountAsync(request);
            result.TotalAmount.ShouldBe(180m);
            result.TotalCount.ShouldBe(9);
        }
        [Fact]
        public async Task Create_An_Income()
        {
            var income = new Income()
            {
                Amount = 4.24M,
                Date = DateTime.Today,
                Notes = "new income!",
                SourceId = 5,
                CategoryId = 1,
                IsRefund = true
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var repo = new IncomeRepository(db);
                var sourceRepo = new IncomeSourceRepository(db);
                var categoryRepo = new IncomeCategoryRepository(db);
                var service = new IncomeService(repo, sourceRepo, _mapper, categoryRepo, new TestWebHostEnvironment());

                var expectedId = (repo.GetCount(x => true).Result) + 1;
                var result = await service.CreateIncomeAsync(income);
                var createdIncome = await repo.FindById(expectedId);
                //auto set refund category if IsRefund is set to true
                createdIncome.Category!.Name.ShouldBe("Refund");
                createdIncome.Id.ShouldBe(expectedId);
            }
        }
        [Fact]
        public async Task Update_An_Income()
        {
            const string testnotes = "blah blah blah";
            var income = new Income()
            {
                Id = 1,
                Amount = 4.24M,
                Date = DateTime.Today,
                Notes = testnotes,
                SourceId = 5,
                CategoryId = 1,
                IsRefund = true
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetIncomeService(db);

                var result = await service.UpdateIncomeAsync(income);
                var verifyResult = await service.GetIncomeByIdAsync(income.Id.Value);
                verifyResult.Category.ShouldBe("Refund");
                verifyResult.Notes.ShouldBe(testnotes);
            }
        }
        [Fact]
        public async Task Update_An_Income_Throws_With_No_Id()
        {
            var income = new Income()
            {
                Amount = 4.24M,
                Date = DateTime.Today,
                Notes = "blah blah blah",
                SourceId = 5,
                CategoryId = 1,
                IsRefund = true
            };
            await Task.Run(() =>
            Should.Throw<ArgumentException>(async () => await _service.UpdateIncomeAsync(income)).Message.ShouldBe("Need an id to update an income")
            );
        }
        [Fact]
        public async Task Update_An_Income_Throws_With_No_Category()
        {
            var income = new Income()
            {
                Id = 1,
                Amount = 4.24M,
                Date = DateTime.Today,
                Notes = "blah blah blah",
                SourceId = 5,
                IsRefund = false
            };
            await Task.Run(() =>
            Should.Throw<CategoryNotFoundException>(async () => await _service.UpdateIncomeAsync(income))
            );
        }
        [Fact]
        public async Task Update_An_Income_Throws_With_Invalid_Id()
        {
            var income = new Income()
            {
                Id = int.MaxValue,
                Amount = 4.24M,
                Date = DateTime.Today,
                Notes = "blah blah blah",
                SourceId = 5,
                IsRefund = false,
                CategoryId = 1
            };
            await Task.Run(() =>
            Should.Throw<IncomeNotFoundException>(async () => await _service.UpdateIncomeAsync(income))
            );
        }
        [Fact]
        public async Task Delete_An_Income_By_Id()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetIncomeService(db);

                var result = await service.DeleteIncomeAsync(1);

                result.ShouldBeTrue();
            }
        }
        [Fact]
        public async Task Delete_Income_Throws_With_Invalid_Id()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetIncomeService(db);
                await Task.Run(() => Should.Throw<IncomeNotFoundException>(async () => await _service.DeleteIncomeAsync(int.MaxValue)));
            }
        }
        private IncomeService GetIncomeService(AppDbContext db)
        {
            var repo = new IncomeRepository(db);
            var sourceRepo = new IncomeSourceRepository(db);
            var categoryRepo = new IncomeCategoryRepository(db);
            var service = new IncomeService(repo, sourceRepo, _mapper, categoryRepo, new TestWebHostEnvironment());
            return service;
        }
    }
}
