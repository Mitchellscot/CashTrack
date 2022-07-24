using CashTrack.Pages.Shared;
using CashTrack.Services.ExpenseReviewService;
using CashTrack.Services.IncomeReviewService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CashTrack.Pages
{

    public class IndexModel : PageModelBase
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IExpenseReviewService _expenseReviewService;
        private readonly IIncomeReviewService _incomeReviewService;
        public int ReviewAmount { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IExpenseReviewService expenseReviewService, IIncomeReviewService incomeReviewService)
        {
            _logger = logger;
            _expenseReviewService = expenseReviewService;
            _incomeReviewService = incomeReviewService;
    }

        public async Task<ActionResult> OnGet()
        {
            var incomeReviews = await _incomeReviewService.GetCountOfIncomeReviews();
            var expenseReviews = await _expenseReviewService.GetCountOfExpenseReviews();
            if (expenseReviews > 0 || incomeReviews > 0) 
            {
                ReviewAmount = incomeReviews + expenseReviews;
            }

            return Page();
        }
    }
}