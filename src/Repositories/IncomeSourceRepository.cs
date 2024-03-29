﻿using CashTrack.Data;
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

    public async Task<int> Create(IncomeSourceEntity entity)
    {
        try
        {
            await _ctx.IncomeSources.AddAsync(entity);
            var success = await _ctx.SaveChangesAsync();
            return success > 0 ? entity.Id : throw new Exception();
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
                .FirstOrDefaultAsync();
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

    public async Task<int> Update(IncomeSourceEntity entity)
    {
        try
        {
            return await _ctx.SaveChangesAsync() > 0 ? entity.Id : throw new Exception("An error occured while trying to save the income source.");
        }
        catch (Exception)
        {
            throw;
        }
    }
}

