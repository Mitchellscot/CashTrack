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

public interface ISubCategoryRepository : IRepository<SubCategories>
{
}
public class SubCategoryRepository : ISubCategoryRepository
{
    private readonly AppDbContext _context;
    public SubCategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SubCategories[]> Find(Expression<Func<SubCategories, bool>> predicate)
    {
        try
        {
            return await _context.SubCategories.Where(predicate).Include(x => x.main_category).ToArrayAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<SubCategories> FindById(int id)
    {
        try
        {
            var category = await _context.SubCategories
                .Where(x => x.Id == id)
                .Include(x => x.main_category)
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

    public async Task<SubCategories[]> FindWithPagination(Expression<Func<SubCategories, bool>> predicate, int pageNumber, int pageSize)
    {
        try
        {
            var categories = await _context.SubCategories
                .Where(predicate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(x => x.sub_category_name)
                .Include(x => x.main_category)
                .ToArrayAsync();
            return categories;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> Create(SubCategories entity)
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
    public async Task<bool> Update(SubCategories entity)
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
    public async Task<bool> Delete(SubCategories entity)
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

    public async Task<int> GetCount(Expression<Func<SubCategories, bool>> predicate)
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

