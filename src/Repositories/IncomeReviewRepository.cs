using CashTrack.Data;
using CashTrack.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CashTrack.Repositories.IncomeReviewRepository;

public class IncomeReviewRepository : TransactionRepository<IncomeReview>
{
    private readonly AppDbContext _ctx;
    public IncomeReviewRepository(AppDbContext ctx) : base(ctx)
    {
        _ctx = ctx;
    }
    public async override Task<IncomeReview[]> FindWithPagination(Expression<Func<IncomeReview, bool>> predicate, int pageNumber, int pageSize)
    {
        return await _ctx
                    .Set<IncomeReview>()
                    .AsQueryable()
                    .Where(predicate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Include(x => x.suggested_category)
                    .Include(x => x.suggested_source)
                    .ToArrayAsync();
    }
    public async override Task<IncomeReview> FindById(int id)
    {
        return await _ctx.Set<IncomeReview>()
                    .AsQueryable()
                    .Include(x => x.suggested_category)
                    .Include(x => x.suggested_source)
                    .SingleOrDefaultAsync(x => x.Id == id);
    }
}

