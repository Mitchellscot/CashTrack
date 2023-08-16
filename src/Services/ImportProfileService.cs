using CashTrack.Models.ImportProfileModels;
using CashTrack.Repositories.ImportRepository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CashTrack.Services
{
    public interface IImportProfileService
    {
        Task<List<ImportProfileListItem>> GetImportProfilesAsync();
    }
    public class ImportProfileService : IImportProfileService
    {
        private readonly IImportProfileRepository _repo;
        public ImportProfileService(IImportProfileRepository repo) => _repo = repo;

        public Task<List<ImportProfileListItem>> GetImportProfilesAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
