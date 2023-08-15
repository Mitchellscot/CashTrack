using CashTrack.Common.Exceptions;
using CashTrack.Data;
using CashTrack.Data.Entities;
using CashTrack.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CashTrack.Repositories.ImportRepository;

public interface IImportProfileRepository : IRepository<ImportProfileEntity>
{
    Task<string[]> GetProfileNames();
}
public class ImportProfileRepository : IImportProfileRepository
{
    private readonly AppDbContext _ctx;
    public ImportProfileRepository(AppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<int> Create(ImportProfileEntity entity)
    {
        try
        {
            var profile = await _ctx.ImportProfiles.AddAsync(entity);
            return await _ctx.SaveChangesAsync() > 0 ? profile.Entity.Id : throw new Exception("Unable to save the profile");
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<bool> Delete(ImportProfileEntity entity)
    {
        try
        {
            _ctx.ImportProfiles.Remove(entity);
            return await (_ctx.SaveChangesAsync()) > 0;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ImportProfileEntity[]> Find(Expression<Func<ImportProfileEntity, bool>> predicate)
    {
        try
        {
            return await _ctx.ImportProfiles
                .Where(predicate)
                .ToArrayAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ImportProfileEntity> FindById(int id)
    {
        try
        {
            var profile = await _ctx.ImportProfiles
                .FirstOrDefaultAsync(x => x.Id == id);
            if (profile == null)
                throw new ImportProfileNotFoundException(id.ToString());

            return profile;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public Task<ImportProfileEntity[]> FindWithPagination(Expression<Func<ImportProfileEntity, bool>> predicate, int pageNumber, int pageSize)
    {
        throw new NotImplementedException();
    }

    public async Task<int> GetCount(Expression<Func<ImportProfileEntity, bool>> predicate)
    {
        try
        {
            return await _ctx.ImportProfiles
            .Where(predicate)
            .CountAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<string[]> GetProfileNames()
    {
        try
        {
            return await _ctx.ImportProfiles.Select(x => x.Name).ToArrayAsync();
        }
        catch(Exception) 
        {
            throw;
        }
    }

    public async Task<int> Update(ImportProfileEntity entity)
    {
        try
        {
            var saveSuccess = await _ctx.SaveChangesAsync();
            return saveSuccess > 0 ? entity.Id : throw new Exception("Unable to save the import profile");
        }
        catch (Exception)
        {
            throw;
        }
    }
}
