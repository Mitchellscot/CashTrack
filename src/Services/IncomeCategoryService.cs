using AutoMapper;
using CashTrack.Data.Entities;
using CashTrack.Common.Exceptions;
using CashTrack.Models.IncomeCategoryModels;
using CashTrack.Repositories.IncomeCategoryRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CashTrack.Services.IncomeCategoryService;

public interface IIncomeCategoryService
{
    Task<IncomeCategoryResponse> GetIncomeCategoriesAsync(IncomeCategoryRequest request);
    Task<AddEditIncomeCategory> CreateIncomeCategoryAsync(AddEditIncomeCategory request);
    Task<int> UpdateIncomeCategoryAsync(AddEditIncomeCategory request);
    Task<bool> DeleteIncomeCategoryAsync(int id);
    Task<IncomeCategoryDropdownSelection[]> GetIncomeCategoryDropdownListAsync();
    Task<string[]> GetIncomeCategoryNames();
    Task<bool> CheckIfIncomeCategoryIsRefund(int categoryId);
}
public class IncomeCategoryService : IIncomeCategoryService
{
    private readonly IIncomeCategoryRepository _repo;
    private readonly IMapper _mapper;

    public IncomeCategoryService(IIncomeCategoryRepository repo, IMapper mapper) => (_repo, _mapper) = (repo, mapper);

    public async Task<AddEditIncomeCategory> CreateIncomeCategoryAsync(AddEditIncomeCategory request)
    {
        var categories = await _repo.Find(x => true);
        if (categories.Any(x => x.Name == request.Name))
            throw new DuplicateNameException(nameof(IncomeCategoryEntity), request.Name);
        //this obviously needs to be rewritten
        var incomeCategoryEntity = _mapper.Map<IncomeCategoryEntity>(request);
        incomeCategoryEntity.Id = await _repo.GetCount(x => true) + 1;

        request.Id = incomeCategoryEntity.Id;
        return request;
    }

    public async Task<bool> DeleteIncomeCategoryAsync(int id)
    {
        var category = await _repo.FindById(id);
        if (category == null)
            throw new CategoryNotFoundException(id.ToString());

        return await _repo.Delete(category);
    }

    public async Task<IncomeCategoryResponse> GetIncomeCategoriesAsync(IncomeCategoryRequest request)
    {
        Expression<Func<IncomeCategoryEntity, bool>> returnAll = (IncomeCategoryEntity x) => true;
        Expression<Func<IncomeCategoryEntity, bool>> searchCategories = (IncomeCategoryEntity x) => x.Name.ToLower().Contains(request.Query.ToLower());

        var predicate = request.Query == null ? returnAll : searchCategories;

        var categories = await _repo.FindWithPagination(predicate, request.PageNumber, request.PageSize);
        var count = await _repo.GetCount(predicate);

        var response = new IncomeCategoryResponse(request.PageNumber, request.PageSize, count, _mapper.Map<IncomeCategoryListItem[]>(categories));

        return response;
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

    public async Task<int> UpdateIncomeCategoryAsync(AddEditIncomeCategory request)
    {
        var categories = await _repo.Find(x => x.Name == request.Name);
        if (categories.Any(x => x.Id != request.Id))
            throw new DuplicateNameException(request.Name, nameof(IncomeCategoryEntity));

        var category = categories.First(x => x.Id == request.Id.Value);
        if (category == null)
            throw new CategoryNotFoundException(request.Id.Value.ToString());

        category.Name = request.Name;
        category.Notes = request.Notes;
        category.InUse = request.InUse;

        return await _repo.Update(category);
    }

    public async Task<bool> CheckIfIncomeCategoryIsRefund(int categoryId)
    {
        var refundCategoryId = (await _repo.Find(x => x.Name == "Refund")).FirstOrDefault().Id;
        return refundCategoryId == categoryId;
    }
}

public class IncomeCategoryProfile : Profile
{
    public IncomeCategoryProfile()
    {
        //Maybe add a property on the List Item that associates the item with a number of incomes ???? 
        //You would have to get rid of this map then.
        CreateMap<IncomeCategoryEntity, IncomeCategoryListItem>()
            .ForMember(x => x.Id, o => o.MapFrom(src => src.Id))
            .ForMember(x => x.Name, o => o.MapFrom(src => src.Name))
            .ReverseMap();

        CreateMap<AddEditIncomeCategory, IncomeCategoryEntity>()
            .ForMember(dest => dest.Id, o => o.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, o => o.MapFrom(src => src.Name))
            .ForMember(dest => dest.Notes, o => o.MapFrom(src => src.Notes))
            .ForMember(dest => dest.InUse, o => o.MapFrom(src => src.InUse))
            .ReverseMap();
    }
}

