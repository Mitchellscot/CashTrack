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

namespace CashTrack.Repositories.ExpenseReviewRepository;

public interface IExpenseReviewRepository : IRepository<ExpenseReviewEntity>
{
    Task<int> UpdateMany(IEnumerable<ExpenseReviewEntity> entities);
    Task<int> AddMany(IEnumerable<ExpenseReviewEntity> entities);
}
public class ExpenseReviewRepository : IExpenseReviewRepository
{
    private readonly AppDbContext _ctx;
    public ExpenseReviewRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task<ExpenseReviewEntity[]> FindWithPagination(Expression<Func<ExpenseReviewEntity, bool>> predicate, int pageNumber, int pageSize)
    {
        try
        {
            return await _ctx.ExpensesToReview
            .Where(x => x.IsReviewed == false)
            .Where(predicate)
            .Include(x => x.SuggestedCategory)
            .Include(x => x.SuggestedMerchant)
            .OrderByDescending(x => x.Date)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync();
        }
        catch (Exception)
        {
            throw;
        }

    }
    public async Task<ExpenseReviewEntity> FindById(int id)
    {
        try
        {
            var expense = await _ctx.ExpensesToReview
            .Include(x => x.SuggestedCategory)
            .Include(x => x.SuggestedMerchant)
            .Where(x => x.IsReviewed == false)
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

    public async Task<ExpenseReviewEntity[]> Find(Expression<Func<ExpenseReviewEntity, bool>> predicate)
    {
        try
        {
            return await _ctx.ExpensesToReview
            .Include(x => x.SuggestedCategory)
            .Include(x => x.SuggestedMerchant)
            .Where(x => x.IsReviewed == false)
            .Where(predicate)
            .OrderByDescending(x => x.Date)
            .ToArrayAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<int> GetCount(Expression<Func<ExpenseReviewEntity, bool>> predicate)
    {
        try
        {
            return await _ctx.ExpensesToReview
            .Where(predicate)
            .CountAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<int> Create(ExpenseReviewEntity entity)
    {
        try
        {
            await _ctx.ExpensesToReview.AddAsync(entity);
            var success = await _ctx.SaveChangesAsync();
            return success > 0 ? entity.Id : throw new Exception();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<int> Update(ExpenseReviewEntity entity)
    {
        try
        {
            return await _ctx.SaveChangesAsync() > 0 ? entity.Id : throw new Exception("An error occured while trying to update the expense review.");
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> Delete(ExpenseReviewEntity entity)
    {
        try
        {
            _ctx.ExpensesToReview.Remove(entity);
            return await (_ctx.SaveChangesAsync()) > 0;
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<int> UpdateMany(IEnumerable<ExpenseReviewEntity> entities)

    {
        try
        {
            _ctx.ExpensesToReview.UpdateRange(entities);
            return await _ctx.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<int> AddMany(IEnumerable<ExpenseReviewEntity> entities)
    {
        try
        {
            _ctx.ExpensesToReview.AddRange(entities);
            return await _ctx.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }
}
