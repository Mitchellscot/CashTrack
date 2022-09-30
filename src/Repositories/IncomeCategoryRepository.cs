using CashTrack.Data;
using CashTrack.Data.Entities;
using CashTrack.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CashTrack.Repositories.Common;

namespace CashTrack.Repositories.IncomeCategoryRepository;

public interface IIncomeCategoryRepository : IRepository<IncomeCategoryEntity>
{
    Task<IncomeCategoryEntity[]> FindWithPaginationIncludeIncome(Expression<Func<IncomeCategoryEntity, bool>> predicate, int pageNumber, int pageSize);
    Task<IncomeCategoryEntity[]> FindWithIncomeAndSources(Expression<Func<IncomeCategoryEntity, bool>> predicate);

}

public class IncomeCategoryRepository : IIncomeCategoryRepository
{
    private readonly AppDbContext _ctx;

    public IncomeCategoryRepository(AppDbContext context) => _ctx = context;

    public async Task<IncomeCategoryEntity[]> FindWithIncomeAndSources(Expression<Func<IncomeCategoryEntity, bool>> predicate)
    {
        try
        {
            return await _ctx.IncomeCategories.Where(predicate)
                .Include(x => x.Income)
                .ThenInclude(x => x.Source)
                .ToArrayAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<int> Create(IncomeCategoryEntity entity)
    {
        try
        {
            await _ctx.IncomeCategories.AddAsync(entity);
            var success = await _ctx.SaveChangesAsync();
            return success > 0 ? entity.Id : throw new Exception();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> Delete(IncomeCategoryEntity entity)
    {
        try
        {
            _ctx.Remove(entity);
            return await (_ctx.SaveChangesAsync()) > 0;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IncomeCategoryEntity[]> Find(Expression<Func<IncomeCategoryEntity, bool>> predicate)
    {
        try
        {
            return await _ctx.IncomeCategories.Where(predicate).ToArrayAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IncomeCategoryEntity> FindById(int id)
    {
        try
        {
            var category = await _ctx.IncomeCategories
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();
            if (category == null)
                throw new CategoryNotFoundException(id.ToString());
            return category;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IncomeCategoryEntity[]> FindWithPagination(Expression<Func<IncomeCategoryEntity, bool>> predicate, int pageNumber, int pageSize)
    {
        try
        {
            var categories = await _ctx.IncomeCategories
                .Where(predicate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(x => x.Name)
                .ToArrayAsync();
            return categories;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IncomeCategoryEntity[]> FindWithPaginationIncludeIncome(Expression<Func<IncomeCategoryEntity, bool>> predicate, int pageNumber, int pageSize)
    {
        try
        {
            var categories = await _ctx.IncomeCategories
                .Where(predicate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(x => x.Name)
                .Include(x => x.Income)
                .ToArrayAsync();
            return categories;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<int> GetCount(Expression<Func<IncomeCategoryEntity, bool>> predicate)
    {
        try
        {
            return await _ctx.IncomeCategories.CountAsync(predicate);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<int> Update(IncomeCategoryEntity entity)
    {
        try
        {
            return await _ctx.SaveChangesAsync() > 0 ? entity.Id : throw new Exception("An error occured while trying to save the income category.");
        }
        catch (Exception)
        {
            throw;
        }
    }
}

