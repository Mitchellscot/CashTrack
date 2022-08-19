using CashTrack.Repositories.ExportRepository;
using CashTrack.Services.ExportService;
using CashTrack.Tests.Services.Common;
using Shouldly;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace CashTrack.Tests.Services
{
    public class ExportServiceTests
    {
        private readonly ExportService _service;

        public ExportServiceTests()
        {
            var db = new AppDbContextFactory().CreateDbContext();
            var repo = new ExportRepository(db);
            _service = new ExportService(repo);
        }
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        public async Task Exports_Raw_Data(int fileOption)
        {
            var result = await _service.ExportData(fileOption, false);
            result.ShouldNotBeNullOrWhiteSpace();
            var contents = await File.ReadAllLinesAsync(result);
            contents.ShouldNotBeEmpty();
            File.Exists(result).ShouldBeTrue();
            File.Delete(result);
        }
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        public async Task Exports_Readable_Data(int fileOption)
        {
            var result = await _service.ExportData(fileOption, true);
            result.ShouldNotBeNullOrWhiteSpace();
            var contents = await File.ReadAllLinesAsync(result);
            contents.ShouldNotBeEmpty();
            File.Exists(result).ShouldBeTrue();
            File.Delete(result);
        }
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Exports_Data_As_Zip_File(bool asReadable)
        {
            var result = await _service.ExportData(0, asReadable);
            result.ShouldNotBeNullOrWhiteSpace();
            File.Exists(result).ShouldBeTrue();
            File.Delete(result);
        }
    }
}
