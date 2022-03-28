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
    Task<bool> CreateMerchantAsync(AddEditMerchant request);
    Task<bool> UpdateMerchantAsync(AddEditMerchant request);
    Task<bool> DeleteMerchantAsync(int id);
    Task<MerchantEntity> GetMerchantByNameAsync(string name);
    Task<string[]> GetAllMerchantNames();
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
        Expression<Func<MerchantEntity, bool>> allMerchants = (MerchantEntity m) => true;
        Expression<Func<MerchantEntity, bool>> merchantSearch = (MerchantEntity m) => m.Name.ToLower().Contains(request.Query.ToLower());

        var predicate = request.Query == null ? allMerchants : merchantSearch;

        var merchantEntities = await _merchantRepo.FindWithPagination(predicate, request.PageNumber, request.PageSize);

        var count = await _merchantRepo.GetCount(predicate);

        var merchantViewModels = merchantEntities.Select(m => new MerchantListItem
        {
            Id = m.Id,
            Name = m.Name,
            City = m.City,
            IsOnline = m.IsOnline,
            NumberOfExpenses = _expenseRepo.GetCount(x => x.MerchantId == m.Id).Result
        }).ToArray();

        return new MerchantResponse(request.PageNumber, request.PageSize, count, merchantViewModels);
    }

    public async Task<MerchantDetail> GetMerchantDetailAsync(int id)
    {
        var merchantEntity = await _merchantRepo.FindById(id);

        var merchantExpenses = await _expenseRepo.GetExpensesAndCategories(x => x.MerchantId == id);

        var recentExpenses = merchantExpenses.OrderByDescending(e => e.Date)
            .Take(10)
            .Select(x => new ExpenseQuickView()
            {
                Id = x.Id,
                Date = x.Date.Date.ToShortDateString(),
                Amount = x.Amount,
                SubCategory = x.Category == null ? "none" : x.Category.Name
            }).ToList();

        var expenseTotals = merchantExpenses.Aggregate(new ExpenseTotalsAggregator(),
                (acc, e) => acc.Accumulate(e),
                acc => acc.Compute());

        var expenseStatistics = merchantExpenses.GroupBy(e => e.Date.Year)
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
            c => c.Id, e => e.Category.Id, (c, g) => new
            {
                Category = c.Name,
                Expenses = g
            }).Select(x => new
            {
                Category = x.Category,
                Count = x.Expenses.Count()
            }).Where(x => x.Count > 0).OrderByDescending(x => x.Count).ToDictionary(k => k.Category, v => v.Count);

        var merchantExpenseAmounts = subCategories.GroupJoin(merchantExpenses,
            c => c.Id, e => e.Category.Id, (c, g) => new
            {
                Category = c.Name,
                Expenses = g
            }).Select(x => new
            {
                Category = x.Category,
                Sum = x.Expenses.Sum(e => e.Amount)
            }).Where(x => x.Sum > 0).OrderByDescending(x => x.Sum).ToDictionary(k => k.Category, v => v.Sum);

        var mostUsedCategory = merchantExpenseCategories.FirstOrDefault().Key;

        var merchantDetail = new MerchantDetail()
        {
            Id = merchantEntity.Id,
            Name = merchantEntity.Name,
            SuggestOnLookup = merchantEntity.SuggestOnLookup,
            City = merchantEntity.City,
            State = merchantEntity.State,
            Notes = merchantEntity.Notes,
            IsOnline = merchantEntity.IsOnline,
            ExpenseTotals = expenseTotals,
            MostUsedCategory = mostUsedCategory,
            AnnualExpenseStatistics = expenseStatistics,
            PurchaseCategoryOccurances = merchantExpenseCategories,
            PurchaseCategoryTotals = merchantExpenseAmounts,
            RecentExpenses = recentExpenses
        };

        return merchantDetail;
    }

    public async Task<bool> CreateMerchantAsync(AddEditMerchant request)
    {
        var merchants = await _merchantRepo.Find(x => x.Name == request.Name);
        if (merchants.Any())
            throw new DuplicateNameException(nameof(MerchantEntity), request.Name);

        var merchantEntity = _mapper.Map<MerchantEntity>(request);

        return await _merchantRepo.Create(merchantEntity);
    }
    public async Task<bool> UpdateMerchantAsync(AddEditMerchant request)
    {
        var merchants = await _merchantRepo.Find(x => x.Name == request.Name);
        if (merchants.Any())
            throw new DuplicateNameException(nameof(MerchantEntity), request.Name);

        var merchant = _mapper.Map<MerchantEntity>(request);
        return await _merchantRepo.Update(merchant);
    }

    public async Task<bool> DeleteMerchantAsync(int id)
    {
        var merchant = await _merchantRepo.FindById(id);

        return await _merchantRepo.Delete(merchant);
    }

    public async Task<string[]> GetMatchingMerchantsAsync(string match)
    {
        return (await _merchantRepo.Find(x => x.Name.StartsWith(match) && x.SuggestOnLookup == true)).Select(x => x.Name).Take(10).ToArray();
    }

    public async Task<MerchantEntity> GetMerchantByNameAsync(string name)
    {
        var merchant = (await _merchantRepo.Find(x => x.Name == name)).FirstOrDefault();
        if (merchant == null)
            throw new MerchantNotFoundException(name);
        return merchant;

    }
    public async Task<string[]> GetAllMerchantNames()
    {
        return (await _merchantRepo.Find(x => true)).Select(x => x.Name).ToArray();
    }
}
public class MerchantMapperProfile : Profile
{
    public MerchantMapperProfile()
    {
        CreateMap<MerchantEntity, MerchantListItem>()
            .ForMember(m => m.Id, o => o.MapFrom(src => src.Id))
            .ForMember(m => m.Name, o => o.MapFrom(src => src.Name))
            .ForMember(m => m.City, o => o.MapFrom(src => src.City))
            .ForMember(m => m.IsOnline, o => o.MapFrom(src => src.IsOnline))
            .ReverseMap();

        CreateMap<AddEditMerchant, MerchantEntity>()
            .ForMember(m => m.Id, o => o.MapFrom(src => src.Id))
            .ForMember(m => m.Name, o => o.MapFrom(src => src.Name))
            .ForMember(m => m.IsOnline, o => o.MapFrom(src => src.IsOnline))
            .ForMember(m => m.City, o => o.MapFrom(src => src.City))
            .ForMember(m => m.State, o => o.MapFrom(src => src.State))
            .ForMember(m => m.Notes, o => o.MapFrom(src => src.Notes))
            .ReverseMap();

    }
}