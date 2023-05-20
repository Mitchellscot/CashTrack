using CashTrack.Models.ExpenseModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.ExpenseService;
using CashTrack.Services.IncomeService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashTrack.Pages.Incomes
{
    public class RefundModel : PageModelBase
    {
        private readonly IIncomeService _incomeService;
        private readonly IExpenseService _expenseService;

        public RefundModel(IIncomeService incomeService, IExpenseService expenseService)
        {
            _incomeService = incomeService;
            _expenseService = expenseService;
        }
        public decimal Total { get; set; }
        [BindProperty]
        public string Query { get; set; }
        public CashTrack.Models.IncomeModels.Income Income { get; set; }
        [BindProperty]
        public Expense[] ExpenseSearch { get; set; }
        [BindProperty]
        public List<ExpenseRefund> SelectedExpenses { get; set; }
        [BindProperty]
        public List<ExpenseRefund> ExpenseRefunds { get; set; }
        [BindProperty]
        public List<int> ExpenseSearchChosenIds { get; set; } = new();
        [BindProperty]
        public int SelectedId { get; set; }

        public async Task<IActionResult> OnGet(int id)
        {
            await SetTotal(id);
            return Page();
        }
        public async Task<IActionResult> OnPostQuery(int id, string Query)
        {
            await GetExpensesFromQuery(Query);
            await GetExpenseRefundsFromSelectedIds();
            await SetTotal(id);
            return Page();
        }

        public async Task<IActionResult> OnPostSelectExpense(int id, string Query, int SelectedId)
        {
            ExpenseSearchChosenIds.Add(SelectedId);
            ExpenseSearchChosenIds = ExpenseSearchChosenIds.Distinct().ToList(); //ensures we don't have any duplicates
            await GetExpenseRefundsFromSelectedIds();
            await GetExpensesFromQuery(Query);
            await SetTotal(id);
            return Page();
        }
        public async Task<IActionResult> OnPostRemoveExpense(int id, string Query)
        {
            ExpenseSearchChosenIds.Remove(SelectedId);
            SelectedExpenses.RemoveAll(x => x.Id == SelectedId);
            await GetExpensesFromQuery(Query);
            await SetTotal(id);
            return Page();
        }
        public async Task<IActionResult> OnPostApplyRefunds(int id)
        {
            await SetTotal(id);
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                if (Income.Amount != SelectedExpenses.Select(x => x.RefundAmount).Sum())
                {
                    ModelState.AddModelError("", "You must refund the entire income amount.");
                    return Page();
                }
                var updateSuccess = await _expenseService.RefundExpensesAsync(SelectedExpenses, Income.Id.Value);
                if (!updateSuccess)
                {
                    ModelState.AddModelError("", "There was a problem applying your refunds. Please try again.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Page();
            }
            SuccessMessage = "Sucessfully Applied the Refund!";
            return LocalRedirect("~/Income/Index");
        }
        public async Task<IActionResult> OnPostDelete(int incomeId)
        {
            var success = await _incomeService.DeleteIncomeAsync(incomeId);
            if (!success)
            {
                ModelState.AddModelError("", "Unable to delete the Refund");
                return Page();
            }
            SuccessMessage = "Sucessfully deleted Refund!";
            return LocalRedirect("~/Income/Index");
        }
        private async Task GetExpenseRefundsFromSelectedIds()
        {
            if (ExpenseSearchChosenIds.Any())
            {
                SelectedExpenses = new();

                foreach (var id in ExpenseSearchChosenIds)
                {
                    var selectedExpense = await _expenseService.GetExpenseRefundByIdAsync(id);
                    SelectedExpenses.Add(selectedExpense);
                }
            }
        }
        private async Task GetExpensesFromQuery(string Query)
        {
            if (Query != null)
            {
                DateTime q;
                if (!DateTime.TryParse(Query, out q))
                {
                    ModelState.AddModelError(string.Empty, "Date is not in desired format, please try again");
                }
                else
                {
                    ExpenseSearch = await _expenseService.GetExpensesByDateWithoutPaginationAsync(q);
                }
            }
        }
        private async Task SetTotal(int id)
        {
            Income = await _incomeService.GetIncomeByIdAsync(id);
            Total = Income.Amount;
        }
    }
}
