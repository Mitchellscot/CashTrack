using AutoMapper;
using CashTrack.Common.Exceptions;
using CashTrack.Data;
using CashTrack.Models.IncomeSourceModels;
using CashTrack.Repositories.IncomeCategoryRepository;
using CashTrack.Repositories.IncomeRepository;
using CashTrack.Repositories.IncomeSourceRepository;
using CashTrack.Services.IncomeSourceService;
using CashTrack.Tests.Services.Common;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CashTrack.Tests.Services
{
    public class SourceServiceTests
    {
        private readonly IncomeSourceService _service;

        public SourceServiceTests()
        {
            var sharedDb = new AppDbContextFactory().CreateDbContext();
            var repo = new IncomeSourceRepository(sharedDb);
            var incomeRepo = new IncomeRepository(sharedDb);
            _service = new IncomeSourceService(repo, incomeRepo);
        }
        [Fact]
        public async Task Get_All_Source_Names()
        {
            var result = await _service.GetAllIncomeSourceNames();
            result.Length.ShouldBe(14);
        }
        [Theory]
        [InlineData("Parents")]
        [InlineData("SCT Tips")]
        [InlineData("BZT Tips")]
        public async Task Get_All_Sources_By_Name(string source)
        {
            var result = await _service.GetIncomeSourceByName(source);
            result.Name.ShouldBe(source);
        }
        [Fact]
        public async Task Throws_When_Name_Is_Invalid()
        {
            await Task.Run(() => Should.Throw<IncomeSourceNotFoundException>(async () => await _service.GetIncomeSourceByName("abcdefghijklmnopqrstuvwxyz")));
        }
        [Theory]
        [InlineData("Parents")]
        [InlineData("SCT Tips")]
        [InlineData("BZT Tips")]
        public async Task Get_All_Matching_Sources(string source)
        {
            var result = await _service.GetMatchingIncomeSourcesAsync(source);
            result[0].ShouldBe(source);
        }
        [Fact]
        public async Task Deletes_A_Source()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetSourceService(db);
                var result = await service.DeleteIncomeSourceAsync(1);
                result.ShouldBeTrue();
                await Task.Run(() => Should.Throw<IncomeSourceNotFoundException>(async () =>
                await service.DeleteIncomeSourceAsync(1)));
            }
        }
        [Fact]
        public async Task Deletes_A_Source_Unassigns_Income()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetSourceService(db);
                var incomeRepo = new IncomeRepository(db);
                var result = await service.DeleteIncomeSourceAsync(3);
                result.ShouldBeTrue();
                var unassignedIncome = await incomeRepo.Find(x => x.Category!.Name == "Tip");
                unassignedIncome!.FirstOrDefault()!.SourceId.ShouldBeNull();
            }
        }
        [Fact]
        public async Task Updates_A_Source()
        {
            var source = new IncomeSource()
            {
                Id = 1,
                Name = "Microsoft",
                Notes = "updated",
                City = "Seattle",
                State = "WA",
                SuggestOnLookup = true
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetSourceService(db);
                var updatedSourceId = await service.UpdateIncomeSourceAsync(source);
                updatedSourceId.ShouldBe(1);
                var updatedSource = await service.GetIncomeSourceByName("Microsoft");
                updatedSource.Notes.ShouldBe("updated");
            }
        }
        [Fact]
        public async Task Throws_On_Duplicate_Name_If_Updating()
        {
            var source = new IncomeSource()
            {
                Id = 2,
                Name = "Parents",
                Notes = "updated",
                City = "Long Beach",
                State = "CA",
                SuggestOnLookup = true
            };

            await Task.Run(() => Should.Throw<DuplicateNameException>(async () => await _service.UpdateIncomeSourceAsync(source)));
        }
        [Fact]
        public async Task Throws_On_Duplicate_Name_If_Creating()
        {
            var source = new IncomeSource()
            {
                Name = "Parents",
                Notes = "created",
                City = "Long Beach",
                State = "CA",
                SuggestOnLookup = true
            };

            await Task.Run(() => Should.Throw<DuplicateNameException>(async () => await _service.CreateIncomeSourceAsync(source)));
        }
        [Fact]
        public async Task Create_Source_Works()
        {
            var source = new IncomeSource()
            {
                Name = "Scott's Labor Leasing",
                Notes = "created",
                City = "Long Beach",
                State = "CA"
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetSourceService(db);
                var repo = new IncomeSourceRepository(db);
                var lastId = (await repo.GetCount(x => true) + 1);
                var created = await service.CreateIncomeSourceAsync(source);
                created.ShouldBe(lastId);
            }
        }
        [Fact]
        public async Task Get_Source_List_Items()
        {
            var result = await _service.GetIncomeSourcesAsync(new IncomeSourceRequest());
            result.TotalCount.ShouldBe(9);
            result.PageNumber.ShouldBe(1);
            result.PageSize.ShouldBe(20);
            var items = result.ListItems.ToArray();
            foreach (var item in items)
            {
                item.Name.ShouldNotBeNullOrEmpty();
                item.Amount.ShouldBeGreaterThan(0);
                item.Category.ShouldNotBeNullOrEmpty();
                item.LastPayment.ShouldBeGreaterThan(new DateTime(2011, 12, 31));
                item.Payments.ShouldBeGreaterThan(0);
            }
        }
        [Fact]
        public async Task Get_Source_Detail()
        {
            var result = await _service.GetIncomeSourceDetailAsync(3);
            result.Name.ShouldNotBeNullOrEmpty();
            result.IncomeTotals.Average.ShouldBeGreaterThan(0);
            result.IncomeTotals.Count.ShouldBeGreaterThan(0);
            result.IncomeTotals.Max.ShouldBeGreaterThan(0);
            result.IncomeTotals.Min.ShouldBeGreaterThan(0);
            result.IncomeTotals.TotalSpentAllTime.ShouldBeGreaterThan(0);
            result.MostUsedCategory.ShouldNotBeNullOrEmpty();
            result.AnnualIncomeStatistcis.ShouldNotBeEmpty();
            result.PaymentCategoryOcurances.ShouldNotBeEmpty();
            result.PaymentCategoryTotals.ShouldNotBeEmpty();
            result.RecentIncomes.ShouldNotBeEmpty();
        }
        [Fact]
        public async Task Gets_Income_Category_Totals()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var repo = new IncomeSourceRepository(db);
                var incomeCategoryRepo = new IncomeCategoryRepository(db);
                var incomeRepo = new IncomeRepository(db);
                var sourceService = new IncomeSourceService(repo, incomeRepo);
                var categories = await incomeCategoryRepo.Find(x => true);
                var income = await incomeRepo.Find(x => true);
                var result = sourceService.GetIncomeCategoryTotals(categories, income);
                result.Count.ShouldBe(8);
                var giftTotal = result.Where(x => x.Key == "Gift").Select(x => x.Value).FirstOrDefault();
                giftTotal.ShouldBe(390);
            }
        }
        [Fact]
        public async Task Gets_Income_Category_Occurances()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var repo = new IncomeSourceRepository(db);
                var incomeCategoryRepo = new IncomeCategoryRepository(db);
                var incomeRepo = new IncomeRepository(db);
                var sourceService = new IncomeSourceService(repo, incomeRepo);
                var categories = await incomeCategoryRepo.Find(x => true);
                var income = await incomeRepo.Find(x => true);
                var result = sourceService.GetIncomeCategoryOccurances(categories, income);
                result.Count.ShouldBe(11);
                var tipTotal = result.Where(x => x.Key == "Tip").Select(x => x.Value).FirstOrDefault();
                tipTotal.ShouldBe(172);
            }
        }
        private IncomeSourceService GetSourceService(AppDbContext db)
        {
            var repo = new IncomeSourceRepository(db);
            var incomeRepo = new IncomeRepository(db);
            var service = new IncomeSourceService(repo, incomeRepo);
            return service;
        }
    }
}
