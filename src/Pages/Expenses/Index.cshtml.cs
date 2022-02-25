using CashTrack.Models.Common;
using CashTrack.Models.ExpenseModels;
using CashTrack.Services.ExpenseService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace CashTrack.Pages.Expenses
{
    public class Index : PageModel
    {
        private readonly IExpenseService _service;

        public Index(IExpenseService service) => _service = service;

        [BindProperty]
        public DateOptions QueryType { get; set; }
        [BindProperty]
        public ExpenseRequest ExpenseRequest { get; set; }
        public ExpenseResponse ExpenseResponse { get; set; }

        public async Task<ActionResult> OnGet()
        {
            ExpenseResponse = await _service.GetExpensesAsync(new ExpenseRequest() { DateOptions = DateOptions.All });
            return Page();
        }
        public async Task<IActionResult> OnGetPresetQuery(DateOptions options)
        {
            var request = await _service.GetExpensesAsync(new ExpenseRequest() { DateOptions = options });
            return Page();
        }
    }
}
