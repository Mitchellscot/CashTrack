using AutoMapper;
using CashTrack.Data.Entities;
using CashTrack.Common.Exceptions;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Repositories.ExpenseRepository;
using CashTrack.Repositories.SubCategoriesRepository;
using System;
using System.Linq;
using System.Threading.Tasks;
using CashTrack.Services.Common;
using CashTrack.Models.Common;
using Microsoft.AspNetCore.Rewrite;
using System.Xml.Linq;
using CashTrack.Models.ExpenseModels;
using CashTrack.Models.MerchantModels;
using System.Collections.Generic;

namespace CashTrack.Services.SubCategoryService;

public interface ISubCategoryService
{
    Task<SubCategoryResponse> GetSubCategoriesAsync(SubCategoryRequest request);
    Task<SubCategoryDetail> GetSubCategoryDetailsAsync(int id);
    Task<int> CreateSubCategoryAsync(SubCategory request);
    Task<SubCategoryDropdownSelection[]> GetSubCategoryDropdownListAsync();
    Task<int> UpdateSubCategoryAsync(SubCategory request);
    Task<bool> DeleteSubCategoryAsync(int id);
    Task<SubCategoryEntity> GetSubCategoryByNameAsync(string name);
    Task<string[]> GetMatchingSubCategoryNamesAsync(string match);
    Task<string[]> GetAllSubCategoryNames();
}
public class SubCategoryService : ISubCategoryService
{
    private readonly ISubCategoryRepository _subCategoryRepo;
    private readonly IMapper _mapper;
    private readonly IExpenseRepository _expenseRepo;

    public SubCategoryService(IExpenseRepository expenseRepo, ISubCategoryRepository subCategoryRepo, IMapper mapper) => (_subCategoryRepo, _mapper, _expenseRepo) = (subCategoryRepo, mapper, expenseRepo);

    public async Task<SubCategoryResponse> GetSubCategoriesAsync(SubCategoryRequest request)
    {
        var categoryViewModels = await ParseCategoryQuery(request);

        var count = await _subCategoryRepo.GetCount(x => true);

        return new SubCategoryResponse(request.PageNumber, request.PageSize, count, categoryViewModels);
    }
    private async Task<SubCategoryListItem[]> ParseCategoryQuery(SubCategoryRequest request)
    {
        switch (request.Order)
        {
            case SubCategoryOrderBy.Name:
                //only pulls the first 20 from the DB instead of pulling all of them
                //this is what is run when you first load the page
                if (!request.Reversed)
                {
                    return await GetSubCategoriesFastPageLoad(request);
                }

                var categoriesByName = await GetSubCategoryListItems();

                return categoriesByName.OrderByDescending(x => x.Name).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToArray();
            case SubCategoryOrderBy.MainCategory:
                var categoriesByMainCategory = await GetSubCategoryListItems();

                return request.Reversed ? categoriesByMainCategory.OrderByDescending(x => x.MainCategoryName).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToArray() :
                    categoriesByMainCategory.OrderBy(x => x.MainCategoryName).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToArray();

            case SubCategoryOrderBy.Purchases:
                var categoriesByPurchases = await GetSubCategoryListItems();

                return request.Reversed ? categoriesByPurchases.OrderByDescending(x => x.Purchases).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToArray() :
                    categoriesByPurchases.OrderBy(x => x.Purchases).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToArray();

            case SubCategoryOrderBy.Amount:
                var categoriesByAmount = await GetSubCategoryListItems();

                return request.Reversed ? categoriesByAmount.OrderByDescending(x => x.Amount).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToArray() :
                    categoriesByAmount.OrderBy(x => x.Amount).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToArray();

            case SubCategoryOrderBy.LastPurchase:
                var categoriesByLastPurchase = await GetSubCategoryListItems();

                return request.Reversed ? categoriesByLastPurchase.OrderByDescending(x => x.LastPurchase).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToArray() :
                    categoriesByLastPurchase.OrderBy(x => x.LastPurchase).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToArray();

            case SubCategoryOrderBy.InUse:
                var categoriesByInUse = await GetSubCategoryListItems();

                return request.Reversed ? categoriesByInUse.OrderByDescending(x => x.InUse).ThenBy(x => x.Name).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToArray() :
                    categoriesByInUse.OrderBy(x => x.InUse).ThenBy(x => x.Name).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToArray();

            default: throw new ArgumentException();
        }

    }

