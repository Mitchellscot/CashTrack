using AutoMapper;
using CashTrack.Common.Exceptions;
using CashTrack.Data.Entities;
using CashTrack.Models.ExpenseModels;
using CashTrack.Models.TagModels;
using CashTrack.Repositories.ExpenseRepository;
using CashTrack.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CashTrack.Services.ExpenseService;

public interface IExpenseService
{
    Task<Expense> GetExpenseByIdAsync(int id);
    Task<ExpenseResponse> GetExpensesAsync(ExpenseRequest request);
    Task<ExpenseResponse> GetExpensesByNotesAsync(ExpenseRequest request);
    Task<ExpenseResponse> GetExpensesByAmountAsync(AmountSearchRequest request);
    Task<ExpenseResponse> GetExpensesBySubCategoryIdAsync(ExpenseRequest request);
    Task<ExpenseResponse> GetExpensesByMerchantAsync(ExpenseRequest request);
    Task<ExpenseResponse> GetExpensesByMainCategoryAsync(ExpenseRequest request);
    Task<Expense[]> GetExpensesByDateWithoutPaginationAsync(DateTime request);
    Task<ExpenseRefund> GetExpenseRefundByIdAsync(int id);
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
    public async Task<Expense> GetExpenseByIdAsync(int id)
    {
        var expense = await _expenseRepo.FindById(id);
        return _mapper.Map<Expense>(expense);
    }
    public async Task<ExpenseRefund> GetExpenseRefundByIdAsync(int id)
    {
        var expense = await _expenseRepo.FindById(id);
        return _mapper.Map<ExpenseRefund>(expense);
    }
    public async Task<ExpenseResponse> GetExpensesAsync(ExpenseRequest request)
    {
        var predicate = DateOption<ExpenseEntity, ExpenseRequest>.Parse(request);
        var expenses = await _expenseRepo.FindWithPagination(predicate, request.PageNumber, request.PageSize);
        var count = await _expenseRepo.GetCount(predicate);
        var amount = await _expenseRepo.GetAmountOfExpenses(predicate);

        return new ExpenseResponse(request.PageNumber, request.PageSize, count, _mapper.Map<Expense[]>(expenses), amount);
    }
    public async Task<ExpenseResponse> GetExpensesByNotesAsync(ExpenseRequest request)
    {
        Expression<Func<ExpenseEntity, bool>> predicate = x => x.Notes.ToLower().Contains(request.Query.ToLower());
        var expenses = await _expenseRepo.FindWithPagination(predicate, request.PageNumber, request.PageSize);
        var count = await _expenseRepo.GetCount(predicate);
        var amount = await _expenseRepo.GetAmountOfExpenses(predicate);

        return new ExpenseResponse(request.PageNumber, request.PageSize, count, _mapper.Map<Expense[]>(expenses), amount);
    }
    public async Task<ExpenseResponse> GetExpensesBySubCategoryIdAsync(ExpenseRequest request)
    {
        Expression<Func<ExpenseEntity, bool>> predicate = x => x.Category.Id == int.Parse(request.Query);
        var expenses = await _expenseRepo.FindWithPagination(predicate, request.PageNumber, request.PageSize);
        var count = await _expenseRepo.GetCount(predicate);
        var amount = await _expenseRepo.GetAmountOfExpenses(predicate);

        return new ExpenseResponse(request.PageNumber, request.PageSize, count, _mapper.Map<Expense[]>(expenses), amount);
    }
    public async Task<ExpenseResponse> GetExpensesByMerchantAsync(ExpenseRequest request)
    {
        Expression<Func<ExpenseEntity, bool>> predicate = x => x.Merchant.Name.ToLower() == request.Query.ToLower();
        var expenses = await _expenseRepo.FindWithPagination(predicate, request.PageNumber, request.PageSize);
        var count = await _expenseRepo.GetCount(predicate);
        var amount = await _expenseRepo.GetAmountOfExpenses(predicate);

        return new ExpenseResponse(request.PageNumber, request.PageSize, count, _mapper.Map<Expense[]>(expenses), amount);
    }
    public async Task<ExpenseResponse> GetExpensesByAmountAsync(AmountSearchRequest request)
    {
        Expression<Func<ExpenseEntity, bool>> predicate = x => x.Amount == request.Query;
        var expenses = await _expenseRepo.FindWithPagination(x => x.Amount == request.Query, request.PageNumber, request.PageSize);
        var count = await _expenseRepo.GetCount(predicate);
        var amount = await _expenseRepo.GetAmountOfExpenses(predicate);
        return new ExpenseResponse(request.PageNumber, request.PageSize, count, _mapper.Map<Expense[]>(expenses), amount);
    }
    public async Task<bool> CreateExpenseAsync(Expense request)
    {
        var expenseEntity = new ExpenseEntity()
        {
            Amount = request.Amount,
            Date = request.Date,
            CategoryId = request.SubCategoryId,
            ExcludeFromStatistics = request.ExcludeFromStatistics,
            Notes = request.Notes,
            MerchantId = string.IsNullOrEmpty(request.Merchant) ? null : int.Parse(request.Merchant)
            //add expense_tags in the future
        };

        return await _expenseRepo.Create(expenseEntity);
    }
    public async Task<bool> CreateExpenseFromSplitAsync(ExpenseSplit request)
    {
        var expenseEntity = new ExpenseEntity()
        {
            Date = request.Date,
            MerchantId = string.IsNullOrEmpty(request.Merchant) ? null : int.Parse(request.Merchant),
            Amount = request.Amount,
            Notes = request.Notes,
            CategoryId = request.SubCategoryId,
            //when spliting an expense, default is to not exclude from statistics
            //since that is only used in massive purchases (cars, mortgage down payments, etc.)
            ExcludeFromStatistics = false
            //TODO: add a way to add and delete tags here
        };
        return await _expenseRepo.Create(expenseEntity);
    }

