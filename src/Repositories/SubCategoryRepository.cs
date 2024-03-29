﻿using CashTrack.Data;
using CashTrack.Data.Entities;
using CashTrack.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CashTrack.Repositories.Common;

namespace CashTrack.Repositories.SubCategoriesRepository;

public interface ISubCategoryRepository : IRepository<SubCategoryEntity>
{
    Task<SubCategoryEntity[]> FindWithPaginationIncludeExpenses(Expression<Func<SubCategoryEntity, bool>> predicate, int pageNumber, int pageSize);
    Task<SubCategoryEntity[]> FindWithExpensesAndMerchants(Expression<Func<SubCategoryEntity, bool>> predicate);
}
public class SubCategoryRepository : ISubCategoryRepository
{
    private readonly AppDbContext _context;
    public SubCategoryRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<SubCategoryEntity[]> FindWithExpensesAndMerchants(Expression<Func<SubCategoryEntity, bool>> predicate)
    {
        try
        {
            return await _context.SubCategories.Where(predicate)
                .Include(x => x.MainCategory)
                .Include(x => x.Expenses)
                .ThenInclude(x => x.Merchant)
                .ToArrayAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<SubCategoryEntity[]> Find(Expression<Func<SubCategoryEntity, bool>> predicate)
    {
        try
        {
            return await _context.SubCategories.Where(predicate).Include(x => x.MainCategory).ToArrayAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<SubCategoryEntity> FindById(int id)
    {
        try
        {
            var category = await _context.SubCategories
                .Where(x => x.Id == id)
                .Include(x => x.MainCategory)
                .FirstOrDefaultAsync();
            if (category == null)
                throw new CategoryNotFoundException(id.ToString());

            return category;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<SubCategoryEntity[]> FindWithPagination(Expression<Func<SubCategoryEntity, bool>> predicate, int pageNumber, int pageSize)
    {
        try
        {
            var categories = await _context.SubCategories
                .Where(predicate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(x => x.Name)
                .Include(x => x.MainCategory)
                .ToArrayAsync();
            return categories;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<int> Create(SubCategoryEntity entity)
    {
        try
        {
            await _context.SubCategories.AddAsync(entity);
            var success = await _context.SaveChangesAsync();
            return success > 0 ? entity.Id : throw new Exception();
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<int> Update(SubCategoryEntity entity)
    {
        try
        {
            return await _context.SaveChangesAsync() > 0 ? entity.Id : throw new Exception("An error occured while trying to save the sub category.");
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<bool> Delete(SubCategoryEntity entity)
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

    public async Task<int> GetCount(Expression<Func<SubCategoryEntity, bool>> predicate)
    {
        try
        {
            var categories = await _context.SubCategories
                .CountAsync(predicate);
            return categories;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<SubCategoryEntity[]> FindWithPaginationIncludeExpenses(Expression<Func<SubCategoryEntity, bool>> predicate, int pageNumber, int pageSize)
    {
        try
        {
            var categories = await _context.SubCategories
                .Where(predicate)
                .OrderBy(x => x.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(x => x.MainCategory)
                .Include(x => x.Expenses)
                .ToArrayAsync();
            return categories;
        }
        catch (Exception)
        {
            throw;
        }
    }
}

