using AutoMapper;
using CashTrack.Data.Entities;
using CashTrack.Common.Exceptions;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Repositories.ExpenseRepository;
using CashTrack.Repositories.SubCategoriesRepository;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using CashTrack.Services.Common;

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
}
public class SubCategoryService : ISubCategoryService
{
    private readonly ISubCategoryRepository _subCategoryRepo;
    private readonly IMapper _mapper;
    private readonly IExpenseRepository _expenseRepo;

    public SubCategoryService(IExpenseRepository expenseRepo, ISubCategoryRepository subCategoryRepo, IMapper mapper) => (_subCategoryRepo, _mapper, _expenseRepo) = (subCategoryRepo, mapper, expenseRepo);

    public async Task<SubCategoryResponse> GetSubCategoriesAsync(SubCategoryRequest request)
    {
        Expression<Func<SubCategoryEntity, bool>> returnAll = (SubCategoryEntity s) => true;
        Expression<Func<SubCategoryEntity, bool>> searchCategories = (SubCategoryEntity s) => s.Name.ToLower().Contains(request.Query.ToLower());

        var predicate = request.Query == null ? returnAll : searchCategories;

        var categories = await _subCategoryRepo.FindWithPaginationIncludeExpenses(predicate, request.PageNumber, request.PageSize);
        var expenses = categories.SelectMany(x => x.Expenses).ToArray();
        var count = await _subCategoryRepo.GetCount(x => true);

        var categoryViewModels = GetSubCategoryListItems(categories, expenses);

        return new SubCategoryResponse(request.PageNumber, request.PageSize, count, categoryViewModels);
    }
    private SubCategoryListItem[] GetSubCategoryListItems(SubCategoryEntity[] categories, ExpenseEntity[] expenses)
    {
        return expenses.GroupBy(e => e.CategoryId).Select(g =>
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
        }).ToArray();
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

        var category = categories.First(x => x.Id == request.Id.Value);
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

        return await _subCategoryRepo.Delete(category);
    }
    public Task<SubCategoryDetail> GetSubCategoryDetailsAsync(int id)
    {
        //think on this one
        throw new NotImplementedException();
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