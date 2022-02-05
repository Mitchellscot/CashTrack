using AutoMapper;
using CashTrack.Data.Entities;
using CashTrack.Models.IncomeModels;
using CashTrack.Repositories.IncomeRepository;
using CashTrack.Services.Common;
using System;
using System.Threading.Tasks;

namespace CashTrack.Services.IncomeService;

public interface IIncomeService
{
    Task<IncomeResponse> GetIncomeAsync(IncomeRequest request);
    Task<IncomeListItem> GetIncomeByIdAsync(int id);
    Task<AddEditIncome> CreateIncomeAsync(AddEditIncome request);
    Task<bool> UpdateIncomeAsync(AddEditIncome request);
    Task<bool> DeleteIncomeAsync(int id);
}
public class IncomeService : IIncomeService
{
    private readonly IMapper _mapper;
    private readonly IIncomeRepository _repo;

    public IncomeService(IIncomeRepository repo, IMapper mapper) => (_repo, _mapper) = (repo, mapper);

    public async Task<AddEditIncome> CreateIncomeAsync(AddEditIncome request)
    {
        if (request.Id != null)
            throw new ArgumentException("Request must not contain an id in order to create an income.");

        var income = _mapper.Map<Incomes>(request);

        income.Id = ((int)await _repo.GetCount(x => true)) + 1;
        var success = await _repo.Create(income);
        if (!success)
            throw new Exception("Couldn't save income to the database.");

        request.Id = income.Id;

        return request;
    }

    public async Task<bool> DeleteIncomeAsync(int id)
    {
        var income = await _repo.FindById(id);

        return await _repo.Delete(income);
    }

    public async Task<IncomeResponse> GetIncomeAsync(IncomeRequest request)
    {
        var predicate = DateOption<Incomes, IncomeRequest>.Parse(request);
        var expenses = await _repo.FindWithPagination(predicate, request.PageNumber, request.PageSize);
        var count = await _repo.GetCount(predicate);
        var amount = await _repo.GetAmountOfIncome(predicate);

        return new IncomeResponse(request.PageNumber, request.PageSize, count, _mapper.Map<IncomeListItem[]>(expenses), amount);
    }

    public async Task<IncomeListItem> GetIncomeByIdAsync(int id)
    {
        //Change this to income detail in the future, once you know what you want it to look like.
        var singleExpense = await _repo.FindById(id);
        return _mapper.Map<IncomeListItem>(singleExpense);
    }

    public async Task<bool> UpdateIncomeAsync(AddEditIncome request)
    {
        if (request.Id == null)
            throw new ArgumentException("Need an id to update an income");

        var income = _mapper.Map<Incomes>(request);
        return await _repo.Update(income);
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
            .ReverseMap();

        CreateMap<AddEditIncome, Incomes>()
            .ForMember(x => x.date, o => o.MapFrom(src => src.Date.ToUniversalTime()))
            .ForMember(x => x.Id, o => o.MapFrom(src => src.Id))
            .ForMember(x => x.amount, o => o.MapFrom(src => src.Amount))
            .ForMember(x => x.categoryid, o => o.MapFrom(src => src.CategoryId))
            .ForMember(x => x.sourceid, o => o.MapFrom(src => src.SourceId))
            .ForMember(x => x.notes, o => o.MapFrom(src => src.Notes))
            .ReverseMap();
    }
}