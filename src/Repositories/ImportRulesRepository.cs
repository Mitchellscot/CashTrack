using CashTrack.Data;
using CashTrack.Data.Entities;
using CashTrack.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CashTrack.Repositories.ImportRuleRepository
{
    public interface IImportRulesRepository : IRepository<ImportRuleEntity>
    {
    }
    public class ImportRulesRepository : IImportRulesRepository
    {
        private readonly AppDbContext _ctx;
        public ImportRulesRepository(AppDbContext ctx) => _ctx = ctx;
        public async Task<int> Create(ImportRuleEntity entity)
        {
            try
            {
                await _ctx.ImportRules.AddAsync(entity);
                var success = await _ctx.SaveChangesAsync();
                return success;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Delete(ImportRuleEntity entity)
        {
            try
            {
                _ctx.ImportRules.Remove(entity);
                return await (_ctx.SaveChangesAsync()) > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ImportRuleEntity[]> Find(Expression<Func<ImportRuleEntity, bool>> predicate)
        {
            try
            {
                return await _ctx.ImportRules
                    .Where(predicate)
                    .ToArrayAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ImportRuleEntity> FindById(int id)
        {
            try
            {
                var rule = await _ctx.ImportRules
                    .SingleOrDefaultAsync(x => x.Id == id);
                return rule;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ImportRuleEntity[]> FindWithPagination(Expression<Func<ImportRuleEntity, bool>> predicate, int pageNumber, int pageSize)
        {
            try
            {
                var income = await _ctx.ImportRules
                        .Where(predicate)
                        .OrderByDescending(x => x.TransactionType)
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToArrayAsync();
                return income;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> GetCount(Expression<Func<ImportRuleEntity, bool>> predicate)
        {
            try
            {
                return await _ctx.ImportRules
                .Where(predicate)
                .CountAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> Update(ImportRuleEntity entity)
        {
            try
            {
                var success = await _ctx.SaveChangesAsync();

                return entity.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}