using CashTrack.Models.IncomeReviewModels;
using CashTrack.Services.IncomeReviewService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace CashTrack.Pages.Review
{
    public class IncomeModel : PageModel
    {
        private readonly IIncomeReviewService _incomeService;

        public IncomeModel(IIncomeReviewService incomeService) => _incomeService = incomeService;
        [BindProperty]
        public int PageNumber { get; set; }
        public IncomeReviewResponse IncomeReviewResponse { get; set; }
        public async Task<IActionResult> OnGet(int pageNumber)
        {
            PageNumber = IncomeReviewResponse != null ? IncomeReviewResponse.PageNumber : pageNumber == 0 ? 1 : pageNumber;

            IncomeReviewResponse = await _incomeService.GetIncomeReviewsAsync(new IncomeReviewRequest() { PageNumber = PageNumber });
            return Page();
        }
    }
}
