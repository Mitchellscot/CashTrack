using AutoMapper;
using CashTrack.Common.Exceptions;
using CashTrack.Data.Entities;
using CashTrack.Models.Common;
using CashTrack.Models.ExpenseModels;
using CashTrack.Models.TagModels;
using CashTrack.Repositories.ExpenseRepository;
using CashTrack.Repositories.MerchantRepository;
using CashTrack.Repositories.SubCategoriesRepository;
using CashTrack.Services.Common;
using CashTrack.Services.MerchantService;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CashTrack.Services.ExpenseService;

public interface IExpenseService
{
    Task<Expenses> GetExpenseByIdAsync(int id);
    Task<ExpenseResponse> GetExpensesAsync(ExpenseRequest request);
    Task<ExpenseResponse> GetExpensesByNotesAsync(ExpenseRequest request);
    Task<ExpenseResponse> GetExpensesByAmountAsync(AmountSearchRequest request);
    Task<ExpenseResponse> GetExpensesBySubCategoryIdAsync(ExpenseRequest request);
    Task<ExpenseResponse> GetExpensesByMerchantAsync(ExpenseRequest request);
    Task<ExpenseResponse> GetExpensesByMainCategoryAsync(ExpenseRequest request);
    Task<bool> CreateExpenseAsync(Expense request);
    Task<bool> CreateExpenseFromSplitAsync(ExpenseSplit request);
    Task<bool> UpdateExpenseAsync(Expense request);
    Task<bool> DeleteExpenseAsync(int id);
}
public class ExpenseService : IExpenseService
{
    private readonly IExpenseRepository _expenseRepo;
    private readonly IMapper _mapper;

    public ExpenseService(IExpenseRepository expenseRepository, IMapper mapper)
    {
        _expenseRepo = expenseRepository;
        _mapper = mapper;
    }
    public async Task<Expenses> GetExpenseByIdAsync(int id)
    {
        var expense = await _expenseRepo.FindById(id);
        return expense;
    }
    public async Task<ExpenseResponse> GetExpensesAsync(ExpenseRequest request)
    {
        var predicate = DateOption<Expenses, ExpenseRequest>.Parse(request);
        var expenses = await _expenseRepo.FindWithPagination(predicate, request.PageNumber, request.PageSize);
        var count = await _expenseRepo.GetCount(predicate);
        var amount = await _expenseRepo.GetAmountOfExpenses(predicate);

        return new ExpenseResponse(request.PageNumber, request.PageSize, count, _mapper.Map<Expense[]>(expenses), amount);
    }
    public async Task<ExpenseResponse> GetExpensesByNotesAsync(ExpenseRequest request)
    {
        Expression<Func<Expenses, bool>> predicate = x => x.notes.ToLower().Contains(request.Query.ToLower());
        var expenses = await _expenseRepo.FindWithPagination(predicate, request.PageNumber, request.PageSize);
        var count = await _expenseRepo.GetCount(predicate);
        var amount = await _expenseRepo.GetAmountOfExpenses(predicate);

        return new ExpenseResponse(request.PageNumber, request.PageSize, count, _mapper.Map<Expense[]>(expenses), amount);
    }
    public async Task<ExpenseResponse> GetExpensesBySubCategoryIdAsync(ExpenseRequest request)
    {
        Expression<Func<Expenses, bool>> predicate = x => x.category.Id == int.Parse(request.Query);
        var expenses = await _expenseRepo.FindWithPagination(predicate, request.PageNumber, request.PageSize);
        var count = await _expenseRepo.GetCount(predicate);
        var amount = await _expenseRepo.GetAmountOfExpenses(predicate);

        return new ExpenseResponse(request.PageNumber, request.PageSize, count, _mapper.Map<Expense[]>(expenses), amount);
    }
    public async Task<ExpenseResponse> GetExpensesByMerchantAsync(ExpenseRequest request)
    {
        Expression<Func<Expenses, bool>> predicate = x => x.merchant.name.ToLower() == request.Query.ToLower();
        var expenses = await _expenseRepo.FindWithPagination(predicate, request.PageNumber, request.PageSize);
        var count = await _expenseRepo.GetCount(predicate);
        var amount = await _expenseRepo.GetAmountOfExpenses(predicate);

        return new ExpenseResponse(request.PageNumber, request.PageSize, count, _mapper.Map<Expense[]>(expenses), amount);
    }
    public async Task<ExpenseResponse> GetExpensesByAmountAsync(AmountSearchRequest request)
    {
        Expression<Func<Expenses, bool>> predicate = x => x.amount == request.Query;
        var expenses = await _expenseRepo.FindWithPagination(x => x.amount == request.Query, request.PageNumber, request.PageSize);
        var count = await _expenseRepo.GetCount(predicate);
        var amount = await _expenseRepo.GetAmountOfExpenses(predicate);
        return new ExpenseResponse(request.PageNumber, request.PageSize, count, _mapper.Map<Expense[]>(expenses), amount);
    }
    public async Task<bool> CreateExpenseAsync(Expense request)
    {
        if (string.IsNullOrEmpty(request.SubCategory))
            throw new CategoryNotFoundException("null");
        var expenseEntity = new Expenses()
        {
            amount = request.Amount,
            date = request.Date,
            categoryid = int.Parse(request.SubCategory),
            exclude_from_statistics = request.ExcludeFromStatistics,
            notes = request.Notes,
            merchantid = string.IsNullOrEmpty(request.Merchant) ? null : int.Parse(request.Merchant)
            //add expense_tags in the future
        };

        return await _expenseRepo.Create(expenseEntity);
    }
    public async Task<bool> CreateExpenseFromSplitAsync(ExpenseSplit request)
    {
        var expenseEntity = new Expenses()
        {
            date = request.Date,
            merchantid = int.Parse(request.Merchant),
            amount = request.Amount,
            notes = request.Notes,
            categoryid = request.SubCategoryId,
            //when spliting an expense, default is to not exclude from statistics
            //since that is only used in massive purchases (cars, mortgage down payments, etc.)
            exclude_from_statistics = false
            //TODO: add a way to add and delete tags here
        };
        return await _expenseRepo.Create(expenseEntity);
    }

