using AutoMapper;
using CashTrack.Common.Exceptions;
using CashTrack.Data.Entities;
using CashTrack.Models.ExpenseReviewModels;
using CashTrack.Models.ImportCsvModels;
using CashTrack.Repositories.Common;
using CashTrack.Repositories.ImportRuleRepository;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CashTrack.Services.ExpenseReviewService;

public interface IExpenseReviewService
{
    Task<ExpenseReviewListItem> GetExpenseReviewByIdAsync(int id);
    Task<ExpenseReviewResponse> GetExpenseReviewsAsync(ExpenseReviewRequest request);
    Task<int> SetExpenseReviewToIgnoreAsync(int id);
    Task<string> ImportTransactions(ImportModel request);
}

public class ExpenseReviewService : IExpenseReviewService
{
    private readonly IRepository<ExpenseReviewEntity> _repo;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _env;
    private readonly IImportRulesRepository _rulesRepo;

    public ExpenseReviewService(IRepository<ExpenseReviewEntity> repo, IMapper mapper, IWebHostEnvironment env, IImportRulesRepository rulesRepo) => (_repo, _mapper, _env, _rulesRepo) = (repo, mapper, env, rulesRepo);

    public async Task<ExpenseReviewListItem> GetExpenseReviewByIdAsync(int id)
    {
        var singleExpense = await _repo.FindById(id);
        return _mapper.Map<ExpenseReviewListItem>(singleExpense);
    }

    public async Task<ExpenseReviewResponse> GetExpenseReviewsAsync(ExpenseReviewRequest request)
    {
        var expenses = await _repo.FindWithPagination(x => true, request.PageNumber, request.PageSize);
        var count = await _repo.GetCount(x => x.IsReviewed == false);

        return new ExpenseReviewResponse(request.PageNumber, request.PageSize, count, _mapper.Map<ExpenseReviewListItem[]>(expenses));
    }

    public async Task<string> ImportTransactions(ImportModel request)
    {
        var rules = await _rulesRepo.Find(x => true);
        var filePath = Path.Combine(_env.ContentRootPath, request.File.FileName);
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await request.File.CopyToAsync(fileStream);
        }

        using var reader = new StreamReader(filePath);
        var bankImports = new List<BankImport>();
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            if (request.FileType == CsvFileType.Bank)
            {
                csv.Context.RegisterClassMap<BankTransactionMap>();
                bankImports = csv.GetRecords<BankImport>().ToList();
            }
        }
        //seperate method
        var expenseBankImports = bankImports.Where(x => !x.IsIncome).ToList();
        var expenseRules = rules.Where(x => x.Transaction == "Expense").ToList();
        foreach (var import in expenseBankImports)
        {
            var rule = expenseRules.FirstOrDefault(x => import.Notes.ToLower().Contains(x.Rule.ToLower()));

            if (rule != null && rule.MerchantSourceId.HasValue)
            {
                import.MerchantSourceId = rule.MerchantSourceId.Value;
            }
            if (rule != null && rule.CategoryId.HasValue)
            {
                import.CategoryId = rule.CategoryId.Value;
            }
        }
        //seperate method
        var incomeBankImports = bankImports.Where(x => x.IsIncome).ToList();
        var incomeRules = rules.Where(x => x.Transaction == "Income").ToList();
        foreach (var import in incomeBankImports)
        {
            var rule = expenseRules.FirstOrDefault(x => import.Notes.ToLower().Contains(x.Rule.ToLower()));

            if (rule != null && rule.MerchantSourceId.HasValue)
            {
                import.MerchantSourceId = rule.MerchantSourceId.Value;
            }
            if (rule != null && rule.CategoryId.HasValue)
            {
                import.CategoryId = rule.CategoryId.Value;
            }
        }

        var x = expenseBankImports;
        return "ya";
    }

    public async Task<int> SetExpenseReviewToIgnoreAsync(int id)
    {
        var expense = await _repo.FindById(id);
        if (expense == null)
            throw new ExpenseNotFoundException(id.ToString());

        expense.IsReviewed = true;
        return await _repo.Update(expense);
    }


}
public sealed class BankTransactionMap : ClassMap<BankImport>
{
    public BankTransactionMap()
    {
        Map(x => x.Date).Name("Posting Date");
        Map(x => x.Amount).Name("Amount");
        Map(x => x.Notes).Name("Description");
        Map(x => x.IsIncome).Name("Transaction Type");
    }
}
public sealed class CreditTransactionMap : ClassMap<CreditImport>
{
    public CreditTransactionMap()
    {
        Map(x => x.Date).Name("Date");
        Map(x => x.Credit).Name("Credit").Optional();
        Map(x => x.Credit).Name("Debit").Optional();
        Map(x => x.Notes).Name("Description");
    }
}

public class ExpenseReviewMapper : Profile
{
    public ExpenseReviewMapper()
    {
        CreateMap<ExpenseReviewEntity, ExpenseReviewListItem>()
            .ForMember(x => x.Id, o => o.MapFrom(src => src.Id))
            .ForMember(x => x.Date, o => o.MapFrom(src => src.Date))
            .ForMember(x => x.Amount, o => o.MapFrom(src => src.Amount))
            .ForMember(x => x.Notes, o => o.MapFrom(src => src.Notes))
            .ForMember(x => x.SuggestedCategoryId, o => o.MapFrom(src => src.SuggestedCategory.Id))
            .ForMember(x => x.SuggestedCategory, o => o.MapFrom(src => src.SuggestedCategory.Name))
            .ForMember(x => x.SuggestedMerchantId, o => o.MapFrom(src => src.SuggestedMerchant.Id))
            .ForMember(x => x.SuggestedMerchant, o => o.MapFrom(src => src.SuggestedMerchant.Name))
            .ReverseMap();
    }
}
