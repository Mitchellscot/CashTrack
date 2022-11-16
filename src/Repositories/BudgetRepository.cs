using CashTrack.Data;
using CashTrack.Data.Entities;
using CashTrack.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CashTrack.Repositories.BudgetRepository
{
    public interface IBudgetRepository : IRepository<BudgetEntity>
    {
        Task<int> CreateMany(IEnumerable<BudgetEntity> entities);
    }
    public class BudgetRepository : IBudgetRepository
    {
        private readonly AppDbContext _ctx;
        public BudgetRepository(AppDbContext ctx) => _ctx = ctx;

        public async Task<int> Create(BudgetEntity entity)
        {
            try
            {
                await _ctx.Budgets.AddAsync(entity);
                var success = await _ctx.SaveChangesAsync();
                return success > 0 ? entity.Id : throw new Exception();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> CreateMany(IEnumerable<BudgetEntity> entities)
        {
            try
            {
                _ctx.Budgets.AddRange(entities);
                return await _ctx.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<bool> Delete(BudgetEntity entity)
        {
            throw new NotImplementedException();
        }

        public async Task<BudgetEntity[]> Find(Expression<Func<BudgetEntity, bool>> predicate)
        {
            try
            {
                return await _ctx.Budgets
                .Include(x => x.SubCategory)
                .Where(predicate)
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToArrayAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<BudgetEntity> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<BudgetEntity[]> FindWithPagination(Expression<Func<BudgetEntity, bool>> predicate, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetCount(Expression<Func<BudgetEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<int> Update(BudgetEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
