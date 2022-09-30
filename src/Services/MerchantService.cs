using AutoMapper;
using CashTrack.Data.Entities;
using CashTrack.Common.Exceptions;
using CashTrack.Models.ExpenseModels;
using CashTrack.Models.MerchantModels;
using CashTrack.Repositories.ExpenseRepository;
using CashTrack.Repositories.MerchantRepository;
using CashTrack.Repositories.SubCategoriesRepository;
using CashTrack.Services.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using CashTrack.Models.Common;

namespace CashTrack.Services.MerchantService;

public interface IMerchantService
{
    Task<MerchantResponse> GetMerchantsAsync(MerchantRequest request);
    Task<string[]> GetMatchingMerchantsAsync(string match);
    Task<MerchantDetail> GetMerchantDetailAsync(int id);
    Task<int> CreateMerchantAsync(Merchant request);
    Task<int> UpdateMerchantAsync(Merchant request);
    Task<bool> DeleteMerchantAsync(int id);
    Task<MerchantEntity> GetMerchantByNameAsync(string name);
    Task<string[]> GetAllMerchantNames();
    Task<MerchantDropdownSelection[]> GetMerchantDropdownListAsync();
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
        var merchantListItems = await ParseMerchantQuery(request);

        var count = await _merchantRepo.GetCount(x => true);

