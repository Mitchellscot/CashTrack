using CashTrack.Common.Exceptions;
using CashTrack.Models.Common;
using CashTrack.Models.ExpenseModels;
using CashTrack.Models.MerchantModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.ExpenseService;
using CashTrack.Services.MerchantService;
using CashTrack.Services.SubCategoryService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;

namespace CashTrack.Pages.Expenses
{
    public class Index : PageModelBase
    {
        private readonly IExpenseService _expenseService;
        private readonly IMerchantService _merchantService;
        private readonly ISubCategoryService _subCategoryService;

        public Index(IExpenseService expenseService, IMerchantService merchantService, ISubCategoryService subCategoryService) =>
            (_expenseService, _merchantService, _subCategoryService) = (expenseService, merchantService, subCategoryService);

        [BindProperty(SupportsGet = true)]
        public string Q { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Q2 { get; set; }
        [BindProperty(SupportsGet = true)]
        public int Query { get; set; }
        public ExpenseResponse ExpenseResponse { get; set; }
        public SelectList QueryOptions { get; set; }
        public SubCategoryDropdownSelection[] CategoryList { get; set; }
        public string InputType { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        [BindProperty]
        public Expense Expense { get; set; }

        public async Task<ActionResult> OnGet(string q, int query, string q2, int pageNumber)
        {
            await PrepareForm(query);

            if (query == 0 && q != null)
            {
                ExpenseResponse = await _expenseService.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.SpecificDate, BeginDate = DateTimeOffset.Parse(q), PageNumber = pageNumber });
                return Page();
            }
            if (query == 1 && q != null && q2 != null)
            {
                ExpenseResponse = await _expenseService.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.DateRange, BeginDate = DateTimeOffset.Parse(q), EndDate = DateTimeOffset.Parse(q2), PageNumber = pageNumber });
                return Page();
            }
            if (query == 2 && q != null)
            {
                ExpenseResponse = await _expenseService.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.SpecificMonthAndYear, BeginDate = DateTimeOffset.Parse(q), PageNumber = pageNumber });
                return Page();
            }
            if (query == 3 && q != null)
            {
                ExpenseResponse = await _expenseService.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.SpecificQuarter, BeginDate = DateTimeOffset.Parse(q), PageNumber = pageNumber });
                return Page();
            }
            if (query == 4 && q != null)
            {
                int year;
                if (int.TryParse(q, out year))
                {
                    ExpenseResponse = await _expenseService.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.SpecificYear, BeginDate = new DateTime(year, 1, 1), PageNumber = pageNumber });
                    return Page();
                }
                else
                {
                    ModelState.AddModelError("", "Unable to convert the given year. Please try again.");
                    return Page();
                }
            }
            if (query == 5 && q != null)
            {
                decimal amount;
                if (Decimal.TryParse(q, out amount))
                {
                    ExpenseResponse = await _expenseService.GetExpensesByAmountAsync(new AmountSearchRequest() { Query = amount, PageNumber = pageNumber });
                    return Page();
                }
                else
                {
                    ModelState.AddModelError("", "Unable to convert the given amount. Please try again.");
                    return Page();
                }
            }

            if (query == 6 && q != null)
            {
                ExpenseResponse = await _expenseService.GetExpensesByNotesAsync(new ExpenseRequest() { Query = q, PageNumber = pageNumber });
                return Page();
            }
            if (query == 7 && q != null)
            {
                ExpenseResponse = await _expenseService.GetExpensesByMerchantAsync(new ExpenseRequest() { Query = q, PageNumber = pageNumber });
                return Page();
            }
            if (query == 8 && q != null)
            {
                ExpenseResponse = await _expenseService.GetExpensesBySubCategoryIdAsync(new ExpenseRequest() { Query = q, PageNumber = pageNumber });
                return Page();
            }
            if (query == 9 && q != null)
            {
                ExpenseResponse = await _expenseService.GetExpensesByMainCategoryAsync(new ExpenseRequest() { Query = q, PageNumber = pageNumber });
                return Page();
            }
            if (query == 10 && q != null)
            {
                ModelState.AddModelError("", "Not Implemented Yet");
                return Page();
            }
            if (query == 11)
            {
                ExpenseResponse = await _expenseService.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.CurrentMonth, PageNumber = pageNumber });
                return Page();
            }
            if (query == 12)
            {
                ExpenseResponse = await _expenseService.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.CurrentQuarter, PageNumber = pageNumber });
                return Page();
            }
            if (query == 13)
            {
                ExpenseResponse = await _expenseService.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.CurrentYear, PageNumber = pageNumber });
                return Page();
            }
            if (query == 14)
            {
                ExpenseResponse = await _expenseService.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.Last30Days, PageNumber = pageNumber });
                return Page();
            }

            ExpenseResponse = await _expenseService.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.All, PageNumber = this.PageNumber });
            return Page();
        }
        public async Task<IActionResult> OnPostDelete(int expenseId, int query, string q, string q2, int pageNumber)
        {
            var success = await _expenseService.DeleteExpenseAsync(expenseId);
            if (!success)
            {
                ModelState.AddModelError("", "Unable to delete the expense");
                return Page();
            }
            TempData["SuccessMessage"] = "Sucessfully deleted expense!";
            return RedirectToPage("./Index", new { query = query, q = q, q2 = q2, pageNumber = pageNumber });
        }
        public async Task<IActionResult> OnPostAddEdit()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                if (Expense.CreateNewMerchant && !string.IsNullOrEmpty(Expense.Merchant))
                {
                    var merchantCreationSuccess = await _merchantService.CreateMerchantAsync(new Merchant() { Name = Expense.Merchant, SuggestOnLookup = true });
                }

                var success = Expense.Id.HasValue ? await _expenseService.UpdateExpenseAsync(Expense) : await _expenseService.CreateExpenseAsync(Expense);

                TempData["SuccessMessage"] = Expense.Id.HasValue ? "Sucessfully updated the Expense!" : "Sucessfully added a new Expense!";
                return RedirectToPage("./Index", new { query = Query, q = Q, q2 = Q2, pageNumber = PageNumber });
            }
            catch (CategoryNotFoundException)
            {
                ModelState.AddModelError("", "Please select a category and try again");
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Page();
            }
        }
        private async Task PrepareForm(int query)
        {
            PageNumber = ExpenseResponse != null ? ExpenseResponse.PageNumber : PageNumber == 0 ? 1 : PageNumber;
            QueryOptions = new SelectList(ExpenseQueryOptions.GetAll, "Key", "Value", query);
            CategoryList = await _subCategoryService.GetSubCategoryDropdownListAsync();

            switch (query)
            {
                case 0 or 1:
                    InputType = "date";
                    break;
                case 2 or 3:
                    InputType = "month";
                    break;
                case 4 or 5:
                    InputType = "number";
                    break;
                case 6 or 7 or 8 or 9 or 10:
                    InputType = "text";
                    break;
                default:
                    InputType = "date";
                    break;
            }
        }
    }
}
