using CashTrack.Models.Common;
using CashTrack.Models.ExpenseModels;
using CashTrack.Services.ExpenseService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;

namespace CashTrack.Pages.Expenses
{
    public class Index : PageModel
    {
        private readonly IExpenseService _service;

        public Index(IExpenseService service) => _service = service;

        [BindProperty]
        public string q { get; set; }
        [BindProperty]
        public string q2 { get; set; }
        [BindProperty]
        public QueryOptions query { get; set; }
        [BindProperty]
        public ExpenseResponse ExpenseResponse { get; set; }

        public async Task<ActionResult> OnGet(string q, QueryOptions query, string q2)
        {
            if (query == QueryOptions.Date && q != null)
            {
                ExpenseResponse = await _service.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.SpecificDate, BeginDate = DateTime.Parse(q) });
                return Page();
            }
            if (query == QueryOptions.DateRange && q != null && q2 != null)
            {
                ExpenseResponse = await _service.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.DateRange, BeginDate = DateTime.Parse(q), EndDate = DateTime.Parse(q2) });
                return Page();
            }
            if (query == QueryOptions.Month && q != null)
            {
                ExpenseResponse = await _service.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.SpecificMonthAndYear, BeginDate = DateTime.Parse(q) });
                return Page();
            }
            if (query == QueryOptions.Quarter && q != null)
            {
                ExpenseResponse = await _service.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.SpecificQuarter, BeginDate = DateTime.Parse(q) });
                return Page();
            }
            if (query == QueryOptions.Year && q != null)
            {
                ExpenseResponse = await _service.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.SpecificYear, BeginDate = DateTime.Parse(q) });
                return Page();
            }
            if (query == QueryOptions.CurrentMonth)
            {
                ExpenseResponse = await _service.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.CurrentMonth });
                return Page();
            }
            if (query == QueryOptions.CurrentQuarter)
            {
                ExpenseResponse = await _service.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.CurrentQuarter });
                return Page();
            }
            if (query == QueryOptions.CurrentYear)
            {
                ExpenseResponse = await _service.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.CurrentYear });
                return Page();
            }
            if (query == QueryOptions.Last30Days)
            {
                ExpenseResponse = await _service.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.Last30Days });
                return Page();
            }
            if (query == QueryOptions.Amount)
            {
                decimal amount;
                if (Decimal.TryParse(q, out amount))
                {
                    ExpenseResponse = await _service.GetExpensesByAmountAsync(new AmountSearchRequest() { Query = amount });
                    return Page();
                }
                else
                {
                    ModelState.AddModelError("", "Unable to convert the given amount. Please try again.");
                    return Page();
                }
            }
            if (query == QueryOptions.Notes)
            {
                ExpenseResponse = await _service.GetExpensesByNotesAsync(new ExpenseRequest() { Query = q });
                return Page();
            }
            if (query == QueryOptions.Merchant)
            {
                ModelState.AddModelError("", "Not Implemented Yet");
                return Page();
            }
            if (query == QueryOptions.SubCategory)
            {
                ModelState.AddModelError("", "Not Implemented Yet");
                return Page();
            }
            if (query == QueryOptions.MainCategory)
            {
                ModelState.AddModelError("", "Not Implemented Yet");
                return Page();
            }
            if (query == QueryOptions.Tag)
            {
                ModelState.AddModelError("", "Not Implemented Yet");
                return Page();
            }

            ExpenseResponse = await _service.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.All });
            return Page();
        }
    }
}