    private async Task<SubCategoryListItem[]> GetSubCategoryListItems()
    {
        var categories = await _subCategoryRepo.FindWithExpenses(x => true);
        var expenses = categories.SelectMany(x => x.Expenses);
        var categoryListItems = expenses.GroupBy(e => e.CategoryId).Select(g =>
        {
            var results = g.Aggregate(new SubCategoryListItemAggregator(g.Key.Value, categories), (acc, e) => acc.Accumulate(e), acc => acc.Compute());
            return new SubCategoryListItem()
            {
                Id = g.Key.Value,
                Name = results.Category.Name,
                MainCategoryName = results.Category.MainCategory.Name,
                Purchases = results.Purchases,
                Amount = results.Amount,
                LastPurchase = results.LastPurchase,
                InUse = results.Category.InUse

            };
        }).ToList();
        var categoriesWithoutExpenses = categories.Where(x => x.Expenses.Count == 0).ToList();

        categoryListItems.AddRange(categoriesWithoutExpenses.Where(x => x.Name != "Uncategorized").Select(x => new SubCategoryListItem()
        {
            Id = x.Id,
            Name = x.Name,
            MainCategoryName = x.MainCategory.Name,
            Purchases = 0,
            LastPurchase = DateTime.MinValue,
            Amount = 0,
            InUse = x.InUse
        }
        ));

        return categoryListItems.ToArray();
    }
    private async Task<SubCategoryListItem[]> GetSubCategoriesFastPageLoad(SubCategoryRequest request)
    {
        var categories = await _subCategoryRepo.FindWithPaginationIncludeExpenses(x => true, request.PageNumber, request.PageSize);

        var expenses = categories.SelectMany(x => x.Expenses).ToArray();
        var categoryListItems = expenses.GroupBy(e => e.CategoryId).Select(g =>
        {
            var results = g.Aggregate(new SubCategoryListItemAggregator(g.Key.Value, categories), (acc, e) => acc.Accumulate(e), acc => acc.Compute());
            return new SubCategoryListItem()
            {
                Id = g.Key.Value,
                Name = results.Category.Name,
                MainCategoryName = results.Category.MainCategory.Name,
                Purchases = results.Purchases,
                Amount = results.Amount,
                LastPurchase = results.LastPurchase,
                InUse = results.Category.InUse

            };
        }).ToList();
        categoryListItems.AddRange(categories
            .Where(x => x.Expenses.Count == 0 && x.Name != "Uncategorized")
            .Select(x => new SubCategoryListItem()
            {
                Id = x.Id,
                Name = x.Name,
                MainCategoryName = x.MainCategory.Name,
                Purchases = 0,
                LastPurchase = DateTime.MinValue,
                Amount = 0,
                InUse = x.InUse
            }));
        return categoryListItems.OrderBy(x => x.Name).ToArray();
    }
    public async Task<int> CreateSubCategoryAsync(SubCategory request)
    {
        var categories = await _subCategoryRepo.Find(x => true);
        if (categories.Any(x => x.Name == request.Name))
            throw new DuplicateNameException(nameof(SubCategoryEntity), request.Name);

        if (request.MainCategoryId < 1)
            throw new CategoryNotFoundException("You must assign a main category to a sub category.", new Exception());

        var entity = new SubCategoryEntity()
        {
            Name = request.Name,
            InUse = request.InUse,
            MainCategoryId = request.MainCategoryId,
            Notes = request.Notes,
        };

        return await _subCategoryRepo.Create(entity);
    }
    public async Task<int> UpdateSubCategoryAsync(SubCategory request)
    {
        var categories = await _subCategoryRepo.Find(x => x.Name == request.Name);
        if (categories.Any(x => x.Id != request.Id))
            throw new DuplicateNameException(request.Name, nameof(SubCategoryEntity));

        var category = await _subCategoryRepo.FindById(request.Id.Value);
        if (category == null)
            throw new CategoryNotFoundException(request.Id.Value.ToString());

        category.Name = request.Name;
        category.Notes = request.Notes;
        category.InUse = request.InUse;
        category.MainCategoryId = request.MainCategoryId;

        return await _subCategoryRepo.Update(category);
    }
    public async Task<bool> DeleteSubCategoryAsync(int id)
    {
        var category = await _subCategoryRepo.FindById(id);
        if (category == null)
            throw new CategoryNotFoundException(id.ToString());
        if (category.Name == "Uncategorized")
            throw new Exception("You cannot delete this category. It's kind of important.");

        var expenses = await _expenseRepo.Find(x => x.CategoryId == id);
        var uncategorizeCategoryd = (await _subCategoryRepo.Find(x => x.Name == "Uncategorized")).FirstOrDefault();
        if (uncategorizeCategoryd == null)
        {
            throw new CategoryNotFoundException("You need to create a new category called 'Uncategorized' before you can delete a sub category. This will assigned all exepenses associated with this sub category to the 'Uncategorized' category.");
        }
        foreach (var expense in expenses)
        {
            expense.CategoryId = uncategorizeCategoryd.Id;
        }
        return await _subCategoryRepo.Delete(category);
    }
    public async Task<SubCategoryDetail> GetSubCategoryDetailsAsync(int id)
    {
        var category = (await _subCategoryRepo.FindWithExpenses(x => x.Id == id)).FirstOrDefault();
        if (category == null)
            throw new CategoryNotFoundException(id.ToString());
        //think on this one
        throw new NotImplementedException();
        //var merchant = await _merchantRepo.FindById(id);
        //if (merchant == null)
        //    throw new MerchantNotFoundException(id.ToString());
        //var expenses = await _expenseRepo.GetExpensesAndCategoriesByMerchantId(id);
        //if (!expenses.Any())
        //    return new MerchantDetail()
        //    {
        //        Id = merchant.Id,
        //        Name = merchant.Name,
        //        SuggestOnLookup = merchant.SuggestOnLookup,
        //        City = merchant.City,
        //        State = merchant.State,
        //        Notes = merchant.Notes,
        //        IsOnline = merchant.IsOnline,
        //        ExpenseTotals = new Totals(),
        //        MostUsedCategory = "No expenses yet.",
        //        AnnualExpenseStatistics = new List<AnnualStatistics>(),
        //        MonthlyExpenseStatistics = new List<MonthlyStatistics>(),
        //        PurchaseCategoryOccurances = new Dictionary<string, int>(),
        //        PurchaseCategoryTotals = new Dictionary<string, decimal>(),
        //        RecentExpenses = new List<ExpenseQuickView>(),
        //    };

        //var subCategories = expenses.Select(x => x.Category).Distinct().ToArray();

        //var expensesSpanMultipleYears = expenses.GroupBy(e => e.Date.Year).ToList().Count() > 1;

        //return new MerchantDetail()
        //{
        //    Id = merchant.Id,
        //    Name = merchant.Name,
        //    SuggestOnLookup = merchant.SuggestOnLookup,
        //    City = merchant.City,
        //    State = merchant.State,
        //    Notes = merchant.Notes,
        //    IsOnline = merchant.IsOnline,

        //    ExpenseTotals = expenses.Aggregate(new TotalsAggregator<ExpenseEntity>(),
        //        (acc, e) => acc.Accumulate(e),
        //        acc => acc.Compute()),

        //    MostUsedCategory = GetExpenseCategoryOccurances(subCategories, expenses).FirstOrDefault().Key,

        //    AnnualExpenseStatistics = expensesSpanMultipleYears ?
        //    AggregateUtilities<ExpenseEntity>.GetAnnualStatistics(expenses) : new List<AnnualStatistics>(),

        //    MonthlyExpenseStatistics = expensesSpanMultipleYears ?
        //    new List<MonthlyStatistics>() : AggregateUtilities<ExpenseEntity>.GetMonthlyStatistics(expenses),

        //    PurchaseCategoryOccurances = GetExpenseCategoryOccurances(subCategories, expenses),
        //    PurchaseCategoryTotals = GetExpenseCategoryTotals(subCategories, expenses),

        //    RecentExpenses = expenses.OrderByDescending(e => e.Date)
        //    .Take(9)
        //    .Select(x => new ExpenseQuickView()
        //    {
        //        Id = x.Id,
        //        Date = x.Date.Date.ToShortDateString(),
        //        Amount = x.Amount,
        //        SubCategory = x.Category == null ? "none" : x.Category.Name
        //    }).ToList()
        //};
    }

