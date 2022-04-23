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

namespace CashTrack.Repositories.IncomeReviewRepository;

public interface IIncomeReviewRepository : IRepository<IncomeReviewEntity>
{
    Task<bool> UpdateMany(List<IncomeReviewEntity> entities);
}

public class IncomeReviewRepository : IIncomeReviewRepository
{
    private readonly AppDbContext _ctx;
    public IncomeReviewRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task<IncomeReviewEntity[]> FindWithPagination(Expression<Func<IncomeReviewEntity, bool>> predicate, int pageNumber, int pageSize)
    {
        return await _ctx.IncomeToReview
                    .Where(predicate)
                    .Include(x => x.SuggestedCategory)
                    .Include(x => x.SuggestedSource)
                    .OrderByDescending(x => x.Date)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToArrayAsync();
    }
    public async Task<IncomeReviewEntity> FindById(int id)
    {
        try
        {
            var income = await _ctx.IncomeToReview
                        .Include(x => x.SuggestedCategory)
                        .Include(x => x.SuggestedSource)
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

    public async Task<IncomeReviewEntity[]> Find(Expression<Func<IncomeReviewEntity, bool>> predicate)
    {
        try
        {
            return await _ctx.IncomeToReview
            .Include(x => x.SuggestedCategory)
            .Include(x => x.SuggestedSource)
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

    public async Task<int> GetCount(Expression<Func<IncomeReviewEntity, bool>> predicate)
    {
        try
        {
            return await _ctx.IncomeToReview
            .Where(predicate)
            .CountAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<int> Create(IncomeReviewEntity entity)
    {
        try
        {
            await _ctx.IncomeToReview.AddAsync(entity);
            var success = await _ctx.SaveChangesAsync();
            return success > 0 ? entity.Id : throw new Exception();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<int> Update(IncomeReviewEntity entity)
    {
        try
        {
            return await _ctx.SaveChangesAsync() > 0 ? entity.Id : throw new Exception("An error occured while trying to update the income review.");
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> Delete(IncomeReviewEntity entity)
    {
        try
        {
            _ctx.IncomeToReview.Remove(entity);
            return await (_ctx.SaveChangesAsync()) > 0;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> UpdateMany(List<IncomeReviewEntity> entities)
    {
        try
        {
            _ctx.IncomeToReview.UpdateRange(entities);
            return await (_ctx.SaveChangesAsync()) > 0;
        }
        catch (Exception)
        {
            throw;
        }
    }
}

