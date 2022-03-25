using CashTrack.Models.ExpenseReviewModels;
using CashTrack.Services.ExpenseReviewService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace CashTrack.Pages.Review
{
    public class ExpensesModel : PageModel
    {
        private readonly IExpenseReviewService _expenseService;

        public ExpensesModel(IExpenseReviewService expenseService)
        {
            _expenseService = expenseService;
        }
        public ExpenseReviewResponse ExpenseReviewResponse { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        public async Task<IActionResult> OnGet(int pageNumber)
        {
            PageNumber = ExpenseReviewResponse != null ? ExpenseReviewResponse.PageNumber : pageNumber == 0 ? 1 : pageNumber;
     
            ExpenseReviewResponse = await _expenseService.GetExpenseReviewsAsync(new ExpenseReviewRequest() { PageNumber = PageNumber });
            return Page();
        }
    }
}