    public async Task<bool> UpdateExpenseAsync(Expense request)
    {
        if (request.Id == null)
            throw new ArgumentException("Need an id to update an expense");

        if (string.IsNullOrEmpty(request.SubCategory))
            throw new CategoryNotFoundException("null");

        var currentExpense = await _expenseRepo.Find(x => x.Id == request.Id.Value);
        if (currentExpense == null)
            throw new ExpenseNotFoundException(request.Id.Value.ToString());

        var expenseEntity = new Expenses()
        {
            Id = request.Id.Value,
            amount = request.Amount,
            date = request.Date,
            categoryid = int.Parse(request.SubCategory),
            exclude_from_statistics = request.ExcludeFromStatistics,
            notes = request.Notes,
            merchantid = string.IsNullOrEmpty(request.Merchant) ? null : int.Parse(request.Merchant)
            //add expense_tags in the future
        };

        return await _expenseRepo.Update(expenseEntity);
    }
    public async Task<bool> DeleteExpenseAsync(int id)
    {
        var expense = await _expenseRepo.FindById(id);

        return await _expenseRepo.Delete(expense);
    }

    public async Task<ExpenseResponse> GetExpensesByMainCategoryAsync(ExpenseRequest request)
    {
        Expression<Func<Expenses, bool>> predicate = x => x.category.main_categoryid == int.Parse(request.Query);
        var expenses = await _expenseRepo.FindWithPagination(predicate, request.PageNumber, request.PageSize);
        var count = await _expenseRepo.GetCount(predicate);
        var amount = await _expenseRepo.GetAmountOfExpenses(predicate);

        return new ExpenseResponse(request.PageNumber, request.PageSize, count, _mapper.Map<Expense[]>(expenses), amount);
    }


}
public class ExpenseMapperProfile : Profile
{
    public ExpenseMapperProfile()
    {
        //from stack overflow
        //For the 1 millionth time: AutoMapper isn't meant for ViewModel -> Persistence/Domain Model conversions.
        //It's for the other way (Domain/Persistance -> Dto/ViewModel).

        CreateMap<Expenses, Expense>()
            .ForMember(e => e.Id, o => o.MapFrom(src => src.Id))
            .ForMember(e => e.Date, o => o.MapFrom(src => src.date))
            .ForMember(e => e.Amount, o => o.MapFrom(src => src.amount))
            .ForMember(e => e.Notes, o => o.MapFrom(src => src.notes))
            .ForMember(e => e.Merchant, o => o.MapFrom(src => src.merchant.name))
            .ForMember(e => e.SubCategory, o => o.MapFrom(src => src.category.sub_category_name))
            .ForMember(e => e.MainCategory, o => o.MapFrom(src => src.category.main_category.main_category_name))
            .ForMember(e => e.ExcludeFromStatistics, o => o.MapFrom(src => src.exclude_from_statistics))
            .ForMember(e => e.Tags, o => o.MapFrom(
                src => src.expense_tags.Select(a => new TagModel() { Id = a.tag_id, TagName = a.tag.tag_name })));

        //goes it Tags Service when created
        CreateMap<Tags, TagModel>()
            .ForMember(t => t.Id, o => o.MapFrom(src => src.Id))
            .ForMember(t => t.TagName, o => o.MapFrom(src => src.tag_name))
            .ReverseMap();
    }
}
