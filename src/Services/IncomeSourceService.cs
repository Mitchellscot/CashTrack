﻿using AutoMapper;
using CashTrack.Data.Entities;
using CashTrack.Common.Exceptions;
using CashTrack.Models.IncomeSourceModels;
using CashTrack.Repositories.IncomeSourceRepository;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CashTrack.Services.IncomeSourceService;

public interface IIncomeSourceService
{
    Task<IncomeSourceResponse> GetIncomeSourcesAsync(IncomeSourceRequest request);
    Task<AddEditIncomeSource> CreateIncomeSourceAsync(AddEditIncomeSource request);
    Task<bool> UpdateIncomeSourceAsync(AddEditIncomeSource request);
    Task<bool> DeleteIncomeSourceAsync(int id);
}
public class IncomeSourceService : IIncomeSourceService
{
    private readonly IIncomeSourceRepository _repo;
    private readonly IMapper _mapper;

    public IncomeSourceService(IIncomeSourceRepository repo, IMapper mapper) => (_repo, _mapper) = (repo, mapper);

    public async Task<AddEditIncomeSource> CreateIncomeSourceAsync(AddEditIncomeSource request)
    {
        var categories = await _repo.Find(x => true);
        if (categories.Any(x => x.source == request.Name))
            throw new DuplicateNameException(nameof(IncomeSources), request.Name);

        var sourceEntity = _mapper.Map<IncomeSources>(request);
        sourceEntity.id = await _repo.GetCount(x => true) + 1;

        if (!await _repo.Create(sourceEntity))
            throw new Exception("Couldn't save income category to the database.");

        request.Id = sourceEntity.id;
        return request;
    }

    public async Task<bool> DeleteIncomeSourceAsync(int id)
    {
        var source = await _repo.FindById(id);
        if (source == null)
            throw new IncomeSourceNotFoundException(id.ToString());

        return await _repo.Delete(source);
    }

    public async Task<IncomeSourceResponse> GetIncomeSourcesAsync(IncomeSourceRequest request)
    {
        Expression<Func<IncomeSources, bool>> returnAll = (IncomeSources x) => true;
        Expression<Func<IncomeSources, bool>> searchSources = (IncomeSources x) => x.source.ToLower().Contains(request.Query.ToLower());

        var predicate = request.Query == null ? returnAll : searchSources;

        var sources = await _repo.FindWithPagination(predicate, request.PageNumber, request.PageSize);
        var count = await _repo.GetCount(predicate);

        var response = new IncomeSourceResponse(request.PageNumber, request.PageSize, count, _mapper.Map<IncomeSourceListItem[]>(sources));

        return response;
    }

    public async Task<bool> UpdateIncomeSourceAsync(AddEditIncomeSource request)
    {
        var checkId = await _repo.FindById(request.Id.Value);
        if (checkId == null)
            throw new IncomeSourceNotFoundException(request.Id.Value.ToString());

        var nameCheck = await _repo.Find(x => x.source == request.Name);
        if (nameCheck.Any())
            throw new DuplicateNameException(nameof(IncomeSources), request.Name);

        var source = _mapper.Map<IncomeSources>(request);
        return await _repo.Update(source);
    }
}

public class IncomeSourcesProfile : Profile
{
    public IncomeSourcesProfile()
    {
        //todo: make this more interesting by putting some more data on it.
        //What would be cool to see when looking at a table of income soures?
        CreateMap<IncomeSources, IncomeSourceListItem>()
            .ForMember(x => x.Id, o => o.MapFrom(src => src.id))
            .ForMember(x => x.Name, o => o.MapFrom(src => src.source))
            .ReverseMap();

        CreateMap<AddEditIncomeSource, IncomeSources>()
            .ForMember(x => x.id, o => o.MapFrom(src => src.Id))
            .ForMember(x => x.source, o => o.MapFrom(src => src.Name))
            .ForMember(x => x.description, o => o.MapFrom(src => src.Description))
            .ForMember(x => x.in_use, o => o.MapFrom(src => src.InUse))
            .ReverseMap();

    }
}
