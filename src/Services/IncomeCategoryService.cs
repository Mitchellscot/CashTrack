using CashTrack.Data.Entities;
using CashTrack.Common.Exceptions;
using CashTrack.Models.IncomeCategoryModels;
using CashTrack.Repositories.IncomeCategoryRepository;
using System;
using System.Linq;
using System.Threading.Tasks;
using CashTrack.Repositories.IncomeRepository;
using CashTrack.Services.Common;
using CashTrack.Models.ExpenseModels;
using CashTrack.Models.SubCategoryModels;
using System.Collections.Generic;
using CashTrack.Models.MerchantModels;
using CashTrack.Common.Extensions;

namespace CashTrack.Services.IncomeCategoryService;

public interface IIncomeCategoryService
{
    Task<IncomeCategoryDetail> GetCategoryDetailAsync(int id);
    Task<IncomeCategoryResponse> GetIncomeCategoriesAsync(IncomeCategoryRequest request);
    Task<int> CreateIncomeCategoryAsync(AddEditIncomeCategoryModal request);
    Task<int> UpdateIncomeCategoryAsync(AddEditIncomeCategoryModal request);
    Task<bool> DeleteIncomeCategoryAsync(int id);
    Task<IncomeCategoryDropdownSelection[]> GetIncomeCategoryDropdownListAsync();
    Task<string[]> GetIncomeCategoryNames();
    Task<bool> CheckIfIncomeCategoryIsRefund(int categoryId);
    Task<IncomeCategoryEntity> GetIncomeCategoryByNameAsync(string name);
    Task<string[]> GetMatchingIncomeCategoryNamesAsync(string match);
}
public class IncomeCategoryService : IIncomeCategoryService
{
    private readonly IIncomeCategoryRepository _repo;
    private readonly IIncomeRepository _incomeRepo;

    public IncomeCategoryService(IIncomeCategoryRepository repo, IIncomeRepository incomeRepo) => (_repo, _incomeRepo) = (repo, incomeRepo);

    public async Task<int> CreateIncomeCategoryAsync(AddEditIncomeCategoryModal request)
    {
        if (string.IsNullOrEmpty(request.Name))
            throw new ArgumentException("Income Category must have a name");

        var categories = await _repo.Find(x => true);
        if (categories.Any(x => x.Name == request.Name))
            throw new DuplicateNameException(request.Name);
        var incomeCategoryEntity = new IncomeCategoryEntity()
        {
            Name = request.Name,
            Notes = request.Notes,
            InUse = request.InUse
        };
        return await _repo.Create(incomeCategoryEntity);
    }

    public async Task<bool> DeleteIncomeCategoryAsync(int id)
    {
        var category = await _repo.FindById(id);
        if (category == null)
            throw new CategoryNotFoundException(id.ToString());
        if (category.Name.IsEqualTo("Uncategorized"))
            throw new Exception("You cannot delete this category. It's kind of important.");

        if (category.Name.IsEqualTo("Refund"))
            throw new Exception("You cannot delete this category. It's kind of important.");

        var uncategorizedCategory = (await _repo.Find(x => x.Name == "Uncategorized")).FirstOrDefault();
        var uncategorizedId = 0;
        if (uncategorizedCategory == null)
        {
            uncategorizedId = await CreateIncomeCategoryAsync(new AddEditIncomeCategoryModal() { Name = "Uncategorized", Notes = "Default category for any income that does not have a category associated with it.", InUse = false });
        }
        else
        {
            uncategorizedId = uncategorizedCategory.Id;
        }
        var incomes = await _incomeRepo.Find(x => x.CategoryId == id);
        foreach (var income in incomes)
        {
            income.CategoryId = uncategorizedId;
        }

        return await _repo.Delete(category);
    }

    public async Task<IncomeCategoryResponse> GetIncomeCategoriesAsync(IncomeCategoryRequest request)
    {
        var categories = await _repo.FindWithPaginationIncludeIncome(x => true, request.PageNumber, request.PageSize);
        var income = categories.SelectMany(x => x.Income).ToArray();
        var categoryListItems = income.GroupBy(i => i.CategoryId).Select(g =>
        {
            var results = g.Aggregate(new IncomeCategoryListItemAggregator(g.Key.Value, categories), (acc, i) => acc.Accumulate(i), acc => acc.Compute());
            return new IncomeCategoryListItem()
            {
                Id = g.Key.Value,
                Name = results.Category.Name,
                Payments = results.Payments,
                Amount = results.Amount,
                LastPayment = results.LastPayment,
                InUse = results.Category.InUse
            };

        }).ToList();
        //adds in all the categories with no assigned income
        categoryListItems.AddRange(categories
            .Where(x => x.Income.Count == 0 && !x.Name.IsEqualTo("Uncategorized"))
            .Select(x => new IncomeCategoryListItem()
            {
                Id = x.Id,
                Name = x.Name,
                Amount = 0,
                Payments = 0,
                LastPayment = DateTime.MinValue,
                InUse = x.InUse
            }));

        var count = await _repo.GetCount(x => true);

        return new IncomeCategoryResponse(request.PageNumber,
            request.PageSize,
            count,
            categoryListItems.OrderBy(x => x.Name).ToArray());
    }

    public async Task<string[]> GetIncomeCategoryNames()
    {
        return (await _repo.Find(x => x.InUse)).Select(x => x.Name).ToArray();
    }

    public async Task<IncomeCategoryDropdownSelection[]> GetIncomeCategoryDropdownListAsync()
    {
        return (await _repo.Find(x => x.InUse == true)).Select(x => new IncomeCategoryDropdownSelection()
        {
            Id = x.Id,
            Category = x.Name
        }).ToArray();
    }

