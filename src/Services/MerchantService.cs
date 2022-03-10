using AutoMapper;
using CashTrack.Data.Entities;
using CashTrack.Common.Exceptions;
using CashTrack.Models.ExpenseModels;
using CashTrack.Models.MerchantModels;
using CashTrack.Repositories.ExpenseRepository;
using CashTrack.Repositories.MerchantRepository;
using CashTrack.Repositories.SubCategoriesRepository;
using CashTrack.Services.Common;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CashTrack.Services.MerchantService;
public interface IMerchantService
{
    Task<MerchantResponse> GetMerchantsAsync(MerchantRequest request);
    Task<string[]> GetMatchingMerchantsAsync(string match);
    Task<MerchantDetail> GetMerchantDetailAsync(int id);
    Task<AddEditMerchant> CreateMerchantAsync(AddEditMerchant request);
    Task<bool> UpdateMerchantAsync(AddEditMerchant request);
    Task<bool> DeleteMerchantAsync(int id);
    Task<Merchants> GetMerchantByNameAsync(string name);
}
public class MerchantService : IMerchantService
{
    private readonly IMapper _mapper;
    private readonly IMerchantRepository _merchantRepo;
    private readonly ISubCategoryRepository _subCategoryRepo;
    private readonly IExpenseRepository _expenseRepo;

    public MerchantService(IMapper mapper, IMerchantRepository merchantRepo, ISubCategoryRepository subCategoryRepo, IExpenseRepository expenseRepo)
    {
        _mapper = mapper;
        _merchantRepo = merchantRepo;
        _subCategoryRepo = subCategoryRepo;
        _expenseRepo = expenseRepo;
    }

    public async Task<MerchantResponse> GetMerchantsAsync(MerchantRequest request)
    {
        Expression<Func<Merchants, bool>> allMerchants = (Merchants m) => true;
        Expression<Func<Merchants, bool>> merchantSearch = (Merchants m) => m.name.ToLower().Contains(request.Query.ToLower());

        var predicate = request.Query == null ? allMerchants : merchantSearch;

        var merchantEntities = await _merchantRepo.FindWithPagination(predicate, request.PageNumber, request.PageSize);

        var count = await _merchantRepo.GetCount(predicate);

        var merchantViewModels = merchantEntities.Select(m => new MerchantListItem
        {
            Id = m.Id,
            Name = m.name,
            City = m.city,
            IsOnline = m.is_online,
            NumberOfExpenses = _expenseRepo.GetCount(x => x.merchantid == m.Id).Result
        }).ToArray();

        return new MerchantResponse(request.PageNumber, request.PageSize, count, merchantViewModels);
    }

