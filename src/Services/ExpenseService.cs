using AutoMapper;
using CashTrack.Common;
using CashTrack.Common.Exceptions;
using CashTrack.Common.Extensions;
using CashTrack.Data.Entities;
using CashTrack.Models.Common;
using CashTrack.Models.ExpenseModels;
using CashTrack.Models.TagModels;
using CashTrack.Repositories.ExpenseRepository;
using CashTrack.Repositories.IncomeRepository;
using CashTrack.Repositories.MerchantRepository;
using CashTrack.Repositories.SubCategoriesRepository;
using CashTrack.Services.Common;
using Microsoft.AspNetCore.Hosting;
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
    Task<int> CreateExpenseAsync(Expense request);
    Task<int> CreateExpenseFromSplitAsync(ExpenseSplit request);
    Task<int> UpdateExpenseAsync(Expense request);
    Task<bool> DeleteExpenseAsync(int id);
    Task<bool> RefundExpensesAsync(List<ExpenseRefund> refunds, int incomeId);
    Task<int[]> GetAnnualSummaryYearsAsync();
}
public class ExpenseService : IExpenseService
{
    private readonly ISubCategoryRepository _subCategoryRepo;
    private readonly IIncomeRepository _incomeRepo;
    private readonly IExpenseRepository _expenseRepo;
    private readonly IMerchantRepository _merchantRepo;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _env;

