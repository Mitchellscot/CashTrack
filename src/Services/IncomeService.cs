using AutoMapper;
using CashTrack.Common.Exceptions;
using CashTrack.Data.Entities;
using CashTrack.Models.ExpenseModels;
using CashTrack.Models.IncomeModels;
using CashTrack.Repositories.IncomeRepository;
using CashTrack.Repositories.IncomeSourceRepository;
using CashTrack.Services.Common;
using System;
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

    public async Task<bool> DeleteIncomeAsync(int id)
    {
        var income = await _incomeRespository.FindById(id);

        return await _incomeRespository.Delete(income);
    }

    public async Task<IncomeResponse> GetIncomeByAmountAsync(AmountSearchRequest request)
    {
        Expression<Func<IncomeEntity, bool>> predicate = x => x.Amount == request.Query;
        var income = await _incomeRespository.FindWithPagination(x => x.Amount == request.Query, request.PageNumber, request.PageSize);
        var count = await _incomeRespository.GetCount(predicate);
        var amount = await _incomeRespository.GetAmountOfIncomeNoRefunds(predicate);
        return new IncomeResponse(request.PageNumber, request.PageSize, count, _mapper.Map<Income[]>(income), amount);
    }
    public async Task<IncomeResponse> GetIncomeByNotesAsync(IncomeRequest request)
    {
        Expression<Func<IncomeEntity, bool>> predicate = x => x.Notes.ToLower().Contains(request.Query.ToLower());
        var income = await _incomeRespository.FindWithPagination(predicate, request.PageNumber, request.PageSize);
        var count = await _incomeRespository.GetCount(predicate);
        var amount = await _incomeRespository.GetAmountOfIncomeNoRefunds(predicate);
        return new IncomeResponse(request.PageNumber, request.PageSize, count, _mapper.Map<Income[]>(income), amount);
    }
    public async Task<IncomeResponse> GetIncomeByIncomeCategoryIdAsync(IncomeRequest request)
    {
        Expression<Func<IncomeEntity, bool>> predicate = x => x.Category.Id == int.Parse(request.Query);
        var income = await _incomeRespository.FindWithPagination(predicate, request.PageNumber, request.PageSize);
        var count = await _incomeRespository.GetCount(predicate);
        var amount = await _incomeRespository.GetAmountOfIncomeNoRefunds(predicate);

        return new IncomeResponse(request.PageNumber, request.PageSize, count, _mapper.Map<Income[]>(income), amount);
    }
    public async Task<IncomeResponse> GetIncomeBySourceAsync(IncomeRequest request)
    {
        Expression<Func<IncomeEntity, bool>> predicate = x => x.Source.Name.ToLower() == request.Query.ToLower();
        var income = await _incomeRespository.FindWithPagination(predicate, request.PageNumber, request.PageSize);
        var count = await _incomeRespository.GetCount(predicate);
        var amount = await _incomeRespository.GetAmountOfIncomeNoRefunds(predicate);

        return new IncomeResponse(request.PageNumber, request.PageSize, count, _mapper.Map<Income[]>(income), amount);
    }
    public async Task<IncomeResponse> GetIncomeAsync(IncomeRequest request)
    {
        var predicate = DateOption<IncomeEntity, IncomeRequest>.Parse(request);
        var income = await _incomeRespository.FindWithPagination(predicate, request.PageNumber, request.PageSize);
        var count = await _incomeRespository.GetCount(predicate);
        //not including refunds in the total amount because that's cheating...
        var amount = await _incomeRespository.GetAmountOfIncomeNoRefunds(predicate);
        return new IncomeResponse(request.PageNumber, request.PageSize, count, _mapper.Map<Income[]>(income), amount);
    }
    public async Task<Income> GetIncomeByIdAsync(int id)
    {
        //Change this to income detail in the future, once you know what you want it to look like.
        var singleExpense = await _incomeRespository.FindById(id);
        return _mapper.Map<Income>(singleExpense);
    }
    public async Task<bool> CreateIncomeAsync(Income request)
    {
        if (request.Id != null)
            throw new ArgumentException("Request must not contain an id in order to create an income.");

        var incomeEntity = new IncomeEntity()
        {
            Amount = request.Amount,
            Date = request.Date,
            Notes = request.Notes,
            //gets converted to string id in controller
            SourceId = string.IsNullOrEmpty(request.Source) ? null : int.Parse(request.Source),
            CategoryId = int.Parse(request.Category), //comes in as a string integer (dropdwn list)
            IsRefund = request.IsRefund
        };

        return await _incomeRespository.Create(incomeEntity);
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

        var incomeEntity = new IncomeEntity()
        {
            Id = request.Id.Value,
            Amount = request.Amount,
            Date = request.Date,
            Notes = request.Notes,
            SourceId = string.IsNullOrEmpty(request.Source) ? null : int.Parse(request.Source), //converted to a string id in page modal
            CategoryId = int.Parse(request.Category), //comes in as a string integer (dropdwn list)
            IsRefund = request.IsRefund
        };
        return await _incomeRespository.Update(incomeEntity);
    }
}

public class IncomeMapperProfile : Profile
{
    public IncomeMapperProfile()
    {
        CreateMap<IncomeEntity, Income>()
            .ForMember(x => x.Id, o => o.MapFrom(x => x.Id))
            .ForMember(x => x.Date, o => o.MapFrom(x => x.Date))
            .ForMember(x => x.Amount, o => o.MapFrom(x => x.Amount))
            .ForMember(x => x.Category, o => o.MapFrom(x => x.Category.Name))
            .ForMember(x => x.Source, o => o.MapFrom(x => x.Source.Name))
            .ForMember(x => x.Notes, o => o.MapFrom(x => x.Notes))
            .ForMember(x => x.IsRefund, o => o.MapFrom(x => x.IsRefund));
    }
}