using CashTrack.Common.Exceptions;
using CashTrack.Models.ExpenseModels;
using CashTrack.Models.MerchantModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Services.ExpenseService;
using CashTrack.Services.MerchantService;
using CashTrack.Services.SubCategoryService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CashTrack.Pages.Expenses
{
    public class EditModel : PageModel
    {
        private readonly IExpenseService _expenseService;
        private readonly ISubCategoryService _subCategoryService;
        public IMerchantService _merchantService { get; }

        public EditModel(IExpenseService expenseService, ISubCategoryService subCategoryService, IMerchantService merchantService)
        {
            _expenseService = expenseService;
            _subCategoryService = subCategoryService;
            _merchantService = merchantService;
        }
        [BindProperty]
        public AddEditExpense ExpenseEdit { get; set; }
        [BindProperty]
        public List<AddEditExpense> BulkExpenses { get; set; }
        public SelectList SubCategories { get; set; }
        public string InitialMainCategory { get; set; }
        [BindProperty(SupportsGet = true)]
        public int Split { get; set; }
        [BindProperty]
        public string MerchantName { get; set; }
        [BindProperty]
        public bool CreateNewMerchant { get; set; }
        [BindProperty(SupportsGet = true)]
        public int id { get; set; }

        public async Task<ActionResult> OnGet(int id, [FromQuery] int split)
        {
            await PrepareForm(id);
            return Page();
        }
        private async Task PrepareForm(int id)
        {
            var expense = await _expenseService.GetExpenseByIdAsync(id);
            ExpenseEdit = new AddEditExpense()
            {
                Date = expense.date,
                Amount = expense.amount,
                SubCategoryId = expense.categoryid.Value,
                MerchantId = expense.merchantid ?? expense.merchantid.Value,
                ExcludeFromStatistics = expense.exclude_from_statistics,
                Notes = expense.notes,
                Id = expense.Id
            };
            var categories = await _subCategoryService.GetAllSubCategoriesAsync();
            SubCategories = new SelectList(categories, nameof(SubCategoryListItem.Id), nameof(SubCategoryListItem.Name), expense.categoryid);
            InitialMainCategory = expense.category.main_category.main_category_name;
            MerchantName = expense.merchant.name ?? expense.merchant.name;
        }
        public async Task<ActionResult> OnPostSingleExpense()
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Fix your crap");
                return Page();
            }
            if (MerchantName != null)
            {
                try
                {
                    ExpenseEdit.MerchantId = (await _merchantService.GetMerchantByNameAsync(MerchantName)).Id;
                }
                catch (Exception ex) when (ex is MerchantNotFoundException)
                {
                    if (!CreateNewMerchant)
                    {
                        await PrepareForm(id);
                        ModelState.AddModelError("", "Check \"Create New Merchant\" and try again.");
                        return Page();
                    }

                    var merchantCreationSuccess = await _merchantService.CreateMerchantAsync(new AddEditMerchant() { Name = MerchantName });
                    if (merchantCreationSuccess == null)
                    {
                        await PrepareForm(id);
                        ModelState.AddModelError("", "Unable to Add the expense - error involving merchant creation.");
                        return Page();
                    }
                    ExpenseEdit.MerchantId = merchantCreationSuccess.Id;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return Page();
                }
            }
            var success = await _expenseService.UpdateExpenseAsync(ExpenseEdit);
            if (!success)
            {
                ModelState.AddModelError("", "Error saving the expense!");
                return Page();
            }
            TempData["Message"] = "Sucessfully updated the Expense!";
            return RedirectToPage("./Index");
        }
        public async Task<ActionResult> OnPostMultipleExpenses(List<AddEditExpense> expenses)
        {
            //break here to test and inspect
            foreach (var expense in expenses)
            {
                var success = await _expenseService.CreateExpenseAsync(expense);
            }
            return Page();
        }
    }
}
