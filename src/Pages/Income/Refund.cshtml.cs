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
        public List<Expense> SelectedExpenses { get; set; }
        [BindProperty]
        public List<int> ExpenseSearchChosenIds { get; set; } = new();

        public async Task<IActionResult> OnGet(int id, string Query)
        {
            Income = await _incomeService.GetIncomeByIdAsync(id);
            Total = Income.Amount;
            return Page();
        }
        public async Task<IActionResult> OnPostQuery(string Query, int incomeId)
        {

            //when a search is made,
            //Post the query so we can get the expense search results
            //Each row in the expense search table is a small form that posts the individual id
            //and when that posts, it sends the ID to the other POST method
            //which adds the id to a list, which then rewrites the form to include that id
            //so a list of ids builds up in the expense query table
            //and the selected query table at the same time
            //that way you can do a search, add an expense, do a search, add an expense, and so on until the
            //selected expense table is full of the amount of expenses you need
            //after that you post to another handler method that
            //processes all the refunds
            //
            //kind of complicated, but it's just managing state across two different forms
            if (Query != null)
            {
                DateTime q;
                if (!DateTime.TryParse(Query, out q))
                {
                    ModelState.AddModelError(string.Empty, "Date is not in desired format, please try again");
                    return Page();
                }
                ExpenseSearch = await _expenseService.GetExpensesByDateWithoutPaginationAsync(q);
            }
            if (ExpenseSearchChosenIds.Any())
            {
                SelectedExpenses = new();

                foreach (var id in ExpenseSearchChosenIds)
                {
                    var selectedExpense = await _expenseService.GetExpenseByIdAsync(id);
                    SelectedExpenses.Add(selectedExpense);
                }
            }
            return Page();
        }
        public async Task<IActionResult> OnPostSelectExpense(string Query)
        {
            ExpenseSearchChosenIds = ExpenseSearchChosenIds.Distinct().ToList(); //ensures we don't have any duplicates
            if (ExpenseSearchChosenIds.Count() > 0)
            {
                SelectedExpenses = new();

                foreach (var id in ExpenseSearchChosenIds)
                {
                    var selectedExpense = await _expenseService.GetExpenseByIdAsync(id);
                    SelectedExpenses.Add(selectedExpense);
                }
            }
            if (Query != null)
            {
                DateTime q;
                if (!DateTime.TryParse(Query, out q))
                {
                    ModelState.AddModelError(string.Empty, "Date is not in desired format, please try again");
                    return Page();
                }
                ExpenseSearch = await _expenseService.GetExpensesByDateWithoutPaginationAsync(q);
            }

            return Page();
        }
    }
}
