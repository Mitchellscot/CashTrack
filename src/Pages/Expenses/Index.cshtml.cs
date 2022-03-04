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
        public QueryOptions query { get; set; }
        [BindProperty]
        public ExpenseResponse ExpenseResponse { get; set; }


        public async Task<ActionResult> OnGet(string q, QueryOptions query)
        {
            if (query == QueryOptions.Date && q != null)
            {
                if (!DateTime.TryParse(q, out _))
                {
                    ModelState.AddModelError("", "Date is not formatted properly");
                    return Page();

                }
                ExpenseResponse = await _service.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.SpecificDate, BeginDate = DateTime.Parse(q) });
                return Page();
            }

            ExpenseResponse = await _service.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.All });
            return Page();
        }
    }
}
