using CashTrack.Models.Common;
using CashTrack.Models.ExpenseModels;
using CashTrack.Services.ExpenseService;
using CashTrack.Services.IncomeService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashTrack.Pages.Incomes
{
    public class RefundModel : PageModel
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
        public List<ExpenseRefund> SelectedExpenses { get; set; }
        [BindProperty]
        public List<int> ExpenseSearchChosenIds { get; set; } = new();
        public int SelectedId { get; set; }

        public async Task<IActionResult> OnGet(int id, string Query)
        {
            Income = await _incomeService.GetIncomeByIdAsync(id);
            Total = Income.Amount;
            return Page();
        }
        public async Task<IActionResult> OnPostQuery(string Query)
        {
            await GetExpensesFromQuery(Query);
            await GetExpenseRefundsFromSelectedIds();
            return Page();
        }

        public async Task<IActionResult> OnPostSelectExpense(string Query, int SelectedId)
        {
            ExpenseSearchChosenIds.Add(SelectedId);
            ExpenseSearchChosenIds = ExpenseSearchChosenIds.Distinct().ToList(); //ensures we don't have any duplicates
            await GetExpenseRefundsFromSelectedIds();
            await GetExpensesFromQuery(Query);

            return Page();
        }
        public async Task<IActionResult> OnPostRemoveExpense(string Query, int SelectedId)
        {

            ExpenseSearchChosenIds = ExpenseSearchChosenIds.Distinct().ToList(); //ensures we don't have any duplicates
            ExpenseSearchChosenIds.Remove(SelectedId);
            await GetExpenseRefundsFromSelectedIds();
            await GetExpensesFromQuery(Query);

            return Page();
        }
        public async Task<IActionResult> OnPostApplyRefunds()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                if (Income.Amount != SelectedExpenses.Sum(x => x.RefundAmount))
                {
                    ModelState.AddModelError("", "You must refund the entire income amount.");
                    return Page();
                }
                var updateSuccess = await _expenseService.RefundExpensesAsync(SelectedExpenses, Income);
                //post some expense refund objects
                //get the expenses from the IDS on the ExpenseRefunds
                //Update all the expenses: If (ModifiedAmount > 0) { expense.Amount = expenseRefund.ModifiedAmount }
                //Add a refund note - OriginalAmount: expense.Amount - Amount Refunded: expenseRefund.RefundAmount
                //- Date Refunded: incomeRefund.Date
                //add a note to the income
                //for each expense incomeis applied to
                //Applied refund to an expense on expense.Date for the amount of expense.RefundAmount
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Page();
            }
            TempData["Message"] = "Sucessfully Applied the Refund!";
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
    }
}
