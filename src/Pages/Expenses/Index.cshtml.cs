using CashTrack.Common.Exceptions;
using CashTrack.Models.Common;
using CashTrack.Models.ExpenseModels;
using CashTrack.Models.MerchantModels;
using CashTrack.Services.ExpenseService;
using CashTrack.Services.MerchantService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CashTrack.Pages.Expenses
{
    public class Index : PageModel
    {
        private readonly IExpenseService _expenseService;
        private readonly IMerchantService _merchantService;

        public Index(IExpenseService expenseService, IMerchantService merchantService) =>
            (_expenseService, _merchantService) = (expenseService, merchantService);

        [BindProperty(SupportsGet = true)]
        public string q { get; set; }
        [BindProperty(SupportsGet = true)]
        public string q2 { get; set; }
        [BindProperty(SupportsGet = true)]
        public int query { get; set; }
        public ExpenseResponse ExpenseResponse { get; set; }
        public SelectList queryOptions { get; set; }
        public string inputType { get; set; }
        [BindProperty(SupportsGet = true)]
        public int pageNumber { get; set; } = 1;
        [TempData]
        public string Message { get; set; }
        [BindProperty]
        public AddEditExpense AddExpense { get; set; }
        [BindProperty]
        public string MerchantName { get; set; }
        [BindProperty]
        public bool CreateNewMerchant { get; set; }

        public async Task<ActionResult> OnGet(string q, int query, string q2, int pageNumber)
        {
            PrepareForm(query);


            if (query == 0 && q != null)
            {
                ExpenseResponse = await _expenseService.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.SpecificDate, BeginDate = DateTime.Parse(q), PageNumber = pageNumber });
                return Page();
            }
            if (query == 1 && q != null && q2 != null)
            {
                ExpenseResponse = await _expenseService.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.DateRange, BeginDate = DateTime.Parse(q), EndDate = DateTime.Parse(q2), PageNumber = pageNumber });
                return Page();
            }
            if (query == 2 && q != null)
            {
                ExpenseResponse = await _expenseService.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.SpecificMonthAndYear, BeginDate = DateTime.Parse(q), PageNumber = pageNumber });
                return Page();
            }
            if (query == 3 && q != null)
            {
                ExpenseResponse = await _expenseService.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.SpecificQuarter, BeginDate = DateTime.Parse(q), PageNumber = pageNumber });
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
            if (query == 5)
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

            if (query == 6)
            {
                ExpenseResponse = await _expenseService.GetExpensesByNotesAsync(new ExpenseRequest() { Query = q, PageNumber = pageNumber });
                return Page();
            }
            if (query == 7)
            {
                ExpenseResponse = await _expenseService.GetExpensesByMerchantAsync(new ExpenseRequest() { Query = q, PageNumber = pageNumber });
                return Page();
            }
            if (query == 8)
            {
                ExpenseResponse = await _expenseService.GetExpensesBySubCategoryIdAsync(new ExpenseRequest() { Query = q, PageNumber = pageNumber });
                return Page();
            }
            if (query == 9)
            {
                ModelState.AddModelError("", "Not Implemented Yet");
                return Page();
            }
            if (query == 10)
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

            ExpenseResponse = await _expenseService.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.All, PageNumber = this.pageNumber });
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
            TempData["Message"] = "Sucessfully deleted expense!";
            return RedirectToPage("./Index", new { query = query, q = q, q2 = q2, pageNumber = pageNumber });
        }
        public async Task<IActionResult> OnPostAdd(AddEditExpense AddExpense)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            //when adding a new expense, we need to get a merchant id to associate with a merchant.
            //a merchant isn't required, but if one is offered, it needs to be an existing one, if it doesn't exist
            //we need to create one. This only works if "create new merchant" is checked on the form.
            if (MerchantName != null)
            {
                try
                {
                    AddExpense.MerchantId = (await _merchantService.GetMerchantByNameAsync(MerchantName)).Id;
                }
                catch (Exception ex) when (ex is MerchantNotFoundException)
                {
                    if (!CreateNewMerchant)
                    {
                        ModelState.AddModelError("", "Check \"Create New Merchant\" and try again.");
                        return Page();
                    }

                    var merchantCreationSuccess = await _merchantService.CreateMerchantAsync(new AddEditMerchant() { Name = MerchantName });
                    if (merchantCreationSuccess == null)
                    {
                        ModelState.AddModelError("", "Unable to Add the expense - error involving merchant creation.");
                        return Page();
                    }
                    AddExpense.MerchantId = merchantCreationSuccess.Id;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return Page();
                }
            }
            var success = await _expenseService.CreateExpenseAsync(AddExpense);
            if (success == null)
            {
                ModelState.AddModelError("", "Unable to Add the expense");
                return Page();
            }
            TempData["Message"] = "Sucessfully added a new Expense!";
            return RedirectToPage("./Index", new { query = query, q = q, q2 = q2, pageNumber = pageNumber });
        }

        private void PrepareForm(int query)
        {
            if (ExpenseResponse != null)
            {
                pageNumber = ExpenseResponse.PageNumber;
            }
            queryOptions = new SelectList(ExpenseQueryOptions.GetAll, "Key", "Value", query);

            switch (query)
            {
                case 0 or 1:
                    inputType = "date";
                    break;
                case 2 or 3:
                    inputType = "month";
                    break;
                case 4 or 5:
                    inputType = "number";
                    break;
                case 6 or 7 or 8 or 9 or 10:
                    inputType = "text";
                    break;
                default:
                    inputType = "date";
                    break;
            }
        }
    }
}
