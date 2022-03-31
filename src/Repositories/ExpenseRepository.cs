using CashTrack.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using CashTrack.Common.Exceptions;
using CashTrack.Data.Entities;
using System.Linq.Expressions;
using CashTrack.Repositories.Common;
using System.Collections.Generic;

namespace CashTrack.Repositories.ExpenseRepository;

public interface IExpenseRepository : IRepository<ExpenseEntity>
{
    Task<decimal> GetAmountOfExpenses(Expression<Func<ExpenseEntity, bool>> predicate);
    Task<ExpenseEntity[]> GetExpensesAndCategories(Expression<Func<ExpenseEntity, bool>> predicate);
    Task<bool> UpdateMany(List<ExpenseEntity> entities);
}
public class ExpenseRepository : IExpenseRepository
{
    private readonly AppDbContext _ctx;
    public ExpenseRepository(AppDbContext context)
    {
        _ctx = context;
    }
    public async Task<ExpenseEntity[]> Find(Expression<Func<ExpenseEntity, bool>> predicate)
    {
        try
        {
            return await _ctx.Expenses
                .Include(x => x.ExpenseTags)
                .ThenInclude(x => x.Tag)
                .Include(x => x.Merchant)
                .Include(x => x.Category)
                .ThenInclude(x => x.MainCategory)
                .Where(predicate).ToArrayAsync();
        }
        catch (Exception)
        {

            throw;
        }
    }
    public async Task<ExpenseEntity> FindById(int id)
    {
        try
        {
            var expense = await _ctx.Expenses
                .Include(x => x.ExpenseTags)
                .ThenInclude(x => x.Tag)
                .Include(x => x.Merchant)
                .Include(x => x.Category)
                .ThenInclude(x => x.MainCategory)
                .SingleOrDefaultAsync(x => x.Id == id);
            if (expense == null)
                throw new ExpenseNotFoundException(id.ToString());

            return expense;
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<ExpenseEntity[]> FindWithPagination(Expression<Func<ExpenseEntity, bool>> predicate, int pageNumber, int pageSize)
    {
        try
        {
            var expenses = await _ctx.Expenses
                    .Where(predicate)
                    .Include(x => x.ExpenseTags)
                    .ThenInclude(x => x.Tag)
                    .Include(x => x.Merchant)
                    .Include(x => x.Category)
                    .ThenInclude(x => x.MainCategory)
                    .OrderByDescending(x => x.Date)
                    .ThenByDescending(x => x.Id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToArrayAsync();
            return expenses;
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<int> Create(ExpenseEntity entity)
    {
        try
        {
            await _ctx.Expenses.AddAsync(entity);
            var success = await _ctx.SaveChangesAsync();
            return success > 0 ? entity.Id : throw new Exception();
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<int> Update(ExpenseEntity entity)
    {
        try
        {
            _ctx.ChangeTracker.Clear();
            var Entity = _ctx.Expenses.Attach(entity);
            Entity.State = EntityState.Modified;
            return await _ctx.SaveChangesAsync() > 0 ? entity.Id : throw new Exception();
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<bool> UpdateMany(List<ExpenseEntity> entities)
    {
        try
        {
            //_ctx.ChangeTracker.Clear();
            _ctx.Expenses.UpdateRange(entities);
            return await (_ctx.SaveChangesAsync()) > 0;
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<bool> Delete(ExpenseEntity entity)
    {
        try
        {
            _ctx.Expenses.Remove(entity);
            return await (_ctx.SaveChangesAsync()) > 0;
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<decimal> GetAmountOfExpenses(Expression<Func<ExpenseEntity, bool>> predicate)
    {
        try
        {
            return (decimal)await _ctx.Expenses
            .Where(predicate)
            .SumAsync(x => x.Amount);
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<ExpenseEntity[]> GetExpensesAndCategories(Expression<Func<ExpenseEntity, bool>> predicate)
    {
        try
        {
            var expenses = await _ctx.Expenses
                .Where(predicate)
                .Include(x => x.Category)
                .ToArrayAsync();
            return expenses;
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<int> GetCount(Expression<Func<ExpenseEntity, bool>> predicate)
    {
        try
        {
            return await _ctx.Expenses
            .Where(predicate)
            .CountAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }
}
