using CashTrack.Data;
using CashTrack.Repositories.ExpenseRepository;
using CashTrack.Services.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shouldly;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CashTrack.Tests.Helpers
{
    public class AggregaterTests
    {
        private readonly ExpenseRepository _repo;
        public AggregaterTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Test.json")
                .Build();
            var dbOptions = new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(config.GetConnectionString("TestDb")).Options;
            var context = new AppDbContext(dbOptions, config);

            _repo = new ExpenseRepository(context);
        }
        [Fact]
        public async Task CanAggregateExpenses()
        {
            var data = await _repo.GetExpensesAndCategories(x => x.merchantid == 85);
            var result = data.Aggregate(new ExpenseTotalsAggregator(),
                (acc, e) => acc.Accumulate(e), acc => acc.Compute());
            //test data does not contain current month or year
            result.TotalSpentAllTime.ShouldBe(56160.69m);
        }
        [Fact]
        public async Task CanAggregateExpenseStatistics()
        {
            var data = await _repo.GetExpensesAndCategories(x => x.merchantid == 85);
            var result = data.GroupBy(e => e.date.Year)
                .Select(g =>
                    {
                        var results = g.Aggregate(new ExpenseStatisticsAggregator(),
                            (acc, e) => acc.Accumulate(e),
                            acc => acc.Compute());

                        return new AnnualExpenseStatistics()
                        {
                            Year = g.Key,
                            Average = results.Average,
                            Min = results.Min,
                            Max = results.Max,
                            Total = results.Total,
                            Count = results.Count
                        };
                    }).OrderBy(x => x.Year).ToList();
            result.First().Year.ShouldBe(2012);
            result.Where(x => x.Year == 2021).SingleOrDefault()!.Total.ShouldBe(14243.34m);
        }
    }
}
