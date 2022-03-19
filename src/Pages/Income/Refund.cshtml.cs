using CashTrack.Models.Common;
using CashTrack.Models.ExpenseModels;
using CashTrack.Services.ExpenseService;
using CashTrack.Services.IncomeService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
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
        public Expense[] ExpenseSearch { get; set; }
        public List<Expense> SelectedExpenses { get; set; }

        public async Task<IActionResult> OnGet(int id, string Query)
        {
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


            Income = await _incomeService.GetIncomeByIdAsync(id);
            Total = Income.Amount;
            return Page();
        }
        public async Task<IActionResult> OnPost(List<int> expenseIds, string Query)
        {
            foreach (var id in expenseIds)
            {
                var selectedExpense = await _expenseService.GetExpenseByIdAsync(id);
                SelectedExpenses.Add(selectedExpense);
            }
            return Page();
        }
    }
}
