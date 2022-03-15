using CashTrack.Models.ExpenseModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Services.ExpenseService;
using CashTrack.Services.SubCategoryService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CashTrack.Pages.Expenses
{
    public class SplitModel : PageModel
    {
        private readonly IExpenseService _expenseService;
        private readonly ISubCategoryService _subCategoryService;

        public SplitModel(IExpenseService expenseService, ISubCategoryService subCategoryService) => (_expenseService, _subCategoryService) = (expenseService, subCategoryService);
        [BindProperty]
        public List<ExpenseSplit> Expenses { get; set; }
        public decimal Total { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Merchant { get; set; }
        [BindProperty]
        public int Split { get; set; }
        [BindProperty]
        public decimal Tax { get; set; }
        public int Id { get; set; }
        public SelectList SplitOptions { get; set; }
        public SelectList SubCategories { get; set; }
        public async Task<IActionResult> OnGet(int id, int? Split, decimal? Tax)
        {
            var originalExpense = await _expenseService.GetExpenseByIdAsync(id);
            if (originalExpense == null)
            {
                TempData["Message"] = $"Unable to find expense with id {id}";
                return LocalRedirect("./Index");
            }

            var categories = await _subCategoryService.GetSubCategoryDropdownListAsync();
            SubCategories = new SelectList(categories, nameof(SubCategoryDropdownSelection.Id), nameof(SubCategoryDropdownSelection.Category), originalExpense.categoryid);

            Id = id;
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
        [Required]
        [Range(0, 10000000, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public decimal Amount { get; set; }
        [Required]
        public int SubCategoryId { get; set; }
        public string Notes { get; set; }
        public bool Taxed { get; set; }
        public decimal Tax { get; set; }
    }
}
