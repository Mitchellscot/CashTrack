using CashTrack.Common.Exceptions;
using CashTrack.Data;
using CashTrack.Models.ImportProfileModels;
using CashTrack.Repositories.ImportRepository;
using CashTrack.Services;
using CashTrack.Services.ImportProfileService;
using CashTrack.Tests.Services.Common;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace CashTrack.Tests.Services
{
    public class ImportProfileServiceTests
    {
        private readonly ImportProfileService _service;

        public ImportProfileServiceTests()
        {
            var db = new AppDbContextFactory().CreateDbContext();
            var repo = new ImportProfileRepository(db);
            _service = new ImportProfileService(repo);
        }
        [Fact]
        public async Task Get_All_ImportProfile()
        {
            var result = await _service.GetImportProfilesAsync();
            result.Count.ShouldBe(2);
        }
        [Fact]
        public async Task Create_Import_Profile()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var repo = new ImportProfileRepository(db);
                var service = new ImportProfileService(repo);

                var newProfile = new AddEditImportProfile()
                {
                    Name = "Test",
                    AmountColumn = "Amount",
                    DateColumn = "Date",
                    NotesColumn = "Notes",
                    IncomeColumn = "Income",
                    ContainsNegativeValue = false,
                    NegativeValueTransactionType = 0,
                    DefaultTransactionType = 0
                };
                var result = await service.CreateImportProfileAsync(newProfile);
                var verfiyResult = await repo.FindById(result);
                verfiyResult.Name.ShouldBe("Test");
            }
        }
        [Fact]
        public async Task Update_Import_Profile()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var repo = new ImportProfileRepository(db);
                var service = new ImportProfileService(repo);

                var newProfile = new AddEditImportProfile()
                {
                    Id = 1,
                    Name = "Test",
                    AmountColumn = "Amount",
                    DateColumn = "Date",
                    NotesColumn = "Notes",
                    IncomeColumn = "Income",
                    ContainsNegativeValue = false,
                    NegativeValueTransactionType = 0,
                    DefaultTransactionType = 0
                };
                var createResult = await service.CreateImportProfileAsync(newProfile);
                var verfiyCreateResult = await repo.FindById(createResult);
                var editedProfile = new AddEditImportProfile()
                {
                    Id = verfiyCreateResult.Id,
                    Name = "Test2",
                    AmountColumn = verfiyCreateResult.ExpenseColumnName,
                    DateColumn = verfiyCreateResult.DateColumnName,
                    NotesColumn = verfiyCreateResult.NotesColumnName,
                    IncomeColumn = verfiyCreateResult.IncomeColumnName,
                    ContainsNegativeValue = verfiyCreateResult.ContainsNegativeValue ?? false,
                    NegativeValueTransactionType = verfiyCreateResult.NegativeValueTransactionType ?? 0,
                    DefaultTransactionType = verfiyCreateResult.NegativeValueTransactionType ?? 0
                };
                var result = await service.UpdateImportProfileAsync(editedProfile);
                var verfiyResult = await repo.FindById(result);
                verfiyResult.Name.ShouldBe("Test2");
            }
        }
        [Fact]
        public async Task Delete_Profile_Throws_With_Invalid_Id()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetService(db);
                await Task.Run(() => Should.Throw<ImportProfileNotFoundException>(async () => await service.DeleteImportProfileAsync(int.MaxValue)));
            }
        }
        [Fact]
        public async Task Update_Profile_Throws_With_Invalid_Id()
        {
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var service = GetService(db);
                var Profile = new AddEditImportProfile()
                { 
                    Id = int.MaxValue,
                    Name = "test",
                    AmountColumn = "test",
                    DateColumn = "test",
                    NotesColumn = "test",
                    IncomeColumn = "test",
                    ContainsNegativeValue = false,
                    NegativeValueTransactionType = 0,
                    DefaultTransactionType = 0
                };
                await Task.Run(() => Should.Throw<ImportProfileNotFoundException>(async () => await service.UpdateImportProfileAsync(Profile)));
            }
        }
        private ImportProfileService GetService(AppDbContext db)
        {
            var repo = new ImportProfileRepository(db);
            return new ImportProfileService(repo);
        }
    }
}
