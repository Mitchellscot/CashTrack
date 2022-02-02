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
public interface IMerchantRepository : IRepository<Merchants>
{
}
public class MerchantRepository : IMerchantRepository
{
    private readonly AppDbContext _context;
    public MerchantRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<Merchants> FindById(int id)
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
    public async Task<Merchants[]> Find(Expression<Func<Merchants, bool>> predicate)
    {
        try
        {
            var merchant = await _context.Merchants.Where(predicate).OrderBy(x => x.name).ToArrayAsync();

            return merchant;
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<Merchants[]> FindWithPagination(Expression<Func<Merchants, bool>> predicate, int pageNumber, int pageSize)
    {
        try
        {
            var merchants = await _context.Merchants
                .Where(predicate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(x => x.name)
                .ToArrayAsync();
            return merchants;
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<bool> Create(Merchants entity)
    {
        try
        {
            await _context.Merchants.AddAsync(entity);
            return await (_context.SaveChangesAsync()) > 0;
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<bool> Update(Merchants entity)
    {
        try
        {
            _context.ChangeTracker.Clear();
            var contextAttachedEntity = _context.Merchants.Attach(entity);
            contextAttachedEntity.State = EntityState.Modified;
            return await (_context.SaveChangesAsync()) > 0;
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<bool> Delete(Merchants entity)
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

    public async Task<int> GetCount(Expression<Func<Merchants, bool>> predicate)
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
