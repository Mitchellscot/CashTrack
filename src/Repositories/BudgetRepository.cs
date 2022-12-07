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

namespace CashTrack.Repositories.BudgetRepository
{
    public interface IBudgetRepository : IRepository<BudgetEntity>
    {
        Task<int> CreateMany(IEnumerable<BudgetEntity> entities);
        Task<bool> DeleteMany(List<BudgetEntity> entity);
        Task<BudgetEntity[]> FindWithMainCategories(Expression<Func<BudgetEntity, bool>> predicate);
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

        public async Task<bool> Delete(BudgetEntity entity)
        {
            try
            {
                _ctx.Budgets.Remove(entity);
                return await (_ctx.SaveChangesAsync()) > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> DeleteMany(List<BudgetEntity> entity)
        {
            try
            {
                _ctx.Budgets.RemoveRange(entity);
                return await (_ctx.SaveChangesAsync()) > 0;
            }
            catch (Exception)
            {
                throw;
            }
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
        public async Task<BudgetEntity[]> FindWithMainCategories(Expression<Func<BudgetEntity, bool>> predicate)
        {
            try
            {
                return await _ctx.Budgets
                .Include(x => x.SubCategory)
                .ThenInclude(x => x.MainCategory)
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

        public async Task<BudgetEntity> FindById(int id)
        {
            try
            {
                var budget = await _ctx.Budgets
                    .Include(x => x.SubCategory)
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (budget == null)
                    throw new BudgetNotFoundException(id.ToString());

                return budget;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<BudgetEntity[]> FindWithPagination(Expression<Func<BudgetEntity, bool>> predicate, int pageNumber, int pageSize)
        {
            //not using pagination on this page
            throw new NotImplementedException();
        }

        public async Task<int> GetCount(Expression<Func<BudgetEntity, bool>> predicate)
        {
            try
            {
                return await _ctx.Budgets
                .Where(predicate)
                .CountAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> Update(BudgetEntity entity)
        {
            try
            {
                //in case a budget gets saved that has no changes associated with it, doesn't throw error
                _ctx.Entry(entity).State = EntityState.Modified;
                return await _ctx.SaveChangesAsync() > 0 ? entity.Id : throw new Exception("An error occured while trying to save the budget.");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
