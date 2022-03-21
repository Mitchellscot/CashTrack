using CashTrack.Data;
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
}
public class SubCategoryRepository : ISubCategoryRepository
{
    private readonly AppDbContext _context;
    public SubCategoryRepository(AppDbContext context)
    {
        _context = context;
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

    public async Task<bool> Create(SubCategoryEntity entity)
    {
        try
        {
            await _context.SubCategories.AddAsync(entity);
            return await (_context.SaveChangesAsync()) > 0;
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<bool> Update(SubCategoryEntity entity)
    {
        try
        {
            _context.ChangeTracker.Clear();
            var contextAttachedEntity = _context.SubCategories.Attach(entity);
            contextAttachedEntity.State = EntityState.Modified;
            return await (_context.SaveChangesAsync()) > 0;
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
}

