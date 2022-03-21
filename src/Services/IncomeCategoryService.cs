﻿using AutoMapper;
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
    Task<bool> UpdateIncomeCategoryAsync(AddEditIncomeCategory request);
    Task<bool> DeleteIncomeCategoryAsync(int id);
    Task<IncomeCategoryDropdownSelection[]> GetIncomeCategoryDropdownListAsync();
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

        var incomeCategoryEntity = _mapper.Map<IncomeCategoryEntity>(request);
        incomeCategoryEntity.Id = await _repo.GetCount(x => true) + 1;

        if (!await _repo.Create(incomeCategoryEntity))
            throw new Exception("Couldn't save income category to the database.");

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

    public async Task<IncomeCategoryDropdownSelection[]> GetIncomeCategoryDropdownListAsync()
    {
        return (await _repo.Find(x => x.InUse == true)).Select(x => new IncomeCategoryDropdownSelection()
        {
            Id = x.Id,
            Category = x.Name
        }).ToArray();
    }

    public async Task<bool> UpdateIncomeCategoryAsync(AddEditIncomeCategory request)
    {
        var checkId = await _repo.FindById(request.Id.Value);
        if (checkId == null)
            throw new CategoryNotFoundException(request.Id.Value.ToString());

        var nameCheck = await _repo.Find(x => x.Name == request.Name);
        if (nameCheck.Any())
            throw new DuplicateNameException(nameof(IncomeCategoryEntity), request.Name);

        var category = _mapper.Map<IncomeCategoryEntity>(request);
        return await _repo.Update(category);
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
            .ForMember(dest => dest.Description, o => o.MapFrom(src => src.Description))
            .ForMember(dest => dest.InUse, o => o.MapFrom(src => src.InUse))
            .ReverseMap();
    }
}

