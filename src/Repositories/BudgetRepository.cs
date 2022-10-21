using CashTrack.Data.Entities;
using CashTrack.Repositories.Common;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CashTrack.Repositories.BudgetRepository
{
    public interface IBudgetRepository : IRepository<BudgetEntity>
    {
    }
    public class BudgetRepository : IBudgetRepository
    {
        public Task<int> Create(BudgetEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(BudgetEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task<BudgetEntity[]> Find(Expression<Func<BudgetEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<BudgetEntity> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<BudgetEntity[]> FindWithPagination(Expression<Func<BudgetEntity, bool>> predicate, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetCount(Expression<Func<BudgetEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<int> Update(BudgetEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