    public async Task<SubCategoryDropdownSelection[]> GetSubCategoryDropdownListAsync()
    {
        return (await _subCategoryRepo.Find(x => x.InUse == true)).Select(x => new SubCategoryDropdownSelection()
        {
            Id = x.Id,
            Category = x.Name
        }).ToArray();
    }

    public async Task<SubCategoryEntity> GetSubCategoryByNameAsync(string name)
    {
        var category = (await _subCategoryRepo.Find(x => x.Name == name)).FirstOrDefault();
        if (category == null)
            throw new CategoryNotFoundException(name);
        return category;
    }

    public async Task<string[]> GetMatchingSubCategoryNamesAsync(string match)
    {
        return (await _subCategoryRepo.Find(x => x.Name.StartsWith(match))).Select(x => x.Name).Take(10).ToArray();
    }

    public async Task<string[]> GetAllSubCategoryNames()
    {
        return (await _subCategoryRepo.Find(x => true)).Select(x => x.Name).ToArray();
    }
}

public class SubCategoryMapperProfile : Profile
{
    public SubCategoryMapperProfile()
    {
        CreateMap<SubCategoryEntity, SubCategoryListItem>()
            .ForMember(c => c.Id, o => o.MapFrom(src => src.Id))
            .ForMember(c => c.Name, o => o.MapFrom(src => src.Name))
            .ForMember(c => c.MainCategoryName, o => o.MapFrom(src => src.MainCategory.Name))
            .ForMember(c => c.Id, o => o.MapFrom(src => src.Id))
            .ReverseMap();

        CreateMap<SubCategory, SubCategoryEntity>()
            .ForMember(c => c.Id, o => o.MapFrom(src => src.Id))
            .ForMember(c => c.Name, o => o.MapFrom(src => src.Name))
            .ForMember(c => c.MainCategoryId, o => o.MapFrom(src => src.MainCategoryId))
            .ForMember(c => c.Notes, o => o.MapFrom(src => src.Notes))
            .ForMember(c => c.InUse, o => o.MapFrom(src => src.InUse))
            .ReverseMap();
    }
}