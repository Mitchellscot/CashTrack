using CashTrack.Common.Exceptions;
using CashTrack.Data.Entities;
using CashTrack.Models.Common;
using CashTrack.Models.ImportProfileModels;
using CashTrack.Pages.Settings;
using CashTrack.Repositories.ImportRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashTrack.Services.ImportProfileService
{
    public interface IImportProfileService
    {
        Task<List<ImportProfileListItem>> GetImportProfilesAsync();
        Task<int> CreateImportProfileAsync(AddProfileModal request);
        Task<bool> DeleteImportProfileAsync(int id);
        Task<List<string>> GetImportProfileNames();
    }
    public class ImportProfileService : IImportProfileService
    {
        private readonly IImportProfileRepository _repo;
        public ImportProfileService(IImportProfileRepository repo) => _repo = repo;

        public async Task<int> CreateImportProfileAsync(AddProfileModal request)
        {
            if (string.IsNullOrEmpty(request.Name))
                throw new ArgumentException("Import Profile must have a name.");
            if(string.IsNullOrEmpty(request.AmountColumn))
                throw new ArgumentException("Import Profile must have an amount column.");
            if(string.IsNullOrEmpty(request.DateColumn))
                throw new ArgumentException("Import Profile must have a date column.");
            if(string.IsNullOrEmpty(request.NotesColumn))
                throw new ArgumentException("Import Profile must have a notes column.");
                
            var names = await _repo.GetProfileNames();
            if (names.Contains(request.Name))
                throw new DuplicateNameException($"There is already an import profile named {request.Name} - please chose another name.");

            var parseNegativeValue = bool.TryParse(request.ContainsNegativeValue, out bool containsNegativeValue);
            var negativeValueTransactionType = request.NegativeValueTransactionType.StartsWith('i') ? TransactionType.Income : TransactionType.Expense;
            var defaultTransactionType = request.DefaultTransactionType is not null && request.DefaultTransactionType.StartsWith('i') ? TransactionType.Income : TransactionType.Expense;
            var incomeColumnName = request.TransactionType is not null && request.TransactionType.Equals("both", System.StringComparison.InvariantCultureIgnoreCase) ? request.IncomeColumn : string.Empty;

            var profile = new ImportProfileEntity()
            {
                Name = request.Name,
                DateColumnName = request.DateColumn,
                ExpenseColumnName = request.AmountColumn,
                NotesColumnName = request.NotesColumn,
                IncomeColumnName = incomeColumnName,
                ContainsNegativeValue = parseNegativeValue && containsNegativeValue,
                NegativeValueTransactionType = negativeValueTransactionType,
                DefaultTransactionType = defaultTransactionType
            };
            return await _repo.Create(profile);
            
        }

        public async Task<bool> DeleteImportProfileAsync(int id)
        {
            var profile = await _repo.FindById(id);
            return profile == null ? throw new ImportProfileNotFoundException() : await _repo.Delete(profile);
        }

        public async Task<List<string>> GetImportProfileNames()
        {
            return await _repo.GetProfileNames();
        }

        public async Task<List<ImportProfileListItem>> GetImportProfilesAsync()
        {
            var profiles = await _repo.Find(x => true);
            return profiles.Select(x => new ImportProfileListItem() 
            {
                Id = x.Id,
                Name = x.Name,
                DateColumn = x.DateColumnName,
                AmountColumn = x.ExpenseColumnName,
                NotesColumn = x.NotesColumnName,
                IncomeColumn = x.IncomeColumnName,
                ContainsNegativeValue = x.ContainsNegativeValue ?? false,
                NegativeValueTransactionType = x.NegativeValueTransactionType ?? TransactionType.Expense,
                DefaultTransactionType = x.DefaultTransactionType ?? TransactionType.Expense
            }).ToList();
        }
    }
}
