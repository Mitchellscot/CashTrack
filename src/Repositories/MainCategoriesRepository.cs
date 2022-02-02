using CashTrack.Data;
using CashTrack.Data.Entities;
using CashTrack.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CashTrack.Repositories.Common;

namespace CashTrack.Repositories.MainCategoriesRepository
{
    public interface IMainCategoriesRepository : IRepository<MainCategories>
    {
    }
    public class MainCategoriesRepository : IMainCategoriesRepository
    {
        private readonly AppDbContext _context;
        public MainCategoriesRepository(AppDbContext dbContext) => (_context) = (dbContext);

        public async Task<bool> Create(MainCategories entity)
        {
            var count = await GetCount(x => true);
            if (count >= 25)
                throw new MainCategoryLimitException(count);

            try
            {
                await _context.MainCategories.AddAsync(entity);
                return await (_context.SaveChangesAsync()) > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Delete(MainCategories entity)
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

        public async Task<MainCategories[]> Find(Expression<Func<MainCategories, bool>> predicate)
        {
            try
            {
                return await _context.MainCategories.Where(predicate).ToArrayAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<MainCategories> FindById(int id)
        {
            try
            {
                var category = await _context.MainCategories.Where(x => x.id == id).SingleOrDefaultAsync();
                if (category == null)
                    throw new CategoryNotFoundException(id.ToString());

                return category;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<MainCategories[]> FindWithPagination(Expression<Func<MainCategories, bool>> predicate, int pageNumber, int pageSize)
        {
            //25 categories max, so not implementing
            throw new NotImplementedException();
        }

        public async Task<int> GetCount(Expression<Func<MainCategories, bool>> predicate)
        {
            try
            {
                return await _context.MainCategories.CountAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Update(MainCategories entity)
        {
            try
            {
                _context.ChangeTracker.Clear();
                var contextAttachedEntity = _context.MainCategories.Attach(entity);
                contextAttachedEntity.State = EntityState.Modified;
                return await (_context.SaveChangesAsync()) > 0;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
