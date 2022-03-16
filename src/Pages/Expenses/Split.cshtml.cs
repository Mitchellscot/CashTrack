using CashTrack.Models.ExpenseModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Services.ExpenseService;
using CashTrack.Services.MerchantService;
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
        private readonly IMerchantService _merchantService;

        public SplitModel(IExpenseService expenseService, ISubCategoryService subCategoryService, IMerchantService merchantService) => (_expenseService, _subCategoryService, _merchantService) = (expenseService, subCategoryService, merchantService);

        [BindProperty]
        public List<ExpenseSplit> ExpenseSplits { get; set; }
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
        public async Task<IActionResult> OnPost(List<ExpenseSplit> expenseSplits)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                foreach (var expenseSplit in expenseSplits)
                {
                    //converting merchant name to a string id... kind of dumb but whatever.
                    expenseSplit.Merchant = (await _merchantService.GetMerchantByNameAsync(expenseSplit.Merchant)).Id.ToString();
                    var success = await _expenseService.CreateExpenseFromSplitAsync(expenseSplit);
                    if (!success)
                    {
                        ModelState.AddModelError("", "Unable to split the expenses - please try again");
                        return Page();
                    }
                }
                var deleteSuccess = await _expenseService.DeleteExpenseAsync(int.Parse(RouteData.Values["id"].ToString()));
                if (!deleteSuccess)
                {
                    ModelState.AddModelError("", "Unable to delete the original expenses - please try again");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Page();
            }
            TempData["Message"] = "Sucessfully Split the Expense!";
            return LocalRedirect("~/Expenses/Index");
        }
    }
}
