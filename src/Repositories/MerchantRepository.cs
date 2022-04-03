using CashTrack.Data;
using CashTrack.Data.Entities;
using CashTrack.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CashTrack.Repositories.Common;

namespace CashTrack.Repositories.MerchantRepository;
public interface IMerchantRepository : IRepository<MerchantEntity>
{
    Task<MerchantEntity[]> FindWithPaginationReversable(Expression<Func<MerchantEntity, bool>> predicate, int pageNumber, int pageSize, bool reversed = false);
    Task<MerchantEntity[]> FindWithPaginationOrderedByLocation(Expression<Func<MerchantEntity, bool>> predicate, int pageNumber, int pageSize, bool reversed = false);
}
public class MerchantRepository : IMerchantRepository
{
    private readonly AppDbContext _context;
    public MerchantRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<MerchantEntity> FindById(int id)
    {
        try
        {
            var merchant = await _context.Merchants.FindAsync(id);
            if (merchant == null)
                throw new MerchantNotFoundException(id.ToString());

            return merchant;
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<MerchantEntity[]> Find(Expression<Func<MerchantEntity, bool>> predicate)
    {
        try
        {
            var merchant = await _context.Merchants.Where(predicate).OrderBy(x => x.Name).ToArrayAsync();

            return merchant;
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<MerchantEntity[]> FindWithPagination(Expression<Func<MerchantEntity, bool>> predicate, int pageNumber, int pageSize)
    {
        try
        {
            var merchants = await _context.Merchants
                .Where(predicate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(x => x.Name)
                .ToArrayAsync();
            return merchants;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<MerchantEntity[]> FindWithPaginationOrderedByLocation(Expression<Func<MerchantEntity, bool>> predicate, int pageNumber, int pageSize, bool reversed)
    {
        try
        {
            if (reversed)
            {
                return await _context.Merchants
                    .OrderByDescending(x => x.IsOnline)
                    .ThenByDescending(x => x.City)
                    .ThenBy(x => x.Name)
                    .Where(predicate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToArrayAsync();
            }
            else
            {
                return await _context.Merchants
                    .OrderBy(x => x.IsOnline)
                    .ThenBy(x => x.City)
                    .ThenBy(x => x.Name)
                    .Where(predicate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToArrayAsync();
            }
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<MerchantEntity[]> FindWithPaginationReversable(Expression<Func<MerchantEntity, bool>> predicate, int pageNumber, int pageSize, bool reversed = false)
    {
        try
        {
            if (reversed)
            {
                return await _context.Merchants
                    .Where(predicate)
                    .OrderByDescending(x => x.Name)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToArrayAsync();
            }
            else
            {
                return await _context.Merchants
                    .Where(predicate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .OrderBy(x => x.Name)
                    .ToArrayAsync();
            }
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<int> Create(MerchantEntity entity)
    {
        try
        {
            await _context.Merchants.AddAsync(entity);
            var success = await _context.SaveChangesAsync();
            return success > 0 ? entity.Id : throw new Exception();
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<int> Update(MerchantEntity entity)
    {
        try
        {
            _context.ChangeTracker.Clear();
            var contextAttachedEntity = _context.Merchants.Attach(entity);
            contextAttachedEntity.State = EntityState.Modified;
            return await _context.SaveChangesAsync() > 0 ? entity.Id : throw new Exception();
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<bool> Delete(MerchantEntity entity)
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

    public async Task<int> GetCount(Expression<Func<MerchantEntity, bool>> predicate)
    {
        try
        {
            return await _context.Merchants.CountAsync(predicate);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