    public async Task<MerchantDetail> GetMerchantDetailAsync(int id)
    {
        var merchantEntity = await _merchantRepo.FindById(id);

        var merchantExpenses = await _expenseRepo.GetExpensesAndCategories(x => x.merchantid == id);

        var recentExpenses = merchantExpenses.OrderByDescending(e => e.date)
            .Take(10)
            .Select(x => new ExpenseQuickView()
            {
                Id = x.Id,
                Date = x.date.Date.ToShortDateString(),
                Amount = x.amount,
                SubCategory = x.category == null ? "none" : x.category.sub_category_name
            }).ToList();

        var expenseTotals = merchantExpenses.Aggregate(new ExpenseTotalsAggregator(),
                (acc, e) => acc.Accumulate(e),
                acc => acc.Compute());

        var expenseStatistics = merchantExpenses.GroupBy(e => e.date.Year)
                .Select(g =>
                {
                    var results = g.Aggregate(new ExpenseStatisticsAggregator(),
                        (acc, e) => acc.Accumulate(e),
                        acc => acc.Compute());

                    return new AnnualExpenseStatistics()
                    {
                        Year = g.Key,
                        Average = results.Average,
                        Min = results.Min,
                        Max = results.Max,
                        Total = results.Total,
                        Count = results.Count
                    };
                }).OrderBy(x => x.Year).ToList();

        var subCategories = await _subCategoryRepo.Find(x => true);

        var merchantExpenseCategories = subCategories.GroupJoin(merchantExpenses,
            c => c.Id, e => e.category.Id, (c, g) => new
            {
                Category = c.sub_category_name,
                Expenses = g
            }).Select(x => new
            {
                Category = x.Category,
                Count = x.Expenses.Count()
            }).Where(x => x.Count > 0).OrderByDescending(x => x.Count).ToDictionary(k => k.Category, v => v.Count);

        var merchantExpenseAmounts = subCategories.GroupJoin(merchantExpenses,
            c => c.Id, e => e.category.Id, (c, g) => new
            {
                Category = c.sub_category_name,
                Expenses = g
            }).Select(x => new
            {
                Category = x.Category,
                Sum = x.Expenses.Sum(e => e.amount)
            }).Where(x => x.Sum > 0).OrderByDescending(x => x.Sum).ToDictionary(k => k.Category, v => v.Sum);

        var mostUsedCategory = merchantExpenseCategories.FirstOrDefault().Key;

        var merchantDetail = new MerchantDetail()
        {
            Id = merchantEntity.Id,
            Name = merchantEntity.name,
            SuggestOnLookup = merchantEntity.suggest_on_lookup,
            City = merchantEntity.city,
            State = merchantEntity.state,
            Notes = merchantEntity.notes,
            IsOnline = merchantEntity.is_online,
            ExpenseTotals = expenseTotals,
            MostUsedCategory = mostUsedCategory,
            AnnualExpenseStatistics = expenseStatistics,
            PurchaseCategoryOccurances = merchantExpenseCategories,
            PurchaseCategoryTotals = merchantExpenseAmounts,
            RecentExpenses = recentExpenses
        };

        return merchantDetail;
    }

    public async Task<AddEditMerchant> CreateMerchantAsync(AddEditMerchant request)
    {
        var merchants = await _merchantRepo.Find(x => x.name == request.Name);
        if (merchants.Any())
            throw new DuplicateNameException(nameof(Merchants), request.Name);

        var merchantEntity = _mapper.Map<Merchants>(request);

        if (!await _merchantRepo.Create(merchantEntity))
            throw new Exception("Couldn't save merchant to the database");

        request.Id = merchantEntity.Id;

        return request;
    }
    public async Task<bool> UpdateMerchantAsync(AddEditMerchant request)
    {
        var merchants = await _merchantRepo.Find(x => x.name == request.Name);
        if (merchants.Any())
            throw new DuplicateNameException(nameof(Merchants), request.Name);

        var merchant = _mapper.Map<Merchants>(request);
        return await _merchantRepo.Update(merchant);
    }

    public async Task<bool> DeleteMerchantAsync(int id)
    {
        var merchant = await _merchantRepo.FindById(id);

        return await _merchantRepo.Delete(merchant);
    }

    public async Task<string[]> GetMatchingMerchantsAsync(string match)
    {
        return (await _merchantRepo.Find(x => x.name.StartsWith(match))).Select(x => x.name).Take(10).ToArray();
    }

    public async Task<Merchants> GetMerchantByNameAsync(string name)
    {
        var merchant = (await _merchantRepo.Find(x => x.name == name)).FirstOrDefault();
        if (merchant == null)
            throw new MerchantNotFoundException(name);
        return merchant;

    }
}
public class MerchantMapperProfile : Profile
{
    public MerchantMapperProfile()
    {
        CreateMap<Merchants, MerchantListItem>()
            .ForMember(m => m.Id, o => o.MapFrom(src => src.Id))
            .ForMember(m => m.Name, o => o.MapFrom(src => src.name))
            .ForMember(m => m.City, o => o.MapFrom(src => src.city))
            .ForMember(m => m.IsOnline, o => o.MapFrom(src => src.is_online))
            .ReverseMap();

        CreateMap<AddEditMerchant, Merchants>()
            .ForMember(m => m.Id, o => o.MapFrom(src => src.Id))
            .ForMember(m => m.name, o => o.MapFrom(src => src.Name))
            .ForMember(m => m.is_online, o => o.MapFrom(src => src.IsOnline))
            .ForMember(m => m.city, o => o.MapFrom(src => src.City))
            .ForMember(m => m.state, o => o.MapFrom(src => src.State))
            .ForMember(m => m.notes, o => o.MapFrom(src => src.Notes))
            .ReverseMap();

    }
}