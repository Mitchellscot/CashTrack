using CashTrack.Data.Entities;
using CashTrack.Common.Exceptions;
using CashTrack.Models.IncomeCategoryModels;
using CashTrack.Repositories.IncomeCategoryRepository;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CashTrack.Repositories.IncomeRepository;

namespace CashTrack.Services.IncomeCategoryService;

public interface IIncomeCategoryService
{
    Task<IncomeCategoryResponse> GetIncomeCategoriesAsync(IncomeCategoryRequest request);
    Task<int> CreateIncomeCategoryAsync(IncomeCategory request);
    Task<int> UpdateIncomeCategoryAsync(IncomeCategory request);
    Task<bool> DeleteIncomeCategoryAsync(int id);
    Task<IncomeCategoryDropdownSelection[]> GetIncomeCategoryDropdownListAsync();
    Task<string[]> GetIncomeCategoryNames();
    Task<bool> CheckIfIncomeCategoryIsRefund(int categoryId);
    Task<IncomeCategoryEntity> GetIncomeCategoryByNameAsync(string name);
}
public class IncomeCategoryService : IIncomeCategoryService
{
    private readonly IIncomeCategoryRepository _repo;
    private readonly IIncomeRepository _incomeRepo;

    public IncomeCategoryService(IIncomeCategoryRepository repo, IIncomeRepository incomeRepo) => (_repo, _incomeRepo) = (repo, incomeRepo);

    public async Task<int> CreateIncomeCategoryAsync(IncomeCategory request)
    {
        var categories = await _repo.Find(x => true);
        if (categories.Any(x => x.Name == request.Name))
            throw new DuplicateNameException(nameof(IncomeCategoryEntity), request.Name);
        var incomeCategoryEntity = new IncomeCategoryEntity()
        {
            Name = request.Name,
            Notes = request.Notes,
            InUse = request.InUse
        };
        return await _repo.Create(incomeCategoryEntity);
    }

    public async Task<bool> DeleteIncomeCategoryAsync(int id)
    {
        var category = await _repo.FindById(id);
        if (category == null)
            throw new CategoryNotFoundException(id.ToString());
        if (category.Name == "Uncategorized")
            throw new Exception("You cannot delete this category. It's kind of important.");

        var uncategorizedCategory = (await _repo.Find(x => x.Name == "Uncategorized")).FirstOrDefault();
        if (uncategorizedCategory == null)
        {
            throw new Exception("You need to create a new category called 'Uncategorized' before you can delete a category. This will assigned all income associated with this category to the 'Uncategorized' category.");
        }
        var incomes = await _incomeRepo.Find(x => x.CategoryId == id);
        foreach (var income in incomes)
        {
            income.CategoryId = uncategorizedCategory.Id;
        }

        return await _repo.Delete(category);
    }

    public async Task<IncomeCategoryResponse> GetIncomeCategoriesAsync(IncomeCategoryRequest request)
    {
        Expression<Func<IncomeCategoryEntity, bool>> returnAll = (IncomeCategoryEntity x) => true;
        Expression<Func<IncomeCategoryEntity, bool>> searchCategories = (IncomeCategoryEntity x) => x.Name.ToLower().Contains(request.Query.ToLower());

        var predicate = request.Query == null ? returnAll : searchCategories;

        var categories = await _repo.FindWithPagination(predicate, request.PageNumber, request.PageSize);
        var count = await _repo.GetCount(predicate);

        var categoryListItems = categories.Select(x => new IncomeCategoryListItem()
        {
            Id = x.Id,
            Name = x.Name
        }).ToArray();

        var response = new IncomeCategoryResponse(request.PageNumber, request.PageSize, count, categoryListItems);

        return response;
    }

    public async Task<string[]> GetIncomeCategoryNames()
    {
        return (await _repo.Find(x => x.InUse)).Select(x => x.Name).ToArray();
    }

    public async Task<IncomeCategoryDropdownSelection[]> GetIncomeCategoryDropdownListAsync()
    {
        return (await _repo.Find(x => x.InUse == true)).Select(x => new IncomeCategoryDropdownSelection()
        {
            Id = x.Id,
            Category = x.Name
        }).ToArray();
    }

    public async Task<int> UpdateIncomeCategoryAsync(IncomeCategory request)
    {
        var categories = await _repo.Find(x => x.Name == request.Name);
        if (categories.Any(x => x.Id != request.Id))
            throw new DuplicateNameException(request.Name, nameof(IncomeCategoryEntity));

        var category = (await _repo.Find(x => x.Id == request.Id.Value)).FirstOrDefault();
        if (category == null)
            throw new CategoryNotFoundException(request.Id.Value.ToString());

        category.Name = request.Name;
        category.Notes = request.Notes;
        category.InUse = request.InUse;

        return await _repo.Update(category);
    }

    public async Task<bool> CheckIfIncomeCategoryIsRefund(int categoryId)
    {
        var refundCategoryId = (await _repo.Find(x => x.Name == "Refund")).FirstOrDefault().Id;
        return refundCategoryId == categoryId;
    }

    public async Task<IncomeCategoryEntity> GetIncomeCategoryByNameAsync(string name)
    {
        var category = (await _repo.Find(x => x.Name == name)).FirstOrDefault();
        if (category == null)
            throw new CategoryNotFoundException(name);
        return category;
    }
}

