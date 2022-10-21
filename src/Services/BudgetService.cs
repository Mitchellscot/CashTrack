using CashTrack.Repositories.BudgetRepository;

namespace CashTrack.Services.BudgetService
{
    public interface IBudgetService
    {

    }
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository _budgetRepo;
        public BudgetService(IBudgetRepository budgetRepo) => (_budgetRepo) = (budgetRepo);
    }
}
