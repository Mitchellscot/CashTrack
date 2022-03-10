using CashTrack.Models.Common;
using CashTrack.Models.ExpenseModels;
using CashTrack.Services.ExpenseService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;

namespace CashTrack.Pages.Expenses
{
    public class Index : PageModel
    {
        private readonly IExpenseService _service;

        public Index(IExpenseService service) => _service = service;
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

        public async Task<ActionResult> OnGet(string q, int query, string q2, int pageNumber)
        {
            PrepareForm(query);


            if (query == 0 && q != null)
            {
                ExpenseResponse = await _service.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.SpecificDate, BeginDate = DateTime.Parse(q), PageNumber = pageNumber });
                return Page();
            }
            if (query == 1 && q != null && q2 != null)
            {
                ExpenseResponse = await _service.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.DateRange, BeginDate = DateTime.Parse(q), EndDate = DateTime.Parse(q2), PageNumber = pageNumber });
                return Page();
            }
            if (query == 2 && q != null)
            {
                ExpenseResponse = await _service.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.SpecificMonthAndYear, BeginDate = DateTime.Parse(q), PageNumber = pageNumber });
                return Page();
            }
            if (query == 3 && q != null)
            {
                ExpenseResponse = await _service.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.SpecificQuarter, BeginDate = DateTime.Parse(q), PageNumber = pageNumber });
                return Page();
            }
            if (query == 4 && q != null)
            {
                int year;
                if (int.TryParse(q, out year))
                {
                    ExpenseResponse = await _service.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.SpecificYear, BeginDate = new DateTime(year, 1, 1), PageNumber = pageNumber });
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
                    ExpenseResponse = await _service.GetExpensesByAmountAsync(new AmountSearchRequest() { Query = amount, PageNumber = pageNumber });
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
                ExpenseResponse = await _service.GetExpensesByNotesAsync(new ExpenseRequest() { Query = q, PageNumber = pageNumber });
                return Page();
            }
            if (query == 7)
            {
                ExpenseResponse = await _service.GetExpensesByMerchantAsync(new ExpenseRequest() { Query= q, PageNumber = pageNumber });
                return Page();
            }
            if (query == 8)
            {
                ExpenseResponse = await _service.GetExpensesBySubCategoryIdAsync(new ExpenseRequest() { Query = q, PageNumber = pageNumber });
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
                ExpenseResponse = await _service.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.CurrentMonth, PageNumber = pageNumber });
                return Page();
            }
            if (query == 12)
            {
                ExpenseResponse = await _service.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.CurrentQuarter, PageNumber = pageNumber });
                return Page();
            }
            if (query == 13)
            {
                ExpenseResponse = await _service.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.CurrentYear, PageNumber = pageNumber });
                return Page();
            }
            if (query == 14)
            {
                ExpenseResponse = await _service.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.Last30Days, PageNumber = pageNumber });
                return Page();
            }

            ExpenseResponse = await _service.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.All, PageNumber = this.pageNumber });
            return Page();
        }
        public async Task<IActionResult> OnPostDelete(int expenseId, int query, string q, string q2, int pageNumber)
        {
            var success = await _service.DeleteExpenseAsync(expenseId);
            if (!success)
            {
                ModelState.AddModelError("", "Unable to delete the expense");
                return Page();
            }
            TempData["Message"] = "Sucessfully deleted expense!";
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
