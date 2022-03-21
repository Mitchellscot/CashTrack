using CashTrack.Data;
using CashTrack.Data.Entities;
using CashTrack.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CashTrack.Repositories.TagRepository;

public interface ITagRepository : IRepository<TagEntity>
{
}
public class TagRepository : ITagRepository
{
    private readonly AppDbContext _context;

    public TagRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<bool> Create(TagEntity entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Delete(TagEntity entity)
    {
        throw new NotImplementedException();
    }

    public async Task<TagEntity[]> Find(Expression<Func<TagEntity, bool>> predicate)
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

    public Task<TagEntity> FindById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<TagEntity[]> FindWithPagination(Expression<Func<TagEntity, bool>> predicate, int pageNumber, int pageSize)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetCount(Expression<Func<TagEntity, bool>> predicate)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Update(TagEntity entity)
    {
        throw new NotImplementedException();
    }
}

