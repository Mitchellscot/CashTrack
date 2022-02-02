using CashTrack.Data.Entities;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CashTrack.Repositories.Common
{
    public interface IRepository<T> where T : class, IEntity
    {
        Task<T> FindById(int id);
        Task<T[]> Find(Expression<Func<T, bool>> predicate);
        Task<T[]> FindWithPagination(Expression<Func<T, bool>> predicate, int pageNumber, int pageSize);
        Task<int> GetCount(Expression<Func<T, bool>> predicate);
        Task<bool> Create(T entity);
        Task<bool> Update(T entity);
        Task<bool> Delete(T entity);
    }
}
