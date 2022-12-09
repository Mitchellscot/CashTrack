using CashTrack.Repositories.BudgetRepository;
using CashTrack.Repositories.ExpenseRepository;
using CashTrack.Repositories.IncomeRepository;
using CashTrack.Services.BudgetService;

namespace CashTrack.Services.DashboardService
{
    public interface IDashboardService
    {

    }
    public class DashboardService : IDashboardService
    {
        private readonly IBudgetRepository _budgetRepo;
        private readonly IExpenseRepository _expenseRepo;
        private readonly IIncomeRepository _incomeRepo;

        public DashboardService(IBudgetRepository budgetRepository, IExpenseRepository expenseRepository, IIncomeRepository incomeRepository)
        {
            _budgetRepo = budgetRepository;
            _expenseRepo = expenseRepository;
            _incomeRepo = incomeRepository;
        }
    }
}
