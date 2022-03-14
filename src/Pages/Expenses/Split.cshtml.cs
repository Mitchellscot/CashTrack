using CashTrack.Models.ExpenseModels;
using CashTrack.Services.ExpenseService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashTrack.Pages.Expenses
{
    public class SplitModel : PageModel
    {
        private readonly IExpenseService _expenseService;
        public SplitModel(IExpenseService expenseService) => _expenseService = expenseService;
        [BindProperty]
        public List<ExpenseSplit> Expenses { get; set; }
        public decimal Total { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Merchant { get; set; }
        [BindProperty]
        public int Split { get; set; }
        [BindProperty]
        public decimal Tax { get; set; }
        //public SelectList SubCategoryList { get; set; }
        public SelectList SplitOptions { get; set; }
        public async Task<IActionResult> OnGet(int id, int? Split, decimal? Tax)
        {
            var originalExpense = await _expenseService.GetExpenseByIdAsync(id);
            if (originalExpense == null)
            {
                TempData["Message"] = $"Unable to find expense with id {id}";
                return LocalRedirect("./Index");
            }

            Total = originalExpense.amount;
            Date = originalExpense.date;
            Merchant = originalExpense.merchant.name;
            this.Tax = Tax ?? 0.0875M;
            this.Split = Split ?? 2;
            SplitOptions = new SelectList(Enumerable.Range(2, 7));
            //SubCategoryList = DO THIS NEXT
            return Page();
        }
        public void OnPost()
        {
            //Take in the list of ExpenseSplits
            //Run calculations to make sure it all adds up to the original total, if not return page
            //Create new Expense for each one, assigning the merchant and date and all other properties
            //if new expenses are created, delete the original expense and return to /Expenses
        }
    }
    public class ExpenseSplit
    {
        public decimal Amount { get; set; }
        public int SubCategoryId { get; set; }
        public string Notes { get; set; }
        public bool Taxed { get; set; }
    }
}