    public async Task<bool> UpdateExpenseAsync(Expense request)
    {
        if (request.Id == null)
            throw new ArgumentException("Need an id to update an expense");

        var currentExpense = await _expenseRepo.Find(x => x.Id == request.Id.Value);
        if (currentExpense == null)
            throw new ExpenseNotFoundException(request.Id.Value.ToString());

        var expenseEntity = new ExpenseEntity()
        {
            Id = request.Id.Value,
            Amount = request.Amount,
            Date = request.Date,
            CategoryId = request.SubCategoryId,
            ExcludeFromStatistics = request.ExcludeFromStatistics,
            Notes = request.Notes,
            MerchantId = string.IsNullOrEmpty(request.Merchant) ? null : int.Parse(request.Merchant)
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
        Expression<Func<ExpenseEntity, bool>> predicate = x => x.Category.MainCategoryId == int.Parse(request.Query);
        var expenses = await _expenseRepo.FindWithPagination(predicate, request.PageNumber, request.PageSize);
        var count = await _expenseRepo.GetCount(predicate);
        var amount = await _expenseRepo.GetAmountOfExpenses(predicate);

        return new ExpenseResponse(request.PageNumber, request.PageSize, count, _mapper.Map<Expense[]>(expenses), amount);
    }
    public async Task<Expense[]> GetExpensesByDateWithoutPaginationAsync(DateTime request)
    {
        var expenses = await _expenseRepo.Find(x => x.Date == request);
        return _mapper.Map<Expense[]>(expenses);
    }
}
public class ExpenseMapperProfile : Profile
{
    public ExpenseMapperProfile()
    {
        //from stack overflow
        //For the 1 millionth time: AutoMapper isn't meant for ViewModel -> Persistence/Domain Model conversions.
        //It's for the other way (Domain/Persistance -> Dto/ViewModel).

        CreateMap<ExpenseEntity, ExpenseRefund>()
            .ForMember(e => e.Id, o => o.MapFrom(src => src.Id))
            .ForMember(e => e.Date, o => o.MapFrom(src => src.Date))
            .ForMember(e => e.OriginalAmount, o => o.MapFrom(src => src.Amount))
            .ForMember(e => e.Category, o => o.MapFrom(src => src.Category.Name))
            .ForMember(e => e.Merchant, o => o.MapFrom(src => src.Merchant.Name));


        CreateMap<ExpenseEntity, Expense>()
            .ForMember(e => e.Id, o => o.MapFrom(src => src.Id))
            .ForMember(e => e.Date, o => o.MapFrom(src => src.Date))
            .ForMember(e => e.Amount, o => o.MapFrom(src => src.Amount))
            .ForMember(e => e.Notes, o => o.MapFrom(src => src.Notes))
            .ForMember(e => e.Merchant, o => o.MapFrom(src => src.Merchant.Name))
            .ForMember(e => e.SubCategory, o => o.MapFrom(src => src.Category.Name))
            .ForMember(e => e.SubCategoryId, o => o.MapFrom(src => src.Category.Id))
            .ForMember(e => e.MainCategory, o => o.MapFrom(src => src.Category.MainCategory.Name))
            .ForMember(e => e.ExcludeFromStatistics, o => o.MapFrom(src => src.ExcludeFromStatistics))
            .ForMember(e => e.Tags, o => o.MapFrom(
                src => src.ExpenseTags.Select(a => new TagModel() { Id = a.TagId, TagName = a.Tag.Name })));

        //goes it Tags Service when created
        CreateMap<TagEntity, TagModel>()
            .ForMember(t => t.Id, o => o.MapFrom(src => src.Id))
            .ForMember(t => t.TagName, o => o.MapFrom(src => src.Name))
            .ReverseMap();
    }
}
