using AutoMapper;
using CashTrack.Common.Exceptions;
using CashTrack.Data.Entities;
using CashTrack.Models.ExpenseModels;
using CashTrack.Models.IncomeModels;
using CashTrack.Repositories.IncomeRepository;
using CashTrack.Repositories.IncomeSourceRepository;
using CashTrack.Services.Common;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CashTrack.Services.IncomeService;

public interface IIncomeService
{
    Task<IncomeResponse> GetIncomeAsync(IncomeRequest request);
    Task<Income> GetIncomeByIdAsync(int id);
    Task<IncomeResponse> GetIncomeByAmountAsync(AmountSearchRequest request);
    Task<IncomeResponse> GetIncomeBySourceAsync(IncomeRequest request);
    Task<IncomeResponse> GetIncomeByIncomeCategoryIdAsync(IncomeRequest request);
    Task<IncomeResponse> GetIncomeByNotesAsync(IncomeRequest request);
    Task<bool> CreateIncomeAsync(Income request);
    Task<bool> UpdateIncomeAsync(Income request);
    Task<bool> DeleteIncomeAsync(int id);
}
public class IncomeService : IIncomeService
{
    private readonly IMapper _mapper;
    private readonly IIncomeRepository _incomeRespository;
    private readonly IIncomeSourceRepository _sourceRepository;

    public IncomeService(IIncomeRepository incomeRepository, IIncomeSourceRepository sourceRepository, IMapper mapper) => (_incomeRespository, _sourceRepository, _mapper) = (incomeRepository, sourceRepository, mapper);

    public async Task<bool> CreateIncomeAsync(Income request)
    {
        if (request.Id != null)
            throw new ArgumentException("Request must not contain an id in order to create an income.");

        var incomeEntity = new Incomes()
        {
            amount = request.Amount,
            date = request.Date,
            notes = request.Notes,
            //gets converted to string id in controller
            sourceid = string.IsNullOrEmpty(request.Source) ? null : int.Parse(request.Source),
            categoryid = int.Parse(request.Category), //comes in as a string integer (dropdwn list)
            is_refund = request.IsRefund
        };

        return await _incomeRespository.Create(incomeEntity);
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
        //not including refunds in the total amount because that's cheating...
        var amount = await _incomeRespository.GetAmountOfIncomeNoRefunds(predicate);
        return new IncomeResponse(request.PageNumber, request.PageSize, count, _mapper.Map<Income[]>(income), amount);
    }
    public async Task<IncomeResponse> GetIncomeByNotesAsync(IncomeRequest request)
    {
        Expression<Func<Incomes, bool>> predicate = x => x.notes.ToLower().Contains(request.Query.ToLower());
        var income = await _incomeRespository.FindWithPagination(predicate, request.PageNumber, request.PageSize);
        var count = await _incomeRespository.GetCount(predicate);
        var amount = await _incomeRespository.GetAmountOfIncomeNoRefunds(predicate);
        return new IncomeResponse(request.PageNumber, request.PageSize, count, _mapper.Map<Income[]>(income), amount);
    }
    public async Task<IncomeResponse> GetIncomeByIncomeCategoryIdAsync(IncomeRequest request)
    {
        Expression<Func<Incomes, bool>> predicate = x => x.category.Id == int.Parse(request.Query);
        var income = await _incomeRespository.FindWithPagination(predicate, request.PageNumber, request.PageSize);
        var count = await _incomeRespository.GetCount(predicate);
        var amount = await _incomeRespository.GetAmountOfIncomeNoRefunds(predicate);

        return new IncomeResponse(request.PageNumber, request.PageSize, count, _mapper.Map<Income[]>(income), amount);
    }
    public async Task<IncomeResponse> GetIncomeBySourceAsync(IncomeRequest request)
    {
        Expression<Func<Incomes, bool>> predicate = x => x.source.source.ToLower() == request.Query.ToLower();
        var income = await _incomeRespository.FindWithPagination(predicate, request.PageNumber, request.PageSize);
        var count = await _incomeRespository.GetCount(predicate);
        var amount = await _incomeRespository.GetAmountOfIncomeNoRefunds(predicate);

        return new IncomeResponse(request.PageNumber, request.PageSize, count, _mapper.Map<Income[]>(income), amount);
    }
    public async Task<IncomeResponse> GetIncomeAsync(IncomeRequest request)
    {
        var predicate = DateOption<Incomes, IncomeRequest>.Parse(request);
        var expenses = await _incomeRespository.FindWithPagination(predicate, request.PageNumber, request.PageSize);
        var count = await _incomeRespository.GetCount(predicate);

        var amount = await _incomeRespository.GetAmountOfIncomeNoRefunds(predicate);

        return new IncomeResponse(request.PageNumber, request.PageSize, count, _mapper.Map<Income[]>(expenses), amount);
    }

    public async Task<Income> GetIncomeByIdAsync(int id)
    {
        //Change this to income detail in the future, once you know what you want it to look like.
        var singleExpense = await _incomeRespository.FindById(id);
        return _mapper.Map<Income>(singleExpense);
    }

    public async Task<bool> UpdateIncomeAsync(Income request)
    {
        if (request.Id == null)
            throw new ArgumentException("Need an id to update an income");

        if (string.IsNullOrEmpty(request.Category))
            throw new CategoryNotFoundException("null");

        var currentIncome = await _incomeRespository.Find(x => x.Id == request.Id.Value);
        if (currentIncome == null)
            throw new IncomeNotFoundException(request.Id.Value.ToString());

        var incomeEntity = new Incomes()
        {
            Id = request.Id.Value,
            amount = request.Amount,
            date = request.Date,
            notes = request.Notes,
            sourceid = string.IsNullOrEmpty(request.Source) ? null : int.Parse(request.Source), //converted to a string id in page modal
            categoryid = int.Parse(request.Category), //comes in as a string integer (dropdwn list)
            is_refund = request.IsRefund
        };
        return await _incomeRespository.Update(incomeEntity);
    }
}

public class IncomeMapperProfile : Profile
{
    public IncomeMapperProfile()
    {
        CreateMap<Incomes, Income>()
            .ForMember(x => x.Id, o => o.MapFrom(x => x.Id))
            .ForMember(x => x.Date, o => o.MapFrom(x => x.date))
            .ForMember(x => x.Amount, o => o.MapFrom(x => x.amount))
            .ForMember(x => x.Category, o => o.MapFrom(x => x.category.category))
            .ForMember(x => x.Source, o => o.MapFrom(x => x.source.source))
            .ForMember(x => x.Notes, o => o.MapFrom(x => x.notes));
    }
}