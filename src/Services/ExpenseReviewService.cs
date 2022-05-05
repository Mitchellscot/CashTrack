using AutoMapper;
using CashTrack.Common.Exceptions;
using CashTrack.Data.Entities;
using CashTrack.Models.ExpenseReviewModels;
using CashTrack.Models.ImportCsvModels;
using CashTrack.Repositories.Common;
using CashTrack.Repositories.ExpenseRepository;
using CashTrack.Repositories.ExpenseReviewRepository;
using CashTrack.Repositories.ImportRuleRepository;
using CashTrack.Repositories.IncomeRepository;
using CashTrack.Repositories.IncomeReviewRepository;
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
    private readonly IIncomeReviewRepository _incomeReviewRepo;
    private readonly IExpenseReviewRepository _expenseReviewRepo;
    private readonly IExpenseRepository _expenseRepo;
    private readonly IIncomeRepository _incomeRepo;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _env;
    private readonly IImportRulesRepository _rulesRepo;

    public ExpenseReviewService(IExpenseReviewRepository expenseReviewRepo, IIncomeReviewRepository incomeReviewRepo, IExpenseRepository expenseRepo, IIncomeRepository incomeRepo, IMapper mapper, IWebHostEnvironment env, IImportRulesRepository rulesRepo)
    {
        _incomeReviewRepo = incomeReviewRepo;
        _expenseReviewRepo = expenseReviewRepo;
        _expenseRepo = expenseRepo;
        _incomeRepo = incomeRepo;
        _mapper = mapper;
        _env = env;
        _rulesRepo = rulesRepo;
    }

    public async Task<ExpenseReviewListItem> GetExpenseReviewByIdAsync(int id)
    {
        var singleExpense = await _expenseReviewRepo.FindById(id);
        return _mapper.Map<ExpenseReviewListItem>(singleExpense);
    }

    public async Task<ExpenseReviewResponse> GetExpenseReviewsAsync(ExpenseReviewRequest request)
    {
        var expenses = await _expenseReviewRepo.FindWithPagination(x => true, request.PageNumber, request.PageSize);
        var count = await _expenseReviewRepo.GetCount(x => x.IsReviewed == false);

        return new ExpenseReviewResponse(request.PageNumber, request.PageSize, count, _mapper.Map<ExpenseReviewListItem[]>(expenses));
    }
    public async Task<string> ImportTransactions(ImportModel request)
    {

        var filePath = Path.Combine(_env.ContentRootPath, request.File.FileName);
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await request.File.CopyToAsync(fileStream);
        }
        IEnumerable<ImportTransaction> imports = new List<ImportTransaction>();
        try
        {
            imports = GetTransactionsFromFile(filePath, request.FileType);
        }
        catch (HeaderValidationException)
        {
            return "CSV File does not match file type.";
        }


        if (!imports.Any())
            return "No transactions imported";

        IEnumerable<ImportTransaction> filteredImports = await FilterTransactions(imports);

        if (!filteredImports.Any())
            return "Transactions have already been imported.";

        var rules = await _rulesRepo.Find(x => true);
        IEnumerable<ImportTransaction> importRulesApplied = SetImportRules(filteredImports, rules);

        var expensesToImport = importRulesApplied.Where(x => !x.IsIncome).Select(x => new ExpenseReviewEntity()
        {
            Date = x.Date,
            Amount = decimal.Round(x.Amount, 2),
            Notes = x.Notes,
            SuggestedMerchantId = x.MerchantSourceId,
            SuggestedCategoryId = x.CategoryId,
            IsReviewed = false
        }).ToList();
        var incomeToImport = importRulesApplied.Where(x => x.IsIncome).Select(x => new IncomeReviewEntity()
        {
            Date = x.Date,
            Amount = decimal.Round(x.Amount, 2),
            Notes = x.Notes,
            SuggestedSourceId = x.MerchantSourceId,
            SuggestedCategoryId = x.CategoryId,
            IsReviewed = false
        }).ToList();
        var expensesAdded = await _expenseReviewRepo.AddMany(expensesToImport);
        var incomeAdded = await _incomeReviewRepo.AddMany(incomeToImport);

        var results = "";

        if (expensesAdded > 0)
            results += $"Added {expensesAdded} Expenses";

        if (expensesAdded > 0 && incomeAdded > 0)
            results += " And ";

        if (incomeAdded > 0)
            results += $"Added {incomeAdded} Income";

        return results;
    }

    private async Task<IEnumerable<ImportTransaction>> FilterTransactions(IEnumerable<ImportTransaction> imports)
    {
        var oldestExpenseDate = imports.OrderBy(x => x.Date).FirstOrDefault().Date;
        var expenseImports = imports.Where(x => !x.IsIncome).ToList();
        var expenses = await _expenseRepo.Find(x => x.Date >= oldestExpenseDate);
        var expenseReviews = await _expenseReviewRepo.Find(x => x.Date >= oldestExpenseDate);
        var expenseImportsNotAlreadyinDatabase = new List<ImportTransaction>();
        foreach (var import in expenseImports)
        {
            if (!expenses.Any(x => x.Date == import.Date && x.Amount == import.Amount) || !expenseReviews.Any(x => x.Date == import.Date && x.Amount == import.Amount))
            {
                expenseImportsNotAlreadyinDatabase.Add(import);
            }
        }
        var oldestIncomeDate = imports.OrderBy(x => x.Date).FirstOrDefault().Date;
        var incomeImports = imports.Where(x => x.IsIncome).ToList();
        var income = await _incomeRepo.Find(x => x.Date >= oldestIncomeDate);
        var incomeReviews = await _incomeReviewRepo.Find(x => x.Date >= oldestIncomeDate);
        var incomeImportsNotAlreadyinDatabase = new List<ImportTransaction>();
        foreach (var import in incomeImports)
        {
            if (!income.Any(x => x.Date == import.Date && x.Amount == import.Amount) || !incomeReviews.Any(x => x.Date == import.Date && x.Amount == import.Amount))
            {
                incomeImportsNotAlreadyinDatabase.Add(import);
            }
        }
        var importsNotInDatabase = new List<ImportTransaction>();
        importsNotInDatabase.AddRange(incomeImportsNotAlreadyinDatabase);
        importsNotInDatabase.AddRange(expenseImportsNotAlreadyinDatabase);
        return importsNotInDatabase;
    }

    private IEnumerable<ImportTransaction> GetTransactionsFromFile(string filePath, CsvFileType fileType)
    {
        using var reader = new StreamReader(filePath);
        var bankImports = new List<BankImport>();
        var creditImports = new List<CreditImport>();
        var otherImports = new List<OtherTransactionImport>();
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            if (fileType == CsvFileType.Bank)
            {
                csv.Context.RegisterClassMap<BankTransactionMap>();
                bankImports = csv.GetRecords<BankImport>().ToList();
            }
            else if (fileType == CsvFileType.Credit)
            {
                csv.Context.RegisterClassMap<CreditTransactionMap>();
                creditImports = csv.GetRecords<CreditImport>().ToList();
            }
            else if (fileType == CsvFileType.Other)
            {
                otherImports = csv.GetRecords<OtherTransactionImport>().ToList();
            }
        }
        IEnumerable<ImportTransaction> imports = bankImports.Any() ? bankImports :
            creditImports.Any() ? creditImports : otherImports.Any() ? otherImports : new List<ImportTransaction>();

        //remove unnecessary credit transactions
        imports = imports.Where(x => !x.Notes.Contains("thank you"));


        File.Delete(filePath);
        return imports;

    }

    private IEnumerable<ImportTransaction> SetImportRules(IEnumerable<ImportTransaction> imports, ImportRuleEntity[] rules)
    {
        var expenseImports = imports.Where(x => !x.IsIncome).ToList();
        var expenseRules = rules.Where(x => x.Transaction == "Expense").ToList();
        foreach (var import in expenseImports)
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
        var incomeImports = imports.Where(x => x.IsIncome).ToList();
        var incomeRules = rules.Where(x => x.Transaction == "Income").ToList();
        foreach (var import in incomeImports)
        {
            var rule = incomeRules.FirstOrDefault(x => import.Notes.ToLower().Contains(x.Rule.ToLower()));

            if (rule != null && rule.MerchantSourceId.HasValue)
            {
                import.MerchantSourceId = rule.MerchantSourceId.Value;
            }
            if (rule != null && rule.CategoryId.HasValue)
            {
                import.CategoryId = rule.CategoryId.Value;
            }
        }
        var setImports = new List<ImportTransaction>();
        setImports.AddRange(incomeImports);
        setImports.AddRange(expenseImports);
        return setImports;
    }


    public async Task<int> SetExpenseReviewToIgnoreAsync(int id)
    {
        var expense = await _expenseReviewRepo.FindById(id);
        if (expense == null)
            throw new ExpenseNotFoundException(id.ToString());

        expense.IsReviewed = true;
        return await _expenseReviewRepo.Update(expense);
    }
}
public sealed class BankTransactionMap : ClassMap<BankImport>
{
    public BankTransactionMap()
    {
        Map(x => x.Date).Name("Posting Date");
        Map(x => x.Amount).Name("Amount");
        Map(x => x.Notes).Name("Description");
    }
}
public sealed class CreditTransactionMap : ClassMap<CreditImport>
{
    public CreditTransactionMap()
    {
        Map(x => x.Date).Name("Date");
        Map(x => x.Credit).Name("Credit").Optional();
        Map(x => x.Debit).Name("Debit").Optional();
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
