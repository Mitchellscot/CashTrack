using CashTrack.Data;
using CashTrack.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CashTrack.Repositories.ExpenseReviewRepository
{
    public class ExpenseReviewRepository : TransactionRepository<ExpenseReview>
    {
        private readonly AppDbContext _ctx;
        public ExpenseReviewRepository(AppDbContext ctx) : base(ctx)
        {
            _ctx = ctx;
        }
        public async override Task<ExpenseReview[]> FindWithPagination(Expression<Func<ExpenseReview, bool>> predicate, int pageNumber, int pageSize)
        {
            return await _ctx
                        .Set<ExpenseReview>()
                        .AsQueryable()
                        .Where(x => x.is_reviewed == false)
                        .Where(predicate)
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .Include(x => x.suggested_category)
                        .Include(x => x.suggested_merchant)
                        .ToArrayAsync();
        }
        public async override Task<ExpenseReview> FindById(int id)
        {
            return await _ctx.Set<ExpenseReview>()
                        .AsQueryable()
                        .Where(x => x.is_reviewed == false)
                        .Include(x => x.suggested_category)
                        .Include(x => x.suggested_merchant)
                        .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}
