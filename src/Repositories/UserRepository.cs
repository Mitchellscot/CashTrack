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

public interface IUserRepository
{

}
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<Users[]> Find(Expression<Func<Users, bool>> predicate)
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
    public async Task<Users> FindById(int id)
    {
        //try
        //{
        //    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        //    if (user == null)
        //    {
        //        throw new UserNotFoundException(id.ToString());
        //    }

        //    return user;
        //}
        //catch (Exception)
        //{

        //    throw;
        //}
        throw new NotImplementedException();
    }
    public Task<bool> Create(Users entity)
    {
        //not creating users
        throw new NotImplementedException();
    }
    public Task<bool> Delete(Users entity)
    {
        //not deleting
        throw new NotImplementedException();
    }
    public Task<Users[]> FindWithPagination(Expression<Func<Users, bool>> predicate, int pageNumber, int pageSize)
    {
        //not paginating users
        throw new NotImplementedException();
    }
    public Task<bool> Update(Users entity)
    {
        //might add this later, change password, email, etc.
        throw new NotImplementedException();
    }

    public async Task<int> GetCount(Expression<Func<Users, bool>> predicate)
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