        return new MerchantResponse(request.PageNumber, request.PageSize, count, merchantListItems);
    }
    private async Task<List<MerchantListItem>> ParseMerchantQuery(MerchantRequest request)
    {
        var merchantViewModels = new List<MerchantListItem>();
        switch (request.Order)
        {
            case MerchantOrderBy.Name:
                var allMerchantsListByName = await GetMerchantListItems();

                merchantViewModels = request.Reversed ?
                    allMerchantsListByName.OrderByDescending(x => x.Name).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList() :
                    allMerchantsListByName.OrderBy(x => x.Name).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
                break;
            case MerchantOrderBy.Location:
                var allMerchantsListByLocation = await GetMerchantListItems();

                merchantViewModels = request.Reversed ?
                    allMerchantsListByLocation.OrderByDescending(x => x.Location).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList() :
                    allMerchantsListByLocation.OrderBy(x => x.Location).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
                break;
            case MerchantOrderBy.Purchases:
                var allMerchantsListByPurchases = await GetMerchantListItems();

                merchantViewModels = request.Reversed ?
                    allMerchantsListByPurchases.OrderByDescending(x => x.NumberOfExpenses).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList() :
                    allMerchantsListByPurchases.OrderBy(x => x.NumberOfExpenses).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
                break;
            case MerchantOrderBy.Amount:
                var allMerchantsListByTotal = await GetMerchantListItems();

                merchantViewModels = request.Reversed ?
                    allMerchantsListByTotal.OrderByDescending(x => x.TotalSpent).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList() :
                    allMerchantsListByTotal.OrderBy(x => x.TotalSpent).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
                break;
            case MerchantOrderBy.LastPurchase:
                var allMerchantsListByDate = await GetMerchantListItems();

                merchantViewModels = request.Reversed ?
                    allMerchantsListByDate.OrderByDescending(x => x.LastPurchase).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList() :
                    allMerchantsListByDate.OrderBy(x => x.LastPurchase).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
                break;
            case MerchantOrderBy.Category:
                var allMerchantsListByCategory = await GetMerchantListItems();

                merchantViewModels = request.Reversed ?
                    allMerchantsListByCategory.OrderByDescending(x => x.Category).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList() :
                    allMerchantsListByCategory.OrderBy(x => x.Category).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
                break;
        }
        return merchantViewModels;
    }
    private async Task<List<MerchantListItem>> GetMerchantListItems()
    {
        var expenses = await _expenseRepo.Find(x => true);
        var merchants = await _merchantRepo.Find(x => true);
        var categories = await _subCategoryRepo.Find(x => true);
        return expenses.GroupBy(e => e.MerchantId)
                    .Select(g =>
                    {
                        var results = g.Aggregate(new MerchantListItemAggregator(g.Key, merchants, categories), (acc, e) => acc.Accumulate(e), acc => acc.Compute());
                        return new MerchantListItem()
                        {
                            Id = g.Key.HasValue ? g.Key.Value : 0,
                            Name = results.Merchant != null ? results.Merchant.Name : null,
                            NumberOfExpenses = results.Purchases,
                            TotalSpent = results.Amount,
                            LastPurchase = results.LastPurchase,
                            Category = results.MostUsedCategory,
                            Location = results.Location
                        };
                    }).Where(x => x.Id > 0).ToList();
    }

    public async Task<MerchantDetail> GetMerchantDetailAsync(int id)
    {
        var merchant = await _merchantRepo.FindById(id);
        if (merchant == null)
            throw new MerchantNotFoundException(id.ToString());
        var expenses = await _expenseRepo.GetExpensesAndCategoriesByMerchantId(id);
        if (!expenses.Any())
            return new MerchantDetail()
            {
                Id = merchant.Id,
                Name = merchant.Name,
                SuggestOnLookup = merchant.SuggestOnLookup,
                City = merchant.City,
                State = merchant.State,
                Notes = merchant.Notes,
                IsOnline = merchant.IsOnline,
                ExpenseTotals = new Totals(),
                MostUsedCategory = "No expenses yet.",
                AnnualExpenseStatistics = new List<AnnualStatistics>(),
                MonthlyExpenseStatistics = new List<MonthlyStatistics>(),
                PurchaseCategoryOccurances = new Dictionary<string, int>(),
                PurchaseCategoryTotals = new Dictionary<string, decimal>(),
                RecentExpenses = new List<ExpenseQuickViewForMerchantDetail>(),
            };

        var subCategories = expenses.Select(x => x.Category).Distinct().ToArray();

        return new MerchantDetail()
        {
            Id = merchant.Id,
            Name = merchant.Name,
            SuggestOnLookup = merchant.SuggestOnLookup,
            City = merchant.City,
            State = merchant.State,
            Notes = merchant.Notes,
            IsOnline = merchant.IsOnline,

            ExpenseTotals = expenses.Aggregate(new TotalsAggregator<ExpenseEntity>(),
                (acc, e) => acc.Accumulate(e),
                acc => acc.Compute()),

            MostUsedCategory = GetExpenseCategoryOccurances(subCategories, expenses).FirstOrDefault().Key,

            AnnualExpenseStatistics = AggregateUtilities<ExpenseEntity>.GetAnnualStatistics(expenses),
            MonthlyExpenseStatistics = AggregateUtilities<ExpenseEntity>.GetStatisticsLast12Months(expenses),

            PurchaseCategoryOccurances = GetExpenseCategoryOccurances(subCategories, expenses),
            PurchaseCategoryTotals = GetExpenseCategoryTotals(subCategories, expenses),

            RecentExpenses = expenses.OrderByDescending(e => e.Date)
            .Take(9)
            .Select(x => new ExpenseQuickViewForMerchantDetail()
            {
                Id = x.Id,
                Date = x.Date.Date.ToShortDateString(),
                Amount = x.Amount,
                SubCategory = x.Category == null ? "none" : x.Category.Name
            }).ToList()
        };
    }
    internal Dictionary<string, int> GetExpenseCategoryOccurances(SubCategoryEntity[] subCategories, ExpenseEntity[] expenses)
    {
        return subCategories.GroupJoin(expenses,
        c => c.Id, e => e.Category.Id, (c, g) => new
        {
            Category = c.Name,
            Expenses = g
        }).Select(x => new
        {
            Category = x.Category,
            Count = x.Expenses.Count()
        }).OrderByDescending(x => x.Count).ToDictionary(k => k.Category, v => v.Count);
    }
    internal Dictionary<string, decimal> GetExpenseCategoryTotals(SubCategoryEntity[] subCategories, ExpenseEntity[] expenses)
    {
        return subCategories.GroupJoin(expenses,
            c => c.Id, e => e.Category.Id, (c, g) => new
            {
                Category = c.Name,
                Expenses = g
            }).Select(x => new
            {
                Category = x.Category,
                Sum = x.Expenses.Sum(e => e.Amount)
            }).Where(x => x.Sum > 0).OrderByDescending(x => x.Sum).ToDictionary(k => k.Category, v => v.Sum);
    }

    public async Task<int> CreateMerchantAsync(Merchant request)
    {
        var merchants = await _merchantRepo.Find(x => x.Name == request.Name);
        if (merchants.Any())
            throw new DuplicateNameException(nameof(MerchantEntity), request.Name);

        var merchantEntity = _mapper.Map<MerchantEntity>(request);

        return await _merchantRepo.Create(merchantEntity);
    }
    public async Task<int> UpdateMerchantAsync(Merchant request)
    {
        var merchants = await _merchantRepo.Find(x => x.Name == request.Name);
        if (merchants.Any(x => x.Id != request.Id))
            throw new DuplicateNameException(request.Name, nameof(MerchantEntity));

        var merchant = await _merchantRepo.FindById(request.Id.Value);

        merchant.Name = request.Name;
        merchant.City = request.IsOnline ? null : request.City;
        merchant.State = request.IsOnline ? null : request.State;
        merchant.Notes = request.Notes;
        merchant.IsOnline = request.IsOnline;
        merchant.SuggestOnLookup = request.SuggestOnLookup;

        return await _merchantRepo.Update(merchant);
    }

    public async Task<bool> DeleteMerchantAsync(int id)
    {
        var merchant = await _merchantRepo.FindById(id);
        if (merchant == null)
            throw new MerchantNotFoundException(id.ToString());
        var expenses = await _expenseRepo.Find(x => x.MerchantId == id);
        if (!expenses.Any())
            return await _merchantRepo.Delete(merchant);

        foreach (var expense in expenses)
        {
            expense.MerchantId = null;
            expense.Merchant = null;
        }
        var success = await _expenseRepo.UpdateMany(expenses.ToList());

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

    public async Task<MerchantDropdownSelection[]> GetMerchantDropdownListAsync()
    {
        return (await _merchantRepo.Find(x => x.SuggestOnLookup == true)).Select(x => new MerchantDropdownSelection()
        {
            Id = x.Id,
            Name = x.Name
        }).ToArray();
    }
}
public class MerchantMapperProfile : Profile
{
    public MerchantMapperProfile()
    {

        CreateMap<Merchant, MerchantEntity>()
            .ForMember(m => m.Id, o => o.MapFrom(src => src.Id))
            .ForMember(m => m.Name, o => o.MapFrom(src => src.Name))
            .ForMember(m => m.IsOnline, o => o.MapFrom(src => src.IsOnline))
            .ForMember(m => m.City, o => o.MapFrom(src => src.City))
            .ForMember(m => m.State, o => o.MapFrom(src => src.State))
            .ForMember(m => m.Notes, o => o.MapFrom(src => src.Notes))
            .ReverseMap();

    }
}