using CashTrack.Common.Exceptions;
using CashTrack.Data;
using CashTrack.Data.Entities;
using CashTrack.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CashTrack.Repositories.IncomeRepository;
public interface IIncomeRepository : IRepository<IncomeEntity>
{
    Task<decimal> GetAmountOfIncome(Expression<Func<IncomeEntity, bool>> predicate);
    Task<decimal> GetAmountOfIncomeNoRefunds(Expression<Func<IncomeEntity, bool>> predicate);
    Task<IncomeEntity[]> GetIncomeAndCategoriesBySourceId(int id);
    Task<bool> UpdateMany(List<IncomeEntity> entities);
}
public class IncomeRepository : IIncomeRepository
{
    private readonly AppDbContext _ctx;
    public IncomeRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task<int> Create(IncomeEntity entity)
    {
        try
        {
            await _ctx.Incomes.AddAsync(entity);
            var success = await _ctx.SaveChangesAsync();
            return success > 0 ? entity.Id : throw new Exception();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> Delete(IncomeEntity entity)
    {
        try
        {
            _ctx.Incomes.Remove(entity);
            return await (_ctx.SaveChangesAsync()) > 0;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IncomeEntity[]> Find(Expression<Func<IncomeEntity, bool>> predicate)
    {
        try
        {
            return await _ctx.Incomes
                .Where(predicate)
                .Include(x => x.Source)
                .Include(x => x.Category)
                .ToArrayAsync();
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<IncomeEntity> FindById(int id)
    {
        try
        {
            var income = await _ctx.Incomes
                .Include(x => x.Source)
                .Include(x => x.Category)
                .SingleOrDefaultAsync(x => x.Id == id);
            if (income == null)
                throw new IncomeNotFoundException(id.ToString());
            return income;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IncomeEntity[]> FindWithPagination(Expression<Func<IncomeEntity, bool>> predicate, int pageNumber, int pageSize)
    {
        try
        {
            var income = await _ctx.Incomes
                    .Where(predicate)
                    .Include(x => x.Source)
                    .Include(x => x.Category)
                    .OrderByDescending(x => x.Date)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToArrayAsync();
            return income;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<decimal> GetAmountOfIncome(Expression<Func<IncomeEntity, bool>> predicate)
    {
        try
        {
            return (decimal)await _ctx.Incomes
            .Where(predicate)
            .SumAsync(x => x.Amount);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<decimal> GetAmountOfIncomeNoRefunds(Expression<Func<IncomeEntity, bool>> predicate)
    {
        try
        {
            return (decimal)await _ctx.Incomes
            .Where(x => !x.IsRefund)
            .Where(predicate)
            .SumAsync(x => x.Amount);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<int> GetCount(Expression<Func<IncomeEntity, bool>> predicate)
    {
        try
        {
            return await _ctx.Incomes
            .Where(predicate)
            .CountAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IncomeEntity[]> GetIncomeAndCategoriesBySourceId(int id)
    {
        try
        {
            var expenses = await _ctx.Incomes
                .Where(x => x.SourceId == id)
                .Include(x => x.Category)
                .ToArrayAsync();
            return expenses;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<int> Update(IncomeEntity entity)
    {
        try
        {
            return await _ctx.SaveChangesAsync() > 0 ? entity.Id : throw new Exception("An error occured while trying to save the income.");
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<bool> UpdateMany(List<IncomeEntity> entities)
    {
        try
        {
            _ctx.Incomes.UpdateRange(entities);
            return await (_ctx.SaveChangesAsync()) > 0;
        }
        catch (Exception)
        {
            throw;
        }
    }
}

