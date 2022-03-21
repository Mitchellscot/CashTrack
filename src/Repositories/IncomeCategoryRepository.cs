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
}

public class IncomeCategoryRepository : IIncomeCategoryRepository
{
    private readonly AppDbContext _ctx;

    public IncomeCategoryRepository(AppDbContext context) => _ctx = context;

    public async Task<bool> Create(IncomeCategoryEntity entity)
    {
        try
        {
            await _ctx.IncomeCategories.AddAsync(entity);
            return await (_ctx.SaveChangesAsync()) > 0;
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

    public async Task<bool> Update(IncomeCategoryEntity entity)
    {
        try
        {
            _ctx.ChangeTracker.Clear();
            var contextAttachedEntity = _ctx.IncomeCategories.Attach(entity);
            contextAttachedEntity.State = EntityState.Modified;
            return await (_ctx.SaveChangesAsync()) > 0;
        }
        catch (Exception)
        {
            throw;
        }
    }
}

