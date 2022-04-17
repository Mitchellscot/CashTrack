﻿using AutoMapper;
using CashTrack.Data.Entities;
using CashTrack.Common.Exceptions;
using CashTrack.Models.IncomeSourceModels;
using CashTrack.Repositories.IncomeSourceRepository;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using CashTrack.Repositories.IncomeRepository;
using CashTrack.Services.Common;

namespace CashTrack.Services.IncomeSourceService;

public interface IIncomeSourceService
{
    Task<IncomeSourceResponse> GetIncomeSourcesAsync(IncomeSourceRequest request);
    Task<int> CreateIncomeSourceAsync(IncomeSource request);
    Task<IncomeSourceEntity> GetIncomeSourceByName(string name);
    Task<int> UpdateIncomeSourceAsync(IncomeSource request);
    Task<bool> DeleteIncomeSourceAsync(int id);
    Task<string[]> GetMatchingIncomeSourcesAsync(string name);
    Task<string[]> GetAllIncomeSourceNames();
}
public class IncomeSourceService : IIncomeSourceService
{
    private readonly IIncomeSourceRepository _repo;
    private readonly IMapper _mapper;
    private readonly IIncomeRepository _incomeRepo;

    public IncomeSourceService(IIncomeSourceRepository repo, IIncomeRepository incomeRepo, IMapper mapper) => (_repo, _incomeRepo, _mapper) = (repo, incomeRepo, mapper);

    public async Task<int> CreateIncomeSourceAsync(IncomeSource request)
    {
        var categories = await _repo.Find(x => true);
        if (categories.Any(x => x.Name == request.Name))
            throw new DuplicateNameException(nameof(IncomeSourceEntity), request.Name);

        var sourceEntity = _mapper.Map<IncomeSourceEntity>(request);

        return await _repo.Create(sourceEntity);
    }

    public async Task<bool> DeleteIncomeSourceAsync(int id)
    {
        var source = await _repo.FindById(id);
        if (source == null)
            throw new IncomeSourceNotFoundException(id.ToString());

        return await _repo.Delete(source);
    }

    public async Task<string[]> GetAllIncomeSourceNames()
    {
        return (await _repo.Find(x => true)).Select(x => x.Name).ToArray();
    }

    public async Task<IncomeSourceEntity> GetIncomeSourceByName(string name)
    {
        var source = (await _repo.Find(x => x.Name == name)).FirstOrDefault();
        if (source == null)
            throw new IncomeSourceNotFoundException(name);
        return source;
    }

    public async Task<IncomeSourceResponse> GetIncomeSourcesAsync(IncomeSourceRequest request)
    {
        var sources = await _repo.Find(x => true);
        var income = await _incomeRepo.Find(x => true);
        var categories = income.Select(x => x.Category).Distinct().ToArray();
        var count = await _repo.GetCount(x => true);
        var sourceListItems = income.GroupBy(i => i.SourceId).Select(g =>
            {
                var results = g.Aggregate(new SourceListItemAggregator(g.Key, categories, sources), (acc, i) => acc.Accumulate(i), acc => acc.Compute());
                return new IncomeSourceListItem()
                {
                    Id = g.Key.HasValue ? g.Key.Value : 0,
                    Name = results.Source != null ? results.Source.Name : null,
                    Payments = results.Payments,
                    Amount = results.Amount,
                    LastPayment = results.LastPayment,
                    Category = results.MostUsedCategory,
                    Description = results.Source != null ? results.Source.Description : null,
                };
            }).Where(x => x.Id > 0).OrderByDescending(x => x.LastPayment).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToArray();

        return new IncomeSourceResponse(request.PageNumber, request.PageSize, count, sourceListItems);
    }

    public async Task<string[]> GetMatchingIncomeSourcesAsync(string name)
    {
        return (await _repo.Find(x => x.Name.StartsWith(name) && x.InUse == true)).Select(x => x.Name).Take(10).ToArray();
    }

    public async Task<int> UpdateIncomeSourceAsync(IncomeSource request)
    {
        var checkId = await _repo.FindById(request.Id.Value);
        if (checkId == null)
            throw new IncomeSourceNotFoundException(request.Id.Value.ToString());

        var nameCheck = await _repo.Find(x => x.Name == request.Name);
        if (nameCheck.Any())
            throw new DuplicateNameException(nameof(IncomeSourceEntity), request.Name);

        var source = _mapper.Map<IncomeSourceEntity>(request);
        return await _repo.Update(source);
    }
}

public class IncomeSourcesProfile : Profile
{
    public IncomeSourcesProfile()
    {
        //todo: make this more interesting by putting some more data on it.
        //What would be cool to see when looking at a table of income soures?
        CreateMap<IncomeSourceEntity, IncomeSourceListItem>()
            .ForMember(x => x.Id, o => o.MapFrom(src => src.Id))
            .ForMember(x => x.Name, o => o.MapFrom(src => src.Name))
            .ReverseMap();

        CreateMap<IncomeSource, IncomeSourceEntity>()
            .ForMember(x => x.Id, o => o.MapFrom(src => src.Id))
            .ForMember(x => x.Name, o => o.MapFrom(src => src.Name))
            .ForMember(x => x.Description, o => o.MapFrom(src => src.Description))
            .ForMember(x => x.InUse, o => o.MapFrom(src => src.InUse))
            .ReverseMap();
    }
}
