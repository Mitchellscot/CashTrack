using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using CashTrack.Data.Entities;
using CashTrack.Data;
using CashTrack.Common.Exceptions;
using System.Linq.Expressions;
using CashTrack.Repositories.Common;

namespace CashTrack.Repositories.UserRepository;

public interface IUserRepository : IRepository<UserEntity>
{

}
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<UserEntity[]> Find(Expression<Func<UserEntity, bool>> predicate)
    {
        try
        {
            var users = await _context.Users.Where(predicate).ToArrayAsync();
            return users;
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<UserEntity> FindById(int id)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                throw new UserNotFoundException(id.ToString());
            }

            return user;
        }
        catch (Exception)
        {

            throw;
        }
        throw new NotImplementedException();
    }
    public Task<bool> Create(UserEntity entity)
    {
        //using userManager to create
        throw new NotImplementedException();
    }
    public Task<bool> Delete(UserEntity entity)
    {
        //using userManager to delete
        throw new NotImplementedException();
    }
    public Task<UserEntity[]> FindWithPagination(Expression<Func<UserEntity, bool>> predicate, int pageNumber, int pageSize)
    {
        //not paginating users
        throw new NotImplementedException();
    }
    public Task<bool> Update(UserEntity entity)
    {
        //using userManager to update
        throw new NotImplementedException();
    }

    public async Task<int> GetCount(Expression<Func<UserEntity, bool>> predicate)
    {
        try
        {
            var users = await _context.Users.CountAsync(predicate);
            return users;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
