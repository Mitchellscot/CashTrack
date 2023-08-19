using CashTrack.Common.Exceptions;
using CashTrack.Data.Entities;
using CashTrack.Models.Common;
using CashTrack.Models.ImportProfileModels;
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
        Task<int> CreateImportProfileAsync(AddEditImportProfile request);
        Task<int> UpdateImportProfileAsync(AddEditImportProfile request);
        Task<bool> DeleteImportProfileAsync(int id);
        Task<List<string>> GetImportProfileNames();
    }
    public class ImportProfileService : IImportProfileService
    {
        private readonly IImportProfileRepository _repo;
        public ImportProfileService(IImportProfileRepository repo) => _repo = repo;

        public async Task<int> CreateImportProfileAsync(AddEditImportProfile request)
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

            var profile = new ImportProfileEntity()
            {
                Name = request.Name,
                DateColumnName = request.DateColumn,
                ExpenseColumnName = request.AmountColumn,
                NotesColumnName = request.NotesColumn,
                IncomeColumnName = request.IncomeColumn,
                ContainsNegativeValue = request.ContainsNegativeValue,
                NegativeValueTransactionType = request.NegativeValueTransactionType,
                DefaultTransactionType = request.DefaultTransactionType
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

        public async Task<int> UpdateImportProfileAsync(AddEditImportProfile request)
        {
            if(!request.Id.HasValue)
                throw new ArgumentException("Unable to find the import profile to update, the ID is missing.");
            var profile = await _repo.FindById(request.Id.Value);
            if(profile == null)
                throw new ImportProfileNotFoundException($"No import Profile found with an Id of{request.Id}");
            
            profile.Name = request.Name;
            profile.DateColumnName = request.DateColumn;
            profile.ExpenseColumnName = request.AmountColumn;
            profile.NotesColumnName = request.NotesColumn;
            profile.IncomeColumnName = request.IncomeColumn;
            profile.ContainsNegativeValue = request.ContainsNegativeValue;
            profile.NegativeValueTransactionType = request.NegativeValueTransactionType;
            profile.DefaultTransactionType = request.DefaultTransactionType;
            return await _repo.Update(profile);
        }
    }
}
