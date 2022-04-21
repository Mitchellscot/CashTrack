﻿using AutoMapper;
using CashTrack.Data.Entities;
using CashTrack.Common.Exceptions;
using CashTrack.Models.IncomeSourceModels;
using CashTrack.Repositories.IncomeSourceRepository;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using CashTrack.Repositories.IncomeRepository;
using CashTrack.Services.Common;
using CashTrack.Models.IncomeModels;

namespace CashTrack.Services.IncomeSourceService;

public interface IIncomeSourceService
{
    Task<IncomeSourceResponse> GetIncomeSourcesAsync(IncomeSourceRequest request);
    Task<IncomeSourceDetail> GetIncomeSourceDetailAsync(int id);
    Task<int> CreateIncomeSourceAsync(IncomeSource request);
    Task<IncomeSourceEntity> GetIncomeSourceByName(string name);
    Task<int> UpdateIncomeSourceAsync(IncomeSource request);
    Task<bool> DeleteIncomeSourceAsync(int id);
    Task<string[]> GetMatchingIncomeSourcesAsync(string name);
    Task<string[]> GetAllIncomeSourceNames();
}
public class IncomeSourceService : IIncomeSourceService
{
    private readonly IIncomeSourceRepository _sourceRepo;
    private readonly IMapper _mapper;
    private readonly IIncomeRepository _incomeRepo;

    public IncomeSourceService(IIncomeSourceRepository repo, IIncomeRepository incomeRepo, IMapper mapper) => (_sourceRepo, _incomeRepo, _mapper) = (repo, incomeRepo, mapper);

    public async Task<int> CreateIncomeSourceAsync(IncomeSource request)
    {
        var categories = await _sourceRepo.Find(x => true);
        if (categories.Any(x => x.Name == request.Name))
            throw new DuplicateNameException(nameof(IncomeSourceEntity), request.Name);

        var sourceEntity = _mapper.Map<IncomeSourceEntity>(request);

        return await _sourceRepo.Create(sourceEntity);
    }

    public async Task<bool> DeleteIncomeSourceAsync(int id)
    {
        var source = await _sourceRepo.FindById(id);
        if (source == null)
            throw new IncomeSourceNotFoundException(id.ToString());

        return await _sourceRepo.Delete(source);
    }

    public async Task<string[]> GetAllIncomeSourceNames()
    {
        return (await _sourceRepo.Find(x => true)).Select(x => x.Name).ToArray();
    }
    public async Task<IncomeSourceEntity> GetIncomeSourceByName(string name)
    {
        var source = (await _sourceRepo.Find(x => x.Name == name)).FirstOrDefault();
        if (source == null)
            throw new IncomeSourceNotFoundException(name);
        return source;
    }
    public async Task<IncomeSourceResponse> GetIncomeSourcesAsync(IncomeSourceRequest request)
    {
        var sources = await _sourceRepo.Find(x => true);
        var income = await _incomeRepo.Find(x => true);
        var categories = income.Select(x => x.Category).Distinct().ToArray();
        var count = await _sourceRepo.GetCount(x => true);
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
                    Category = results.MostUsedCategory
                };
            }).Where(x => x.Id > 0).OrderByDescending(x => x.LastPayment).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToArray();

        return new IncomeSourceResponse(request.PageNumber, request.PageSize, count, sourceListItems);
    }

    public async Task<string[]> GetMatchingIncomeSourcesAsync(string name)
    {
        return (await _sourceRepo.Find(x => x.Name.StartsWith(name) && x.InUse == true)).Select(x => x.Name).Take(10).ToArray();
    }

    public async Task<int> UpdateIncomeSourceAsync(IncomeSource request)
    {
        var source = await _sourceRepo.FindById(request.Id.Value);
        if (source == null)
            throw new IncomeSourceNotFoundException(request.Id.Value.ToString());

        var nameCheck = await _sourceRepo.Find(x => x.Name == request.Name);
        if (nameCheck.Any())
            throw new DuplicateNameException(nameof(IncomeSourceEntity), request.Name);

        source.Id = request.Id.Value;
        source.Name = request.Name;
        source.InUse = true;
        source.Description = request.Description;

        return await _sourceRepo.Update(source);
    }
    public async Task<IncomeSourceDetail> GetIncomeSourceDetailAsync(int id)
    {
        var source = await _sourceRepo.FindById(id);
        var incomes = await _incomeRepo.GetIncomeAndCategoriesBySourceId(id);
        var categories = incomes.Select(x => x.Category).Distinct().ToArray();

        var incomeSpansMultipleYears = incomes.GroupBy(x => x.Date.Year).ToList().Count > 1;

        return new IncomeSourceDetail()
        {
            Id = source.Id,
            Name = source.Name,
            InUse = source.InUse,
            Description = source.Description,

            IncomeTotals = incomes.Aggregate(new TotalsAggregator<IncomeEntity>(),
                (acc, i) => acc.Accumulate(i),
                acc => acc.Compute()
            ),

            MostUsedCategory = GetIncomeCategoryOccurances(categories, incomes).FirstOrDefault().Key,

            AnnualIncomeStatistcis = incomeSpansMultipleYears ? AggregateUtilities<IncomeEntity>.GetAnnualStatistics(incomes) : new List<AnnualStatistics>(),

            MonthlyIncomeStatistcis = incomeSpansMultipleYears ? new List<MonthlyStatistics>() : AggregateUtilities<IncomeEntity>.GetMonthlyStatistics(incomes),

            PaymentCategoryOcurances = GetIncomeCategoryOccurances(categories, incomes),
            PaymentCategoryTotals = GetIncomeCategoryTotals(categories, incomes),

            RecentIncomes = incomes.OrderByDescending(x => x.Date)
            .Take(9)
            .Select(x => new IncomeQuickView()
            {
                Id = x.Id,
                Date = x.Date.Date.ToShortDateString(),
                Amount = x.Amount,
                Category = x.Category == null ? "none" : x.Category.Name
            }).ToList()
        };
    }
    private Dictionary<string, decimal> GetIncomeCategoryTotals(IncomeCategoryEntity[] categories, IncomeEntity[] incomes)
    {
        return categories.GroupJoin(incomes,
            c => c.Id, e => e.Category.Id, (c, g) => new
            {
                Category = c.Name,
                Incomes = g
            }).Select(x => new
            {
                Category = x.Category,
                Sum = x.Incomes.Sum(e => e.Amount)
            }).Where(x => x.Sum > 0).OrderByDescending(x => x.Sum).ToDictionary(k => k.Category, v => v.Sum);
    }
    private Dictionary<string, int> GetIncomeCategoryOccurances(IncomeCategoryEntity[] categories, IncomeEntity[] incomes)
    {
        return categories.GroupJoin(incomes,
            c => c.Id, i => i.SourceId, (c, g) => new
            {
                Category = c.Name,
                Incomes = g
            }).Select(x => new
            {
                Category = x.Category,
                Count = x.Incomes.Count()
            }).OrderByDescending(x => x.Count).ToDictionary(k => k.Category, v => v.Count);
    }
}


public class IncomeSourcesProfile : Profile
{
    public IncomeSourcesProfile()
    {
        CreateMap<IncomeSource, IncomeSourceEntity>()
            .ForMember(x => x.Id, o => o.MapFrom(src => src.Id))
            .ForMember(x => x.Name, o => o.MapFrom(src => src.Name))
            .ForMember(x => x.Description, o => o.MapFrom(src => src.Description))
            .ForMember(x => x.InUse, o => o.MapFrom(src => src.InUse))
            .ReverseMap();
    }
}
