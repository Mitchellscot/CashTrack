using CashTrack.Data;
using CashTrack.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CashTrack.Repositories.ExpenseReviewRepository
{
    public class ExpenseReviewRepository : TransactionRepository<ExpenseReviewEntity>
    {
        private readonly AppDbContext _ctx;
        public ExpenseReviewRepository(AppDbContext ctx) : base(ctx)
        {
            _ctx = ctx;
        }
        public async override Task<ExpenseReviewEntity[]> FindWithPagination(Expression<Func<ExpenseReviewEntity, bool>> predicate, int pageNumber, int pageSize)
        {
            return await _ctx
                        .Set<ExpenseReviewEntity>()
                        .AsQueryable()
                        .Where(x => x.IsReviewed == false)
                        .Where(predicate)
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .Include(x => x.SuggestedCategory)
                        .Include(x => x.SuggestedMerchant)
                        .ToArrayAsync();
        }
        public async override Task<ExpenseReviewEntity> FindById(int id)
        {
            return await _ctx.Set<ExpenseReviewEntity>()
                        .AsQueryable()
                        .Where(x => x.IsReviewed == false)
                        .Include(x => x.SuggestedCategory)
                        .Include(x => x.SuggestedMerchant)
                        .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}
