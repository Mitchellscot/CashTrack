using CashTrack.Data.Entities;
using CashTrack.Models.ImportRuleModels;
using CashTrack.Repositories.ImportRuleRepository;
using System;
using System.Threading.Tasks;
using System.Linq;
using CashTrack.Repositories.MerchantRepository;
using CashTrack.Repositories.IncomeSourceRepository;
using CashTrack.Repositories.SubCategoriesRepository;
using CashTrack.Repositories.IncomeCategoryRepository;
using System.Data;
using CashTrack.Common.Exceptions;

namespace CashTrack.Services.ImportRulesService
{
    public interface IImportRulesService
    {
        Task<ImportRuleResponse> GetImportRulesAsync(ImportRuleRequest request);
        Task<int> CreateImportRuleAsync(AddEditImportRule request);
        Task<int> UpdateImportRuleAsync(AddEditImportRule request);
    }
    public class ImportRulesService : IImportRulesService
    {
        private readonly IImportRulesRepository _repo;
        private readonly IMerchantRepository _merchantRepo;
        private readonly IIncomeSourceRepository _sourceRepo;
        private readonly ISubCategoryRepository _subCategoryRepo;
        private readonly IIncomeCategoryRepository _incomeCategoryRepo;

        public ImportRulesService(IImportRulesRepository repo, IMerchantRepository merchantRepo, IIncomeSourceRepository sourceRepo, ISubCategoryRepository subCategoryRepo, IIncomeCategoryRepository incomeCategoryRepo)
        {
            _repo = repo;
            _merchantRepo = merchantRepo;
            _sourceRepo = sourceRepo;
            _subCategoryRepo = subCategoryRepo;
            _incomeCategoryRepo = incomeCategoryRepo;

        }

        public async Task<int> CreateImportRuleAsync(AddEditImportRule request)
        {
            var rule = new ImportRuleEntity()
            {
                FileType = (CsvFileType)request.FileType,
                TransactionType = (TransactionType)request.TransactionType,
                RuleType = (RuleType)request.RuleType,
                Rule = request.Rule,
                MerchantSourceId = (RuleType)request.RuleType == RuleType.Assignment ? request.MerchantSourceId.HasValue ? request.MerchantSourceId.Value : null : null,
                CategoryId = (RuleType)request.RuleType == RuleType.Assignment ? request.CategoryId.HasValue ? request.CategoryId.Value : null : null,

            };
            return await _repo.Create(rule);
        }

        public async Task<ImportRuleResponse> GetImportRulesAsync(ImportRuleRequest request)
        {
            var rules = await _repo.FindWithPagination(x => true, request.PageNumber, request.PageSize);
            var count = await _repo.GetCount(x => true);
            var merchants = await _merchantRepo.Find(x => true);
            var sources = await _sourceRepo.Find(x => true);
            var subCategories = await _subCategoryRepo.Find(x => true);
            var incomeCategories = await _incomeCategoryRepo.Find(x => true);

            var categoryListItems = rules.Select(x => new ImportRuleListItem()
            {
                Id = x.Id,
                FileType = x.FileType.ToString(),
                RuleType = x.RuleType.ToString(),
                TransactionType = x.TransactionType.ToString(),
                Rule = x.Rule,
                MerchantSource = x.RuleType == RuleType.Assignment ?
                    x.TransactionType == TransactionType.Expense ?
                    merchants.FirstOrDefault(m => m.Id == x.MerchantSourceId).Name :
                    sources.FirstOrDefault(s => s.Id == x.MerchantSourceId).Name : null,
                MerchantSourceId = x.MerchantSourceId,
                Category = x.RuleType == RuleType.Assignment ?
                    x.TransactionType == TransactionType.Expense ?
                    subCategories.FirstOrDefault(m => m.Id == x.CategoryId).Name :
                    incomeCategories.FirstOrDefault(s => s.Id == x.CategoryId).Name : null,
                CategoryId = x.CategoryId,
            }).OrderBy(x => x.RuleType).ThenBy(x => x.TransactionType).ThenBy(x => x.FileType).ToArray();

            var response = new ImportRuleResponse(request.PageNumber, request.PageSize, count, categoryListItems);

            return response;
        }

        public async Task<int> UpdateImportRuleAsync(AddEditImportRule request)
        {
            var rule = await _repo.FindById(request.Id.Value);
            if (rule == null)
                throw new ImportRuleNotFoundException($"No Import Rule found with an Id of {request.Id.Value}");


            rule.Id = request.Id.Value;
            rule.FileType = (CsvFileType)request.FileType;
            rule.TransactionType = (TransactionType)request.TransactionType;
            rule.RuleType = (RuleType)request.RuleType;
            rule.Rule = request.Rule;
            if ((RuleType)request.RuleType == RuleType.Assignment)
            {
                rule.MerchantSourceId = request.MerchantSourceId.HasValue ? request.MerchantSourceId.Value : null;
                rule.CategoryId = request.CategoryId.HasValue ? request.CategoryId.Value : null;
            }
            if ((RuleType)rule.RuleType == RuleType.Filter && rule.CategoryId.HasValue)
            {
                rule.CategoryId = null;
            }
            if ((RuleType)rule.RuleType == RuleType.Filter && rule.MerchantSourceId.HasValue)
            {
                rule.MerchantSourceId = null;
            }
            return await _repo.Update(rule);
        }
        public async Task<bool> DeleteImportRuleAsync(int id)
        {
            var rule = await _repo.FindById(id);
            if (rule == null)
                throw new ImportRuleNotFoundException($"No Import Rule found with an Id of {id}");

            return await _repo.Delete(rule);
        }
    }
}

