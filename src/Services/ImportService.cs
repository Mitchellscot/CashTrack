﻿using CashTrack.Common.Exceptions;
using CashTrack.Common.Extensions;
using CashTrack.Data.Entities;
using CashTrack.Models.Common;
using CashTrack.Models.ImportCsvModels;
using CashTrack.Models.ImportRuleModels;
using CashTrack.Repositories.ExpenseRepository;
using CashTrack.Repositories.ExpenseReviewRepository;
using CashTrack.Repositories.ImportRepository;
using CashTrack.Repositories.ImportRuleRepository;
using CashTrack.Repositories.IncomeRepository;
using CashTrack.Repositories.IncomeReviewRepository;
using CsvHelper;
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
        private readonly IImportProfileRepository _profileRepo;
        public ImportService(IIncomeReviewRepository incomeReviewRepo, IExpenseRepository expenseRepo, IIncomeRepository incomeRepo, IWebHostEnvironment env, IImportRulesRepository rulesRepo, IExpenseReviewRepository expenseReviewRepo, IImportProfileRepository profileRepo)
        {
            _incomeReviewRepo = incomeReviewRepo;
            _expenseReviewRepo = expenseReviewRepo;
            _expenseRepo = expenseRepo;
            _incomeRepo = incomeRepo;
            _env = env;
            _rulesRepo = rulesRepo;
            _profileRepo = profileRepo;
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
                imports = await GetTransactionsFromFileAsync(filePath, request.FileType, rules);
            }
            catch (ImportProfileNotFoundException)
            {
                return "Unable to find an import file profile associated with this file type.";
            }
            catch (CsvHelper.MissingFieldException)
            {
                File.Delete(filePath);
                return "Please inspect the csv file for the correct headers.";
            }
            catch (ArgumentException ex)
            {
                File.Delete(filePath);
                return $"No transactions imported - {ex.Message}";
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

        internal async Task<IEnumerable<ImportTransaction>> GetTransactionsFromFileAsync(string filePath, string fileType, ImportRuleEntity[] rules)
        {
            var profile = (await _profileRepo.Find(x => x.Name == fileType)).FirstOrDefault() ?? throw new ImportProfileNotFoundException();

            var filterRules = rules.Where(x =>
                x.FileType.IsEqualTo(fileType) &&
                x.RuleType == RuleType.Filter).ToList();

            using var reader = new StreamReader(filePath);
            ICollection<ImportTransaction> imports = new List<ImportTransaction>();
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var notes = csv.GetField<string>(profile.NotesColumnName);
                    var rule = filterRules.FirstOrDefault(x => notes.ToLower().Contains(x.Rule.ToLower()));
                    if (rule is not null)
                        continue;

                    if (!string.IsNullOrEmpty(profile.IncomeColumnName))
                    {
                        var parsedIncome = decimal.TryParse(csv.GetField<string>(profile.IncomeColumnName), out decimal income);
                        if (parsedIncome)
                        {
                            var incomeTransaction = new ImportTransaction()
                            {
                                Date = csv.GetField<DateTime>(profile.DateColumnName),
                                Amount = income,
                                IsIncome = true,
                                Notes = notes
                            };
                            imports.Add(incomeTransaction);
                            continue;
                        }
                    }
                    var parsedAmount = decimal.TryParse(csv.GetField<string>(profile.ExpenseColumnName), out decimal amount);
                    if (!parsedAmount)
                        throw new ArgumentException($"Unable to determine the amount from a row in column named: {profile.ExpenseColumnName}");
                    
                    var isIncome = false;
                    if (profile.ContainsNegativeValue.Value == true &&
                        profile.NegativeValueTransactionType == TransactionType.Expense)
                    {
                        isIncome = amount > 0;
                    }
                    else if (profile.ContainsNegativeValue.Value == true &&
                        profile.NegativeValueTransactionType == TransactionType.Income)
                    {
                        isIncome = amount < 0;
                    }
                    else if (profile.ContainsNegativeValue.Value == false && profile.DefaultTransactionType == TransactionType.Income)
                    {
                        isIncome = true;
                    }

                    var transaction = new ImportTransaction()
                    {
                        Date = csv.GetField<DateTime>(profile.DateColumnName),
                        Amount = amount,
                        IsIncome = isIncome, 
                        Notes = notes
                    };
                    imports.Add(transaction);
                }
            }
            File.Delete(filePath);
            return imports;
        }

        internal IEnumerable<ImportTransaction> SetImportRules(IEnumerable<ImportTransaction> imports, ImportRuleEntity[] rules)
        {
            var expenseImports = imports.Where(x => !x.IsIncome).ToList();
            var expenseRules = rules.Where(x => x.TransactionType == TransactionType.Expense && x.RuleType == RuleType.Assignment).ToList();
            foreach (var import in expenseImports)
            {
                var notes = import.Notes.ToLower();
                if (string.IsNullOrEmpty(notes))
                    continue;

                var rule = expenseRules.FirstOrDefault(x => notes.Contains(x.Rule.ToLower()));

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
                var notes = import.Notes.ToLower();
                if (string.IsNullOrEmpty(notes))
                    continue;

                var rule = incomeRules.FirstOrDefault(x => notes.Contains(x.Rule.ToLower()));

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
}