    public ExpenseService(IExpenseRepository expenseRepository, IIncomeRepository incomeRepository, IMerchantRepository merchantRepository, IMapper mapper, ISubCategoryRepository subCategoryRepository, IWebHostEnvironment env)
    {
        _subCategoryRepo = subCategoryRepository;
        _incomeRepo = incomeRepository;
        _expenseRepo = expenseRepository;
        _merchantRepo = merchantRepository;
        _mapper = mapper;
        _env = env;
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
    public async Task<int> CreateExpenseAsync(Expense request)
    {
        if (request.Amount <= 0)
            throw new ArgumentException("Expense amount cannot be zero.");

        int? merchantId = 0;
        if (request.Merchant != null)
        {
            var merchant = (await _merchantRepo.Find(x => x.Name == request.Merchant)).FirstOrDefault();
            if (merchant == null)
                throw new MerchantNotFoundException(nameof(request.Merchant));
            merchantId = merchant.Id;
        }


        var expenseEntity = new ExpenseEntity()
        {
            Amount = request.Amount,
            Date = request.Date,
            CategoryId = request.SubCategoryId,
            ExcludeFromStatistics = request.ExcludeFromStatistics,
            Notes = request.Notes,
            MerchantId = merchantId > 0 ? merchantId : null,
            //add expense_tags in the future
        };

        return await _expenseRepo.Create(expenseEntity);
    }
    public async Task<int> CreateExpenseFromSplitAsync(ExpenseSplit request)
    {
        request.SetTaxIfTaxed();
        var expenseEntity = new ExpenseEntity()
        {
            Date = request.Date,
            MerchantId = string.IsNullOrEmpty(request.Merchant) ? null : (await _merchantRepo.Find(x => x.Name == request.Merchant)).FirstOrDefault().Id,
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

    public async Task<int> UpdateExpenseAsync(Expense request)
    {
        if (request.Id == null)
            throw new ArgumentException("Need an Id to update an expense");

        if (request.SubCategoryId == 0)
            throw new CategoryNotFoundException("null");

        var currentExpense = await _expenseRepo.FindById(request.Id.Value);
        if (currentExpense == null)
            throw new ExpenseNotFoundException(request.Id.Value.ToString());

        var categoryId = (await _subCategoryRepo.FindById(request.SubCategoryId)).Id;

        currentExpense.Amount = request.Amount;
        currentExpense.Date = request.Date;
        currentExpense.Notes = request.Notes;
        currentExpense.CategoryId = categoryId;
        currentExpense.ExcludeFromStatistics = request.ExcludeFromStatistics;
        currentExpense.RefundNotes = request.RefundNotes;
        currentExpense.RefundNotes = request.RefundNotes;
        currentExpense.MerchantId = request.MerchantId;

        if (request.Merchant != null)
            currentExpense.MerchantId = (await _merchantRepo.Find(x => x.Name == request.Merchant)).FirstOrDefault().Id;

        //TODO: Setup up tags in the future
        //currentExpense.ExpenseTags

        return await _expenseRepo.Update(currentExpense);
    }
    public async Task<bool> DeleteExpenseAsync(int id)
    {
        var expense = await _expenseRepo.FindById(id);
        if (expense == null)
            throw new ExpenseNotFoundException("Invalid expense Id");

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
        Expression<Func<ExpenseEntity, bool>> predicate = x => x.Date.Date.CompareTo(request.Date) == 0;
        if (_env.EnvironmentName.IsEqualTo("Test"))
        {
            //sqllite DB treats dates differently...
            predicate = x => x.Date == request.Date;
        }
        var expenses = await _expenseRepo.Find(predicate);
        return _mapper.Map<Expense[]>(expenses);
    }
    public async Task<bool> RefundExpensesAsync(List<ExpenseRefund> refunds, int incomeId)
    {
        var income = await _incomeRepo.FindById(incomeId);
        var expenseUpdates = new List<ExpenseEntity>();
        foreach (var refund in refunds)
        {
            if (refund.RefundAmount != default(decimal))
            {
                var expense = await _expenseRepo.FindById(refund.Id);
                expense.RefundNotes = $"Original Amount: {refund.OriginalAmount} - Refunded Amount: {refund.RefundAmount} - Date Refunded: {income.Date.Date.ToShortDateString()}";
                expense.Amount = Decimal.Round((refund.OriginalAmount - refund.RefundAmount), 2);
                income.RefundNotes += $"Applied refund for the amount of {refund.RefundAmount} to an expense on {refund.Date.Date.ToShortDateString()}. ";
                expenseUpdates.Add(expense);
            }
        }
        if (expenseUpdates.Any())
        {
            income.IsRefund = true;
            var updateIncome = await _incomeRepo.Update(income);
            return await _expenseRepo.UpdateMany(expenseUpdates);
        }
        else return false;
    }

    public async Task<int[]> GetAnnualSummaryYearsAsync()
    {
        var expenseYears = (await _expenseRepo.Find(x => true)).GroupBy(x => x.Date.Year).Select(x => x.Key).ToList();
        var incomeYears = (await _incomeRepo.Find(x => true)).GroupBy(x => x.Date.Year).Select(x => x.Key).ToList();
        incomeYears.AddRange(expenseYears);
        var years = incomeYears.Distinct().ToList();
        var currentYear = DateTime.Now.Year;
        if (!years.Contains(currentYear))
            years.Add(currentYear);
        return years.OrderBy(x => x).ToArray();
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
            .ForMember(e => e.MerchantId, o => o.MapFrom(src => src.MerchantId))
            .ForMember(e => e.MerchantId, o => o.NullSubstitute(null))
            .ForMember(e => e.SubCategory, o => o.MapFrom(src => src.Category.Name))
            .ForMember(e => e.SubCategoryId, o => o.MapFrom(src => src.Category.Id))
            .ForMember(e => e.MainCategory, o => o.MapFrom(src => src.Category.MainCategory.Name))
            .ForMember(e => e.ExcludeFromStatistics, o => o.MapFrom(src => src.ExcludeFromStatistics))
            .ForMember(e => e.RefundNotes, o => o.MapFrom(src => src.RefundNotes))
            .ForMember(e => e.Tags, o => o.MapFrom(
                src => src.ExpenseTags.Select(a => new TagModel() { Id = a.TagId, TagName = a.Tag.Name })));
    }
}
