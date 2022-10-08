using CashTrack.Data;
using CashTrack.Data.Entities;
using CashTrack.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CashTrack.Repositories.Common;
using CashTrack.Models.MainCategoryModels;
using System.Collections.Generic;

namespace CashTrack.Repositories.MainCategoriesRepository
{
    public interface IMainCategoriesRepository : IRepository<MainCategoryEntity>
    {
        Task<MainCategoryListItem[]> GetMainCategoryListItems(Expression<Func<ExpenseEntity, bool>> predicate);
        Task<Dictionary<string, int>> GetSubCategoryCounts(Expression<Func<ExpenseEntity, bool>> predicate);
        Task<Dictionary<string, decimal>> GetSubCategoryAmounts(Expression<Func<ExpenseEntity, bool>> predicate);
        Task<Dictionary<string, int>> GetMainCategoryPercentages(Expression<Func<ExpenseEntity, bool>> predicate);
    }
    public class MainCategoriesRepository : IMainCategoriesRepository
    {
        private readonly AppDbContext _context;
        public MainCategoriesRepository(AppDbContext dbContext) => (_context) = (dbContext);

        //my attempt to produce a fast query with an aggregate on a navigation property
        public async Task<MainCategoryListItem[]> GetMainCategoryListItems(Expression<Func<ExpenseEntity, bool>> predicate)
        {
            var categories = await _context.SubCategories
                .Where(x => x.Name != "Uncategorized")
                .ToListAsync();

            return categories.GroupBy(c => c.MainCategoryId).Select(g =>
            {
                var subCategoryEntities = categories.Where(x => x.MainCategoryId == g.Key).ToList();

                var subCategoryAmounts = subCategoryEntities.Select(subCategory =>
                {
                    var totalAmountofSubCategory = _context.Entry(subCategory)
                    .Collection(x => x.Expenses)
                    .Query()
                    .Where(sc => sc.Category.Name != "Uncategorized")
                    .Where(predicate)
                    .Sum(x => x.Amount);

                    return (Name: subCategory.Name, Amount: totalAmountofSubCategory);
                }).ToDictionary(k => k.Name, v => v.Amount);

                var mainCategoryTotal = (int)decimal.Round(subCategoryAmounts.Values.Sum(), 0);

                var subCategoryPercentages = subCategoryAmounts.Where(sc => sc.Value > 0).Select(x =>
                (Name: x.Key, Percentage: (int)decimal.Round((x.Value / mainCategoryTotal) * 100)))
                .ToDictionary(k => k.Name, v => v.Percentage);

                return new MainCategoryListItem()
                {
                    Id = g.Key,
                    Name = subCategoryEntities.First().MainCategory.Name,
                    NumberOfSubCategories = subCategoryPercentages.Keys.Count(),
                    SubCategoryExpenses = subCategoryPercentages
                };

            }).Where(x => x.NumberOfSubCategories > 0).OrderBy(x => x.Name).ToArray();
        }

        public async Task<Dictionary<string, int>> GetMainCategoryPercentages(Expression<Func<ExpenseEntity, bool>> predicate)
        {
            var categories = await _context.SubCategories
                .Where(x => x.Name != "Uncategorized")
                .Include(x => x.Expenses)
                .ToListAsync();

            var amounts =  categories.GroupBy(x => x.MainCategory.Name).Select(x =>
            {
                return(Name: x.Key, Amount: x.SelectMany(x => x.Expenses)
                .AsQueryable()
                .Where(predicate)
                .Sum(x => x.Amount));
            }).ToDictionary(k => k.Name, v => v.Amount);
            var total = amounts.Values.Sum();
            return amounts.Where(x => x.Value > 0).Select(x =>
            {
                return (Name: x.Key, Percentage: (int)decimal.Round((x.Value / total) * 100));
            }).Where(x => x.Percentage > 0).ToDictionary(k => k.Name, v => v.Percentage);
        }

        public async Task<int> Create(MainCategoryEntity entity)
        {
            var count = await GetCount(x => true);
            if (count >= 25)
                throw new MainCategoryLimitException(count);

            try
            {
                await _context.MainCategories.AddAsync(entity);
                var success = await _context.SaveChangesAsync();
                return success > 0 ? entity.Id : throw new Exception();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Delete(MainCategoryEntity entity)
        {
            try
            {
                _context.Remove(entity);
                return await (_context.SaveChangesAsync()) > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<MainCategoryEntity[]> Find(Expression<Func<MainCategoryEntity, bool>> predicate)
        {
            try
            {
                return await _context.MainCategories.Where(predicate).ToArrayAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<MainCategoryEntity> FindById(int id)
        {
            try
            {
                var category = await _context.MainCategories.Where(x => x.Id == id).SingleOrDefaultAsync();
                if (category == null)
                    throw new CategoryNotFoundException(id.ToString());

                return category;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<MainCategoryEntity[]> FindWithPagination(Expression<Func<MainCategoryEntity, bool>> predicate, int pageNumber, int pageSize)
        {
            //25 categories max, so not implementing
            throw new NotImplementedException();
        }

        public async Task<int> GetCount(Expression<Func<MainCategoryEntity, bool>> predicate)
        {
            try
            {
                return await _context.MainCategories.CountAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> Update(MainCategoryEntity entity)
        {
            try
            {
                return await _context.SaveChangesAsync() > 0 ? entity.Id : throw new Exception("An error occured while trying to save the main category.");
            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task<Dictionary<string, decimal>> GetSubCategoryAmounts(Expression<Func<ExpenseEntity, bool>> predicate)
        {
            var subCategories = await _context.SubCategories.Where(x => x.Name != "Uncategorized").ToListAsync();
            return subCategories.Select(subCategory =>
            {
                var totalAmountofSubCategory = _context.Entry(subCategory)
                .Collection(x => x.Expenses)
                .Query()
                .Where(sc => sc.Category.Name != "Uncategorized")
                .Where(predicate)
                .Sum(x => x.Amount);

                return (Name: subCategory.Name, Amount: totalAmountofSubCategory);
            }).ToDictionary(k => k.Name, v => v.Amount);
        }
        public async Task<Dictionary<string, int>> GetSubCategoryCounts(Expression<Func<ExpenseEntity, bool>> predicate)
        {
            var subCategories = await _context.SubCategories.Where(x => x.Name != "Uncategorized").ToListAsync();
            return subCategories.Select(subCategory =>
            {
                var totalOfSubCategory = _context.Entry(subCategory)
                .Collection(x => x.Expenses)
                .Query()
                .Where(x => x.Category.Name != "Uncategorized")
                .Where(predicate)
                .Count();

                return (Name: subCategory.Name, Count: totalOfSubCategory);
            }).Where(x => x.Count > 0).ToDictionary(k => k.Name, v => v.Count);
        }
    }
}
