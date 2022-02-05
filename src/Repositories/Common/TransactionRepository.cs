using CashTrack.Data;
using CashTrack.Data.Entities;
using CashTrack.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CashTrack.Repositories
{
    public abstract class TransactionRepository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly AppDbContext _ctx;
        public TransactionRepository(AppDbContext ctx) => _ctx = ctx;

        public async Task<bool> Create(T entity)
        {
            await _ctx.Set<T>().AddAsync(entity);
            return await (_ctx.SaveChangesAsync()) > 0;
        }

        public async Task<bool> Delete(T entity)
        {
            _ctx.Set<T>().Remove(entity);
            return await (_ctx.SaveChangesAsync()) > 0;
        }

        public async virtual Task<T[]> Find(Expression<Func<T, bool>> predicate)
        {
            return await _ctx.Set<T>().AsQueryable().Where(predicate).ToArrayAsync();
        }

        public async virtual Task<T> FindById(int id)
        {
            return await _ctx.Set<T>()
                        .AsQueryable()
                        .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async virtual Task<T[]> FindWithPagination(Expression<System.Func<T, bool>> predicate, int pageNumber, int pageSize)
        {
            return await _ctx
                        .Set<T>()
                        .AsQueryable()
                        .Where(predicate)
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToArrayAsync();
        }

        public async virtual Task<int> GetCount(Expression<System.Func<T, bool>> predicate)
        {
            return await _ctx.Set<T>()
                        .Where(predicate)
                        .CountAsync();
        }

        public async virtual Task<bool> Update(T entity)
        {
            _ctx.ChangeTracker.Clear();
            var Entity = _ctx.Set<T>().Attach(entity);
            Entity.State = EntityState.Modified;
            return await (_ctx.SaveChangesAsync()) > 0;
        }
    }
}
