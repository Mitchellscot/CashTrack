using AutoMapper;
using CashTrack.Data.Entities;
using CashTrack.Models.ExpenseModels;
using CashTrack.Models.IncomeModels;
using CashTrack.Repositories.IncomeRepository;
using CashTrack.Services.Common;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CashTrack.Services.IncomeService;

public interface IIncomeService
{
    Task<IncomeResponse> GetIncomeAsync(IncomeRequest request);
    Task<IncomeListItem> GetIncomeByIdAsync(int id);
    Task<IncomeResponse> GetIncomeByAmountAsync(AmountSearchRequest request);
    Task<IncomeResponse> GetIncomeBySourceAsync(IncomeRequest request);
    Task<IncomeResponse> GetIncomeByIncomeCategoryIdAsync(IncomeRequest request);
    Task<IncomeResponse> GetIncomeByNotesAsync(IncomeRequest request);
    Task<AddEditIncome> CreateIncomeAsync(AddEditIncome request);
    Task<bool> UpdateIncomeAsync(AddEditIncome request);
    Task<bool> DeleteIncomeAsync(int id);
}
public class IncomeService : IIncomeService
{
    private readonly IMapper _mapper;
    private readonly IIncomeRepository _incomeRespository;

    public IncomeService(IIncomeRepository incomeRepository, IMapper mapper) => (_incomeRespository, _mapper) = (incomeRepository, mapper);

    public async Task<AddEditIncome> CreateIncomeAsync(AddEditIncome request)
    {
        if (request.Id != null)
            throw new ArgumentException("Request must not contain an id in order to create an income.");

        var income = _mapper.Map<Incomes>(request);

        income.Id = ((int)await _incomeRespository.GetCount(x => true)) + 1;
        var success = await _incomeRespository.Create(income);
        if (!success)
            throw new Exception("Couldn't save income to the database.");

        request.Id = income.Id;

        return request;
    }

    public async Task<bool> DeleteIncomeAsync(int id)
    {
        var income = await _incomeRespository.FindById(id);

        return await _incomeRespository.Delete(income);
    }

    public async Task<IncomeResponse> GetIncomeByAmountAsync(AmountSearchRequest request)
    {
        Expression<Func<Incomes, bool>> predicate = x => x.amount == request.Query;
        var income = await _incomeRespository.FindWithPagination(x => x.amount == request.Query, request.PageNumber, request.PageSize);
        var count = await _incomeRespository.GetCount(predicate);
        var amount = await _incomeRespository.GetAmountOfIncome(predicate);
        return new IncomeResponse(request.PageNumber, request.PageSize, count, _mapper.Map<IncomeListItem[]>(income), amount);
    }
    public async Task<IncomeResponse> GetIncomeByNotesAsync(IncomeRequest request)
    {
        Expression<Func<Incomes, bool>> predicate = x => x.notes.ToLower().Contains(request.Query.ToLower());
        var income = await _incomeRespository.FindWithPagination(predicate, request.PageNumber, request.PageSize);
        var count = await _incomeRespository.GetCount(predicate);
        var amount = await _incomeRespository.GetAmountOfIncome(predicate);
        return new IncomeResponse(request.PageNumber, request.PageSize, count, _mapper.Map<IncomeListItem[]>(income), amount);
    }
    public async Task<IncomeResponse> GetIncomeByIncomeCategoryIdAsync(IncomeRequest request)
    {
        Expression<Func<Incomes, bool>> predicate = x => x.category.Id == int.Parse(request.Query);
        var income = await _incomeRespository.FindWithPagination(predicate, request.PageNumber, request.PageSize);
        var count = await _incomeRespository.GetCount(predicate);
        var amount = await _incomeRespository.GetAmountOfIncome(predicate);

        return new IncomeResponse(request.PageNumber, request.PageSize, count, _mapper.Map<IncomeListItem[]>(income), amount);
    }
    public async Task<IncomeResponse> GetIncomeBySourceAsync(IncomeRequest request)
    {
        Expression<Func<Incomes, bool>> predicate = x => x.source.source.ToLower() == request.Query.ToLower();
        var income = await _incomeRespository.FindWithPagination(predicate, request.PageNumber, request.PageSize);
        var count = await _incomeRespository.GetCount(predicate);
        var amount = await _incomeRespository.GetAmountOfIncome(predicate);

        return new IncomeResponse(request.PageNumber, request.PageSize, count, _mapper.Map<IncomeListItem[]>(income), amount);
    }
    public async Task<IncomeResponse> GetIncomeAsync(IncomeRequest request)
    {
        var predicate = DateOption<Incomes, IncomeRequest>.Parse(request);
        var expenses = await _incomeRespository.FindWithPagination(predicate, request.PageNumber, request.PageSize);
        var count = await _incomeRespository.GetCount(predicate);
        var amount = await _incomeRespository.GetAmountOfIncome(predicate);

        return new IncomeResponse(request.PageNumber, request.PageSize, count, _mapper.Map<IncomeListItem[]>(expenses), amount);
    }

    public async Task<IncomeListItem> GetIncomeByIdAsync(int id)
    {
        //Change this to income detail in the future, once you know what you want it to look like.
        var singleExpense = await _incomeRespository.FindById(id);
        return _mapper.Map<IncomeListItem>(singleExpense);
    }

    public async Task<bool> UpdateIncomeAsync(AddEditIncome request)
    {
        if (request.Id == null)
            throw new ArgumentException("Need an id to update an income");

        var income = _mapper.Map<Incomes>(request);
        return await _incomeRespository.Update(income);
    }
}

public class IncomeMapperProfile : Profile
{
    public IncomeMapperProfile()
    {
        CreateMap<Incomes, IncomeListItem>()
            .ForMember(x => x.Id, o => o.MapFrom(x => x.Id))
            .ForMember(x => x.Date, o => o.MapFrom(x => x.date))
            .ForMember(x => x.Amount, o => o.MapFrom(x => x.amount))
            .ForMember(x => x.Category, o => o.MapFrom(x => x.category.category))
            .ForMember(x => x.Source, o => o.MapFrom(x => x.source.source))
            .ForMember(x => x.Notes, o => o.MapFrom(x => x.notes));

        CreateMap<AddEditIncome, Incomes>()
            .ForMember(x => x.date, o => o.MapFrom(src => src.Date.ToUniversalTime()))
            .ForMember(x => x.Id, o => o.MapFrom(src => src.Id))
            .ForMember(x => x.amount, o => o.MapFrom(src => src.Amount))
            .ForMember(x => x.categoryid, o => o.MapFrom(src => src.CategoryId))
            .ForMember(x => x.sourceid, o => o.MapFrom(src => src.SourceId))
            .ForMember(x => x.notes, o => o.MapFrom(src => src.Notes));
    }
}