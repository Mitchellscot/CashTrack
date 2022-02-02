using CashTrack.Data;
using CashTrack.Data.Entities;
using CashTrack.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CashTrack.Repositories.TagRepository;

public interface ITagRepository : IRepository<Tags>
{
}
public class TagRepository : ITagRepository
{
    private readonly AppDbContext _context;

    public TagRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<bool> Create(Tags entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Delete(Tags entity)
    {
        throw new NotImplementedException();
    }

    public async Task<Tags[]> Find(Expression<Func<Tags, bool>> predicate)
    {
        try
        {
            var tags = await _context.Tags.Where(predicate).ToArrayAsync();
            return tags;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public Task<Tags> FindById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Tags[]> FindWithPagination(Expression<Func<Tags, bool>> predicate, int pageNumber, int pageSize)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetCount(Expression<Func<Tags, bool>> predicate)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Update(Tags entity)
    {
        throw new NotImplementedException();
    }
}

