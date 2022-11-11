using CashTrack.Data.Entities;
using CashTrack.Models.ImportCsvModels;
using CashTrack.Models.ImportRuleModels;
using CashTrack.Repositories.ExpenseRepository;
using CashTrack.Repositories.ExpenseReviewRepository;
using CashTrack.Repositories.ImportRuleRepository;
using CashTrack.Repositories.IncomeRepository;
using CashTrack.Repositories.IncomeReviewRepository;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CashTrack.Services.ImportService
{
    public interface IImportService
    {
        Task<string> ImportTransactions(ImportModel request);
    }
    public class ImportService : IImportService
    {
        private readonly IIncomeReviewRepository _incomeReviewRepo;
        private readonly IExpenseRepository _expenseRepo;
        private readonly IIncomeRepository _incomeRepo;
        private readonly IWebHostEnvironment _env;
        private readonly IImportRulesRepository _rulesRepo;
        private readonly IExpenseReviewRepository _expenseReviewRepo;
        public ImportService(IIncomeReviewRepository incomeReviewRepo, IExpenseRepository expenseRepo, IIncomeRepository incomeRepo, IWebHostEnvironment env, IImportRulesRepository rulesRepo, IExpenseReviewRepository expenseReviewRepo)
        {
            _incomeReviewRepo = incomeReviewRepo;
            _expenseReviewRepo = expenseReviewRepo;
            _expenseRepo = expenseRepo;
            _incomeRepo = incomeRepo;
            _env = env;
            _rulesRepo = rulesRepo;
        }
        public async Task<string> ImportTransactions(ImportModel request)
        {

            var filePath = Path.Combine(_env.ContentRootPath, request.File.FileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await request.File.CopyToAsync(fileStream);
            }
            IEnumerable<ImportTransaction> imports = new List<ImportTransaction>();
            var rules = await _rulesRepo.Find(x => true);
            try
            {
                imports = GetTransactionsFromFile(filePath, request.FileType, rules);
            }
            catch (HeaderValidationException)
            {
                File.Delete(filePath);
                return "Please inspect the csv file for the correct headers.";
            }
            catch (TypeConverterException)
            {
                File.Delete(filePath);
                return "No transactions imported - is the file formatted properly?";
            }
            if (!imports.Any())
            {
                File.Delete(filePath);
                return "No transactions imported";
            }


            IEnumerable<ImportTransaction> filteredImports = await FilterTransactionsInDatabase(imports);

            if (!filteredImports.Any())
                return "Transactions have already been imported.";


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

        internal async Task<IEnumerable<ImportTransaction>> FilterTransactionsInDatabase(IEnumerable<ImportTransaction> imports)
        {
            var oldestExpenseDate = imports.OrderBy(x => x.Date).FirstOrDefault().Date;
            var expenseImports = imports.Where(x => !x.IsIncome).ToList();
            var expenses = await _expenseRepo.Find(x => x.Date >= oldestExpenseDate);
            var expenseReviews = await _expenseReviewRepo.Find(x => x.Date >= oldestExpenseDate);
            var expenseImportsNotAlreadyinDatabase = new List<ImportTransaction>();
            foreach (var import in expenseImports)
            {
                if (!expenses.Any(x => x.Date.Date == import.Date.Date && x.Amount == import.Amount) && !expenseReviews.Any(x => x.Date.Date == import.Date.Date && x.Amount == import.Amount))
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
                if (!income.Any(x => x.Date.Date == import.Date.Date && x.Amount == import.Amount) && !incomeReviews.Any(x => x.Date.Date == import.Date.Date && x.Amount == import.Amount))
                {
                    incomeImportsNotAlreadyinDatabase.Add(import);
                }
            }
            var importsNotInDatabase = new List<ImportTransaction>();
            importsNotInDatabase.AddRange(incomeImportsNotAlreadyinDatabase);
            importsNotInDatabase.AddRange(expenseImportsNotAlreadyinDatabase);
            return importsNotInDatabase;
        }

        internal IEnumerable<ImportTransaction> GetTransactionsFromFile(string filePath, CsvFileType fileType, ImportRuleEntity[] rules)
        {
            var bankFilterRules = rules.Where(x =>
            x.FileType == CsvFileType.Bank &&
            x.RuleType == RuleType.Filter).ToList();
            var creditFilterRules = rules.Where(x =>
            x.FileType == CsvFileType.Credit &&
            x.RuleType == RuleType.Filter).ToList();
            using var reader = new StreamReader(filePath);
            var bankImports = new List<BankImport>();
            var creditImports = new List<CreditImport>();
            var otherImports = new List<OtherTransactionImport>();
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                if (fileType == CsvFileType.Bank)
                {
                    csv.Context.RegisterClassMap<BankTransactionMap>();
                    var csvResult = csv.GetRecords<BankImport>().ToList();
                    foreach (var line in csvResult)
                    {
                        var rule = bankFilterRules.FirstOrDefault(x => line.Notes.ToLower().Contains(x.Rule.ToLower()));
                        if (rule == null)
                        {
                            bankImports.Add(line);
                        }
                    }
                }
                else if (fileType == CsvFileType.Credit)
                {
                    csv.Context.RegisterClassMap<CreditTransactionMap>();
                    var csvResult = csv.GetRecords<CreditImport>().ToList();
                    foreach (var item in csvResult)
                    {
                        if (item.Credit > 0)
                        {
                            item.Amount = item.Credit.Value;
                            item.IsIncome = true;
                        }
                        if (item.Debit > 0)
                        {
                            item.Amount = item.Debit.Value;
                            item.IsIncome = false;
                        }
                        var rule = creditFilterRules.FirstOrDefault(x => item.Notes.ToLower().Contains(x.Rule.ToLower()));
                        if (rule == null)
                        {
                            creditImports.Add(item);
                        }
                    }
                }
                else if (fileType == CsvFileType.Other)
                {
                    //TODO: Figure this out
                    //might be something like
                    //giving instructions on what to name the headers
                    //and then parsing a csv file that looks like that
                    throw new NotImplementedException();
                    //otherImports = csv.GetRecords<OtherTransactionImport>().ToList();
                }
            }
            IEnumerable<ImportTransaction> imports = bankImports.Any() ? bankImports :
                creditImports.Any() ? creditImports : otherImports.Any() ? otherImports : new List<ImportTransaction>();

            File.Delete(filePath);
            return imports;

        }

        internal IEnumerable<ImportTransaction> SetImportRules(IEnumerable<ImportTransaction> imports, ImportRuleEntity[] rules)
        {
            var expenseImports = imports.Where(x => !x.IsIncome).ToList();
            var expenseRules = rules.Where(x => x.TransactionType == TransactionType.Expense && x.RuleType == RuleType.Assignment).ToList();
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
            var incomeRules = rules.Where(x => x.TransactionType == TransactionType.Income && x.RuleType == RuleType.Assignment).ToList();
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
}