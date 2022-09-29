using CashTrack.Data.Entities;
using CashTrack.Common.Exceptions;
using CashTrack.Models.IncomeCategoryModels;
using CashTrack.Repositories.IncomeCategoryRepository;
using System;
using System.Linq;
using System.Threading.Tasks;
using CashTrack.Repositories.IncomeRepository;
using CashTrack.Services.Common;

namespace CashTrack.Services.IncomeCategoryService;

public interface IIncomeCategoryService
{
    Task<IncomeCategoryDetail> GetCategoryDetailAsync(int id);
    Task<IncomeCategoryResponse> GetIncomeCategoriesAsync(IncomeCategoryRequest request);
    Task<int> CreateIncomeCategoryAsync(AddEditIncomeCategoryModal request);
    Task<int> UpdateIncomeCategoryAsync(AddEditIncomeCategoryModal request);
    Task<bool> DeleteIncomeCategoryAsync(int id);
    Task<IncomeCategoryDropdownSelection[]> GetIncomeCategoryDropdownListAsync();
    Task<string[]> GetIncomeCategoryNames();
    Task<bool> CheckIfIncomeCategoryIsRefund(int categoryId);
    Task<IncomeCategoryEntity> GetIncomeCategoryByNameAsync(string name);
    Task<string[]> GetMatchingIncomeCategoryNamesAsync(string match);
}
public class IncomeCategoryService : IIncomeCategoryService
{
    private readonly IIncomeCategoryRepository _repo;
    private readonly IIncomeRepository _incomeRepo;

    public IncomeCategoryService(IIncomeCategoryRepository repo, IIncomeRepository incomeRepo) => (_repo, _incomeRepo) = (repo, incomeRepo);

    public async Task<int> CreateIncomeCategoryAsync(AddEditIncomeCategoryModal request)
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
        var categories = await _repo.FindWithPaginationIncludeIncome(x => true, request.PageNumber, request.PageSize);
        var income = categories.SelectMany(x => x.Income).ToArray();
        var categoryListItems = income.GroupBy(i => i.CategoryId).Select(g =>
        {
            var results = g.Aggregate(new IncomeCategoryListItemAggregator(g.Key.Value, categories), (acc, i) => acc.Accumulate(i), acc => acc.Compute());
            return new IncomeCategoryListItem()
            {
                Id = g.Key.Value,
                Name = results.Category.Name,
                Payments = results.Payments,
                Amount = results.Amount,
                LastPayment = results.LastPayment,
                InUse = results.Category.InUse
            };

        }).ToList();
        //adds in all the categories with no assigned income
        categoryListItems.AddRange(categories
            .Where(x => x.Income.Count == 0 && x.Name != "Uncategorized")
            .Select(x => new IncomeCategoryListItem()
            {
                Id = x.Id,
                Name = x.Name,
                Amount = 0,
                Payments = 0,
                LastPayment = DateTime.MinValue,
                InUse = x.InUse
            }));

        var count = await _repo.GetCount(x => true);

        return new IncomeCategoryResponse(request.PageNumber,
            request.PageSize,
            count,
            categoryListItems.OrderBy(x => x.Name).ToArray());
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

    public async Task<int> UpdateIncomeCategoryAsync(AddEditIncomeCategoryModal request)
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

    public async Task<string[]> GetMatchingIncomeCategoryNamesAsync(string match)
    {
        return (await _repo.Find(x => x.Name.StartsWith(match))).Select(x => x.Name).Take(10).ToArray();
    }

    public Task<IncomeCategoryDetail> GetCategoryDetailAsync(int id)
    {
        throw new NotImplementedException();
    }
}

