using CashTrack.Data;
using CashTrack.Data.Entities;
using CashTrack.Models.ExportModels;
using CashTrack.Models.ImportRuleModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace CashTrack.Repositories.ExportRepository;
public interface IExportRepository
{
    Task<BudgetExport[]> GetBudgets();
    Task<ExpenseExport[]> GetExpenses();
    Task<ImportRuleExport[]> GetImportRules();
    Task<IncomeExport[]> GetIncome();
    Task<IncomeCategoryExport[]> GetIncomeCategories();
    Task<IncomeSourceExport[]> GetIncomeSources();
    Task<MainCategoryExport[]> GetMainCategories();
    Task<MerchantExport[]> GetMerchants();
    Task<SubCategoryExport[]> GetSubCategories();
    Task<ReadableBudgetExport[]> ReadableBudgetExport();
    Task<ReadableExpenseExport[]> GetReadableExpenses();
    Task<ReadableImportRuleExport[]> GetReadableImportRules();
    Task<ReadableIncomeExport[]> GetReadableIncome();
    Task<ReadableIncomeCategoryExport[]> GetReadableIncomeCategories();
    Task<ReadableIncomeSourceExport[]> GetReadableIncomeSources();
    Task<ReadableMainCategoryExport[]> GetReadableMainCategories();
    Task<ReadableMerchantExport[]> GetReadableMerchants();
    Task<ReadableSubCategoryExport[]> GetReadableSubCategories();
}
public class ExportRepository : IExportRepository
{
    private readonly AppDbContext _ctx;
    public ExportRepository(AppDbContext ctx) => _ctx = ctx;
    public async Task<BudgetExport[]> GetBudgets()
    {
        try
        {
            IQueryable<BudgetEntity> query = _ctx.Budgets.Include(x => x.SubCategory);
            query = query.OrderBy(x => x.Id);
            var budgetEntities = await query.ToArrayAsync();
            if (!budgetEntities.Any())
            {
                return new BudgetExport[] { };
            }
            return budgetEntities.Select(x => new BudgetExport(
                Id: x.Id.ToString(),
                Month: @CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Month),
                Year: x.Year.ToString(),
                Amount: x.Amount.ToString(),
                SubCategoryId: x.SubCategoryId?.ToString(),
                BudgetType: x.BudgetType.ToString()
                )).ToArray();
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<ExpenseExport[]> GetExpenses()
    {
        try
        {
            IQueryable<ExpenseEntity> query = _ctx.Expenses.Include(x => x.Merchant).Include(x => x.Category);
            query = query.OrderByDescending(x => x.Date);
            var expenseEntities = await query.ToArrayAsync();
            if (!expenseEntities.Any())
            {
                return new ExpenseExport[] { };
            }
            return expenseEntities.Select(x => new ExpenseExport(
                Id: x.Id.ToString(),
                Date: x.Date.ToShortDateString(),
                Amount: x.Amount.ToString(),
                CategoryId: x.Category.Id.ToString(),
                MerchantId: x.Merchant?.Id.ToString(),
                Notes: x.Notes,
                ExcludeFromStatistics: x.ExcludeFromStatistics ? "1" : "0",
                RefundNotes: x.RefundNotes
                )).ToArray();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ImportRuleExport[]> GetImportRules()
    {
        try
        {
            var importRuleEntities = await _ctx.ImportRules.OrderBy(x => x.Id).ToArrayAsync();
            if (!importRuleEntities.Any())
            {
                return new ImportRuleExport[] { };
            }

            return importRuleEntities.Select(x => new ImportRuleExport(
                Id: x.Id.ToString(),
                RuleType: x.RuleType,
                FileType: x.FileType,
                TransactionType: x.TransactionType,
                Rule: x.Rule,
                CategoryId: x.CategoryId.ToString(),
                MerchantSourceId: x.MerchantSourceId?.ToString()
                )).ToArray();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IncomeExport[]> GetIncome()
    {
        try
        {
            IQueryable<IncomeEntity> query = _ctx.Incomes.Include(x => x.Source).Include(x => x.Category);
            query = query.OrderByDescending(x => x.Date);
            var incomeEntities = await query.ToArrayAsync();
            if (!incomeEntities.Any())
            {
                return new IncomeExport[] { };
            }
            return incomeEntities.Select(x => new IncomeExport(
                Id: x.Id.ToString(),
                Date: x.Date.ToShortDateString(),
                Amount: x.Amount.ToString(),
                CategoryId: x.Category.Id.ToString(),
                SourceId: x.Source?.Id.ToString(),
                Notes: x.Notes,
                IsRefund: x.IsRefund ? "1" : "0",
                RefundNotes: x.RefundNotes
                )).ToArray();
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<IncomeCategoryExport[]> GetIncomeCategories()
    {
        try
        {
            var incomeCategoryEntities = await _ctx.IncomeCategories.OrderBy(x => x.Id).ToArrayAsync();
            if (!incomeCategoryEntities.Any())
            {
                return new IncomeCategoryExport[] { };
            }

            return incomeCategoryEntities.Select(x => new IncomeCategoryExport(
                Id: x.Id.ToString(),
                Name: x.Name,
                InUse: x.InUse ? "1" : "0",
                Notes: x.Notes
                )).ToArray();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IncomeSourceExport[]> GetIncomeSources()
    {
        try
        {
            var incomeSourceEntities = await _ctx.IncomeSources.OrderBy(x => x.Id).ToArrayAsync();
            if (!incomeSourceEntities.Any())
            {
                return new IncomeSourceExport[] { };
            }

            return incomeSourceEntities.Select(x => new IncomeSourceExport(
                Id: x.Id.ToString(),
                Name: x.Name,
                SuggestOnLookup: x.SuggestOnLookup ? "1" : "0",
                City: x.City,
                State: x.State,
                IsOnline: x.IsOnline ? "1" : "0",
                Notes: x.Notes
                )).ToArray();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<MainCategoryExport[]> GetMainCategories()
    {
        try
        {
            var mainCategoryEntities = await _ctx.MainCategories.OrderBy(x => x.Id).ToArrayAsync();
            if (!mainCategoryEntities.Any())
            {
                return new MainCategoryExport[] { };
            }
            return mainCategoryEntities.Select(x => new MainCategoryExport(x.Id.ToString(), x.Name)).ToArray();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<MerchantExport[]> GetMerchants()
    {
        try
        {
            var merchantEntities = await _ctx.Merchants.OrderBy(x => x.Id).ToArrayAsync();
            if (!merchantEntities.Any())
            {
                return new MerchantExport[] { };
            }

            return merchantEntities.Select(x => new MerchantExport(
                Id: x.Id.ToString(),
                Name: x.Name,
                SuggestOnLookup: x.SuggestOnLookup ? "1" : "0",
                City: x.City,
                State: x.State,
                IsOnline: x.IsOnline ? "1" : "0",
                Notes: x.Notes
                )).ToArray();
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<SubCategoryExport[]> GetSubCategories()
    {
        try
        {
            var subCategoryEntities = await _ctx.SubCategories.OrderBy(x => x.Id).ToArrayAsync();
            if (!subCategoryEntities.Any())
            {
                return new SubCategoryExport[] { };
            }

            return subCategoryEntities.Select(x => new SubCategoryExport(
                Id: x.Id.ToString(),
                Name: x.Name,
                MainCategoryId: x.MainCategoryId.ToString(),
                InUse: x.InUse ? "1" : "0",
                Notes: x.Notes
                )).ToArray();
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<ReadableBudgetExport[]> ReadableBudgetExport()
    {
        try
        {
            IQueryable<BudgetEntity> query = _ctx.Budgets.Include(x => x.SubCategory).ThenInclude(x => x.MainCategory);
            query = query.OrderByDescending(x => x.Year).ThenByDescending(x => x.Month);
            var budgetEntities = await query.ToArrayAsync();
            if (!budgetEntities.Any())
            {
                return new ReadableBudgetExport[] { };
            }
            return budgetEntities.Select(x => new ReadableBudgetExport(
                Month: @CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Month),
                Year: x.Year.ToString(),
                Amount: x.Amount.ToString(),
                SubCategory: x.SubCategory?.Name,
                MainCategory: x.SubCategory?.MainCategory.Name,
                BudgetType: x.BudgetType
                )).ToArray();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ReadableExpenseExport[]> GetReadableExpenses()
    {
        try
        {
            IQueryable<ExpenseEntity> query = _ctx.Expenses.Include(x => x.Merchant).Include(x => x.Category);
            query = query.OrderByDescending(x => x.Date);
            var expenseEntities = await query.ToArrayAsync();
            if (!expenseEntities.Any())
            {
                return new ReadableExpenseExport[] { };
            }
            return expenseEntities.Select(x => new ReadableExpenseExport(
                Date: x.Date.ToShortDateString(),
                Amount: x.Amount.ToString(),
                Category: x.Category.Name,
                Merchant: x.Merchant?.Name,
                Notes: x.Notes + x.RefundNotes ?? ""
                )).ToArray();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ReadableIncomeExport[]> GetReadableIncome()
    {
        try
        {
            IQueryable<IncomeEntity> query = _ctx.Incomes.Include(x => x.Source).Include(x => x.Category);
            query = query.OrderBy(x => x.Date);
            var incomeEntities = await query.ToArrayAsync();
            if (!incomeEntities.Any())
            {
                return new ReadableIncomeExport[] { };
            }
            return incomeEntities.Select(x => new ReadableIncomeExport(
                Date: x.Date.ToShortDateString(),
                Amount: x.Amount.ToString(),
                Category: x.Category.Name,
                Source: x.Source?.Name,
                Notes: x.Notes + x.RefundNotes ?? "",
                IsRefund: x.IsRefund ? "Refund" : ""
                )).ToArray();
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<ReadableIncomeCategoryExport[]> GetReadableIncomeCategories()
    {
        try
        {
            var incomeCategoryEntities = await _ctx.IncomeCategories.OrderBy(x => x.Id).ToArrayAsync();
            if (!incomeCategoryEntities.Any())
            {
                return new ReadableIncomeCategoryExport[] { };
            }

            return incomeCategoryEntities.Select(x => new ReadableIncomeCategoryExport(
                Name: x.Name,
                InUse: x.InUse ? "Yes" : "No",
                Notes: x.Notes
                )).ToArray();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ReadableIncomeSourceExport[]> GetReadableIncomeSources()
    {
        try
        {
            var incomeSourceEntities = await _ctx.IncomeSources.OrderBy(x => x.Id).ToArrayAsync();
            if (!incomeSourceEntities.Any())
            {
                return new ReadableIncomeSourceExport[] { };
            }

            return incomeSourceEntities.Select(x => new ReadableIncomeSourceExport(
                Name: x.Name,
                City: x.City,
                State: x.State,
                Notes: x.Notes
                )).ToArray();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ReadableMainCategoryExport[]> GetReadableMainCategories()
    {
        try
        {
            var mainCategoryEntities = await _ctx.MainCategories.OrderBy(x => x.Id).ToArrayAsync();
            if (!mainCategoryEntities.Any())
            {
                return new ReadableMainCategoryExport[] { };
            }
            return mainCategoryEntities.Select(x => new ReadableMainCategoryExport(x.Name)).ToArray();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ReadableMerchantExport[]> GetReadableMerchants()
    {
        try
        {
            var merchantEntities = await _ctx.Merchants.OrderBy(x => x.Id).ToArrayAsync();
            if (!merchantEntities.Any())
            {
                return new ReadableMerchantExport[] { };
            }

            return merchantEntities.Select(x => new ReadableMerchantExport(
                Name: x.Name,
                City: x.City,
                State: x.State,
                Notes: x.Notes
                )).ToArray();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ReadableSubCategoryExport[]> GetReadableSubCategories()
    {
        try
        {
            var subCategoryEntities = await _ctx.SubCategories.OrderBy(x => x.Id).Include(x => x.MainCategory).ToArrayAsync();
            if (!subCategoryEntities.Any())
            {
                return new ReadableSubCategoryExport[] { };
            }

            return subCategoryEntities.Select(x => new ReadableSubCategoryExport(
                Name: x.Name,
                MainCategory: x.MainCategory.Name,
                InUse: x.InUse ? "Yes" : "No",
                Notes: x.Notes
                )).ToArray();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ReadableImportRuleExport[]> GetReadableImportRules()
    {
        try
        {
            var importRuleEntities = await _ctx.ImportRules.OrderBy(x => x.Id).ToArrayAsync();
            if (!importRuleEntities.Any())
            {
                return new ReadableImportRuleExport[] { };
            }

            var importRulesWithoutAssignmentData = importRuleEntities.Select(x => new ReadableImportRuleExport()
            {
                Id = x.Id.ToString(),
                RuleType = x.RuleType,
                FileType = x.FileType,
                TransactionType = x.TransactionType,
                Rule = x.Rule,
                MerchantSource = x.MerchantSourceId.ToString(),
                Category = x.CategoryId.ToString()
            }
            ).ToList();

            var importRules = new List<ReadableImportRuleExport>();

            foreach (var rule in importRulesWithoutAssignmentData)
            {
                if (rule.RuleType == RuleType.Assignment &&
                    rule.TransactionType == TransactionType.Expense &&
                    !string.IsNullOrEmpty(rule.MerchantSource))
                {
                    rule.MerchantSource = (await _ctx.Merchants.Where(x => x.Id ==
                    Convert.ToInt32(rule.MerchantSource)).FirstOrDefaultAsync()).Name;
                }
                if (rule.RuleType == RuleType.Assignment &&
                    rule.TransactionType == TransactionType.Expense &&
                    !string.IsNullOrEmpty(rule.Category))
                {
                    rule.Category = (await _ctx.SubCategories.Where(x => x.Id ==
                    Convert.ToInt32(rule.Category)).FirstOrDefaultAsync()).Name;
                }
                if (rule.RuleType == RuleType.Assignment &&
                    rule.TransactionType == TransactionType.Income &&
                    !string.IsNullOrEmpty(rule.MerchantSource))
                {
                    rule.MerchantSource = (await _ctx.IncomeSources.Where(x => x.Id ==
                    Convert.ToInt32(rule.MerchantSource)).FirstOrDefaultAsync()).Name;
                }
                if (rule.RuleType == RuleType.Assignment &&
                    rule.TransactionType == TransactionType.Income &&
                    !string.IsNullOrEmpty(rule.Category))
                {
                    rule.Category = (await _ctx.IncomeCategories.Where(x => x.Id ==
                    Convert.ToInt32(rule.Category)).FirstOrDefaultAsync()).Name;
                }
                importRules.Add(rule);
            }
            return importRules.ToArray();
        }
        catch (Exception)
        {
            throw;
        }
    }
}