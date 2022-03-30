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

namespace CashTrack.Services.SubCategoryService;

public interface ISubCategoryService
{
    Task<SubCategoryResponse> GetSubCategoriesAsync(SubCategoryRequest request);
    Task<SubCategoryDetail> GetSubCategoryDetailsAsync(int id);
    Task<AddEditSubCategory> CreateSubCategoryAsync(AddEditSubCategory request);
    Task<SubCategoryDropdownSelection[]> GetSubCategoryDropdownListAsync();
    Task<int> UpdateSubCategoryAsync(AddEditSubCategory request);
    Task<bool> DeleteSubCategoryAsync(int id);
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

        var categories = await _subCategoryRepo.FindWithPagination(predicate, request.PageNumber, request.PageSize);
        var count = await _subCategoryRepo.GetCount(predicate);

        var categoryViewModels = categories.Select(c => new SubCategoryListItem
        {
            Id = c.Id,
            Name = c.Name,
            MainCategoryName = c.MainCategory.Name,
            NumberOfExpenses = (int)_expenseRepo.GetCount(x => x.CategoryId == c.Id).Result
        }).ToArray();

        return new SubCategoryResponse(request.PageNumber, request.PageSize, count, categoryViewModels);
    }
    public async Task<AddEditSubCategory> CreateSubCategoryAsync(AddEditSubCategory request)
    {
        var categories = await _subCategoryRepo.Find(x => true);
        if (categories.Any(x => x.Name == request.Name))
            throw new DuplicateNameException(nameof(SubCategoryEntity), request.Name);

        var subCategoryEntity = _mapper.Map<SubCategoryEntity>(request);

        //Here i am setting the id and then returning addedit because I'm lazy and returning the entity causes json circular reference issues.
        request.Id = subCategoryEntity.Id;

        return request;
    }
    public async Task<int> UpdateSubCategoryAsync(AddEditSubCategory request)
    {
        var checkId = await _subCategoryRepo.FindById(request.Id.Value);
        if (checkId == null)
            throw new CategoryNotFoundException(request.Id.Value.ToString());

        var nameCheck = await _subCategoryRepo.Find(x => x.Name == request.Name);
        if (nameCheck.Any())
            throw new DuplicateNameException(nameof(SubCategoryEntity), request.Name);

        var category = _mapper.Map<SubCategoryEntity>(request);
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

        CreateMap<AddEditSubCategory, SubCategoryEntity>()
            .ForMember(c => c.Id, o => o.MapFrom(src => src.Id))
            .ForMember(c => c.Name, o => o.MapFrom(src => src.Name))
            .ForMember(c => c.MainCategoryId, o => o.MapFrom(src => src.MainCategoryId))
            .ForMember(c => c.Notes, o => o.MapFrom(src => src.Notes))
            .ForMember(c => c.InUse, o => o.MapFrom(src => src.InUse))
            .ReverseMap();
    }
}