    public async Task<int> UpdateIncomeCategoryAsync(AddEditIncomeCategoryModal request)
    {
        var categories = await _repo.Find(x => x.Name == request.Name);
        if (categories.Any(x => x.Id != request.Id))
            throw new DuplicateNameException(request.Name);

        var category = (await _repo.Find(x => x.Id == request.Id.Value)).FirstOrDefault();
        if (category == null)
            throw new CategoryNotFoundException(request.Id.Value.ToString());

        category.Name = request.Name;
        category.Notes = request.Notes;
        category.InUse = request.InUse;

        return await _repo.Update(category);
    }

    public async Task<bool> CheckIfIncomeCategoryIsRefund(int categoryId)
    {
        var refundCategory = (await _repo.Find(x => x.Name == "Refund")).FirstOrDefault();
        var refundCategoryId = 0;
        if (refundCategory == null)
        {
            refundCategoryId = await CreateIncomeCategoryAsync(new AddEditIncomeCategoryModal() { Name = "Refund", InUse = true });
        }
        else
        {
            refundCategoryId = refundCategory.Id;
        }

        return refundCategoryId == categoryId;
    }

    public async Task<IncomeCategoryEntity> GetIncomeCategoryByNameAsync(string name)
    {
        var category = (await _repo.Find(x => x.Name == name)).FirstOrDefault();
        if (category == null)
            throw new CategoryNotFoundException(name);
        return category;
    }

    public async Task<string[]> GetMatchingIncomeCategoryNamesAsync(string match)
    {
        return (await _repo.Find(x => x.InUse && x.Name.ToLower().StartsWith(match.ToLower()))).Select(x => x.Name).Take(10).ToArray();
    }

    public async Task<IncomeCategoryDetail> GetCategoryDetailAsync(int id)
    {
        var category = (await _repo.FindWithIncomeAndSources(x => x.Id == id)).FirstOrDefault();
        if (category == null)
            throw new CategoryNotFoundException(id.ToString());

        if (!category.Income.Any())
            return new IncomeCategoryDetail()
            {
                Id = category.Id,
                Name = category.Name,
                InUse = category.InUse,
                Notes = category.Notes,
                IncomeTotals = new Totals(),
                AnnualIncomeStatistics = new List<AnnualStatistics>(),
                MonthlyIncomeStatistics = new List<MonthlyStatistics>(),
                RecentIncome = new List<IncomeQuickViewForCategoryDetail>(),
                SourcePurchaseOccurances = new Dictionary<string, int>(),
                SourcePurchaseTotals = new Dictionary<string, decimal>()
            };

        var sources = category.Income.Where(x => x.Source != null).Select(x => x.Source).Distinct().ToArray();
        var incomeTotals = category.Income.Aggregate(new TotalsAggregator<IncomeEntity>(),
            (acc, e) => acc.Accumulate(e),
            acc => acc.Compute());
        var annualStatistics = AggregateUtilities<IncomeEntity>.GetAnnualStatistics(category.Income.ToArray());
        var monthlyStatistics = AggregateUtilities<IncomeEntity>.GetStatisticsLast12Months(category.Income.ToArray());
        var recentIncome = category.Income.OrderByDescending(i => i.Date)
            .Take(8)
            .Select(x => new IncomeQuickViewForCategoryDetail()
            {
                Id = x.Id,
                Date = x.Date.Date.ToShortDateString(),
                Amount = x.Amount,
                Source = x.Source == null ? "" : x.Source.Name
            }).ToList();
        var sourcePurchaseOccurances = GetSourcePurchaseOccurances(sources, category.Income.ToArray());
        var sourcePurchaseTotals = GetSourcePurchaseTotals(sources, category.Income.ToArray());
        return new IncomeCategoryDetail()
        {
            Id = category.Id,
            Name = category.Name,
            InUse = category.InUse,
            Notes = category.Notes,
            IncomeTotals = incomeTotals,
            AnnualIncomeStatistics = annualStatistics,
            MonthlyIncomeStatistics = monthlyStatistics,
            RecentIncome = recentIncome,
            SourcePurchaseOccurances = sourcePurchaseOccurances,
            SourcePurchaseTotals = sourcePurchaseTotals
        };
    }

    private Dictionary<string, decimal> GetSourcePurchaseTotals(IncomeSourceEntity[] sources, IncomeEntity[] income)
    {
        var incomeWithSources = income.Where(x => x.SourceId.HasValue).ToList();
        return sources.GroupJoin(incomeWithSources,
            m => m.Id, i => i.Source.Id, (s, g) => new
            {
                Source = s.Name,
                Income = g
            }).Select(x => new
            {
                Source = x.Source,
                Sum = x.Income.Sum(i => i.Amount)
            }).Where(x => x.Sum > 0).OrderByDescending(x => x.Sum).ToDictionary(k => k.Source, v => v.Sum);
    }

    private Dictionary<string, int> GetSourcePurchaseOccurances(IncomeSourceEntity[] sources, IncomeEntity[] income)
    {
        var incomeWithSources = income.Where(x => x.SourceId.HasValue).ToList();
        return sources.GroupJoin(incomeWithSources,
        c => c.Id, i => i.Source.Id, (s, g) => new
        {
            Source = s.Name,
            Income = g
        }).Select(x => new
        {
            Source = x.Source,
            Count = x.Income.Count()
        }).OrderByDescending(x => x.Count).ToDictionary(k => k.Source, v => v.Count);
    }
}

