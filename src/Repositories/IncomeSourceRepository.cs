using CashTrack.Data;
using CashTrack.Data.Entities;
using CashTrack.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CashTrack.Repositories.Common;

namespace CashTrack.Repositories.IncomeSourceRepository;

public interface IIncomeSourceRepository : IRepository<IncomeSourceEntity>
{
}
public class IncomeSourceRepository : IIncomeSourceRepository
{
    private readonly AppDbContext _ctx;

    public IncomeSourceRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task<bool> Create(IncomeSourceEntity entity)
    {
        try
        {
            await _ctx.IncomeSources.AddAsync(entity);
            return await (_ctx.SaveChangesAsync()) > 0;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> Delete(IncomeSourceEntity entity)
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

    public async Task<IncomeSourceEntity[]> Find(Expression<Func<IncomeSourceEntity, bool>> predicate)
    {
        try
        {
            return await _ctx.IncomeSources.Where(predicate).ToArrayAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IncomeSourceEntity> FindById(int id)
    {
        try
        {
            var category = await _ctx.IncomeSources
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();
            if (category == null)
                throw new IncomeSourceNotFoundException(id.ToString());
            return category;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IncomeSourceEntity[]> FindWithPagination(Expression<Func<IncomeSourceEntity, bool>> predicate, int pageNumber, int pageSize)
    {
        try
        {
            var sources = await _ctx.IncomeSources
                .Where(predicate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(x => x.Name)
                .ToArrayAsync();
            return sources;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<int> GetCount(Expression<Func<IncomeSourceEntity, bool>> predicate)
    {
        try
        {
            return await _ctx.IncomeSources.CountAsync(predicate);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> Update(IncomeSourceEntity entity)
    {
        try
        {
            _ctx.ChangeTracker.Clear();
            var contextAttachedEntity = _ctx.IncomeSources.Attach(entity);
            contextAttachedEntity.State = EntityState.Modified;
            return await (_ctx.SaveChangesAsync()) > 0;
        }
        catch (Exception)
        {
            throw;
        }
    }
}

