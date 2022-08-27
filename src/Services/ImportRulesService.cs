using CashTrack.Data.Entities;
using CashTrack.Models.ImportRuleModels;
using CashTrack.Repositories.ImportRuleRepository;
using System.Threading.Tasks;
using System.Linq;
using CashTrack.Repositories.MerchantRepository;
using CashTrack.Repositories.IncomeSourceRepository;
using CashTrack.Repositories.SubCategoriesRepository;
using CashTrack.Repositories.IncomeCategoryRepository;
using System.Data;
using CashTrack.Common.Exceptions;
using CashTrack.Models.MerchantModels;
using System.Collections.Generic;
using CashTrack.Models.Common;
using System;

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
            var categoryListItems = await ParseImportRuleQuery(request);
            var count = await _repo.GetCount(x => true);
            var response = new ImportRuleResponse(request.PageNumber, request.PageSize, count, categoryListItems);
            return response;
        }
        private async Task<List<ImportRuleListItem>> ParseImportRuleQuery(ImportRuleRequest request)
        {
            var ImportRuleListItems = new List<ImportRuleListItem>();
            switch (request.OrderBy)
            {
                case ImportRuleOrderBy.RuleType:
                    var allRulesByRuleType = await GetImportRuleListItems();

                    ImportRuleListItems = request.Reversed ?
                        allRulesByRuleType.OrderByDescending(x => x.RuleType).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList() :
                        allRulesByRuleType.OrderBy(x => x.RuleType).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
                    break;
                case ImportRuleOrderBy.FileType:
                    var allRulesByFileType = await GetImportRuleListItems();

                    ImportRuleListItems = request.Reversed ?
                        allRulesByFileType.OrderByDescending(x => x.FileType).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList() :
                        allRulesByFileType.OrderBy(x => x.FileType).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
                    break;
                case ImportRuleOrderBy.TransactionType:
                    var allRulesByTransactionType = await GetImportRuleListItems();

                    ImportRuleListItems = request.Reversed ?
                        allRulesByTransactionType.OrderByDescending(x => x.TransactionType).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList() :
                        allRulesByTransactionType.OrderBy(x => x.TransactionType).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
                    break;
                case ImportRuleOrderBy.Rule:
                    var allRulesByRule = await GetImportRuleListItems();

                    ImportRuleListItems = request.Reversed ?
                        allRulesByRule.OrderByDescending(x => x.Rule).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList() :
                        allRulesByRule.OrderBy(x => x.Rule).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
                    break;
                case ImportRuleOrderBy.MerchantSource:
                    var allRulesByMerchantSource = await GetImportRuleListItems();

                    ImportRuleListItems = request.Reversed ?
                        allRulesByMerchantSource.OrderByDescending(x => x.MerchantSource).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList() :
                        allRulesByMerchantSource.OrderBy(x => x.MerchantSource).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
                    break;
                case ImportRuleOrderBy.Category:
                    var allRulesByCategory = await GetImportRuleListItems();

                    ImportRuleListItems = request.Reversed ?
                        allRulesByCategory.OrderByDescending(x => x.Category).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList() :
                        allRulesByCategory.OrderBy(x => x.Category).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
                    break;
                default: throw new ArgumentOutOfRangeException();

            }
            return ImportRuleListItems;
        }

        private async Task<List<ImportRuleListItem>> GetImportRuleListItems()
        {
            var rules = await _repo.Find(x => true);
            var merchants = await _merchantRepo.Find(x => true);
            var sources = await _sourceRepo.Find(x => true);
            var subCategories = await _subCategoryRepo.Find(x => true);
            var incomeCategories = await _incomeCategoryRepo.Find(x => true);

            return rules.Select(x => new ImportRuleListItem()
            {
                Id = x.Id,
                FileType = x.FileType.ToString(),
                RuleType = x.RuleType.ToString(),
                TransactionType = x.TransactionType.ToString(),
                Rule = x.Rule,
                MerchantSource = x.RuleType == RuleType.Assignment ?
                    x.TransactionType == TransactionType.Expense ?
                    x.MerchantSourceId.HasValue ?
                    merchants.FirstOrDefault(m => m.Id == x.MerchantSourceId.Value).Name : null :
                    x.MerchantSourceId.HasValue ?
                    sources.FirstOrDefault(s => s.Id == x.MerchantSourceId.Value).Name ?? null : null : null,
                MerchantSourceId = x.MerchantSourceId ?? null,
                Category = x.RuleType == RuleType.Assignment ?
                    x.TransactionType == TransactionType.Expense ?
                    x.CategoryId.HasValue ?
                    subCategories.FirstOrDefault(m => m.Id == x.CategoryId).Name : null :
                    x.CategoryId.HasValue ?
                    incomeCategories.FirstOrDefault(s => s.Id == x.CategoryId).Name : null : null,
                CategoryId = x.CategoryId ?? null,
            }).ToList();
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

