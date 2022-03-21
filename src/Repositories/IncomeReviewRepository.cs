using CashTrack.Data;
using CashTrack.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CashTrack.Repositories.IncomeReviewRepository;

public class IncomeReviewRepository : TransactionRepository<IncomeReviewEntity>
{
    private readonly AppDbContext _ctx;
    public IncomeReviewRepository(AppDbContext ctx) : base(ctx)
    {
        _ctx = ctx;
    }
    public async override Task<IncomeReviewEntity[]> FindWithPagination(Expression<Func<IncomeReviewEntity, bool>> predicate, int pageNumber, int pageSize)
    {
        return await _ctx
                    .Set<IncomeReviewEntity>()
                    .AsQueryable()
                    .Where(predicate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Include(x => x.SuggestedCategory)
                    .Include(x => x.SuggestedSource)
                    .ToArrayAsync();
    }
    public async override Task<IncomeReviewEntity> FindById(int id)
    {
        return await _ctx.Set<IncomeReviewEntity>()
                    .AsQueryable()
                    .Include(x => x.SuggestedCategory)
                    .Include(x => x.SuggestedSource)
                    .SingleOrDefaultAsync(x => x.Id == id);
    }
}

