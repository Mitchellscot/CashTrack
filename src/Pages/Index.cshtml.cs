using CashTrack.Models.SummaryModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.BudgetService;
using CashTrack.Services.ExpenseReviewService;
using CashTrack.Services.IncomeReviewService;
using CashTrack.Services.SummaryService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CashTrack.Pages
{

    public class IndexModel : PageModelBase
    {
        private readonly ISummaryService _summaryService;
        private readonly IBudgetService _budgetService;
        private readonly IExpenseReviewService _expenseReviewService;
        private readonly IIncomeReviewService _incomeReviewService;
        public int ReviewAmount { get; set; }
        [BindProperty(SupportsGet = true)]
        public int Year { get; set; } = DateTime.Now.Year;
        [BindProperty(SupportsGet = true)]
        public int Month { get; set; } = DateTime.Now.Month;
        public SelectList YearSelectList { get; set; }
        public SelectList MonthList { get; set; } = new SelectList(new Dictionary<string, int> {
            { "January", 1 },
            { "February", 2 },
            { "March", 3 },
            { "April", 4 },
            { "May", 5 },
            { "June", 6 },
            { "July", 7 },
            { "August", 8 },
            { "September", 9 },
            { "October", 10 },
            { "November", 11 },
            { "December", 12 }
        }, "Value", "Key", DateTime.Now.Month);
        public MonthlySummaryResponse SummaryResponse { get; set; }
        public IndexModel(IBudgetService budgetService, IExpenseReviewService expenseReviewService, IIncomeReviewService incomeReviewService, ISummaryService summaryService)
        {
            _summaryService = summaryService;
            _budgetService = budgetService;
            _expenseReviewService = expenseReviewService;
            _incomeReviewService = incomeReviewService;
        }

        public async Task<IActionResult> OnGet()
        {
            var incomeReviews = await _incomeReviewService.GetCountOfIncomeReviews();
            var expenseReviews = await _expenseReviewService.GetCountOfExpenseReviews();
            if (expenseReviews > 0 || incomeReviews > 0)
            {
                ReviewAmount = incomeReviews + expenseReviews;
            }

            return await PrepareAndRenderPage();
        }
        private async Task<IActionResult> PrepareAndRenderPage()
        {
            //CategoryList = await _subCategoryService.GetSubCategoryDropdownListAsync();
            //MainCategoryList = new SelectList(await _mainCategoryService.GetMainCategoriesForDropdownListAsync(), "Id", "Category");
            SummaryResponse = await _summaryService.GetMonthlySummaryAsync(new MonthlySummaryRequest() { Year = this.Year, Month = this.Month });
            YearSelectList = new SelectList(await _budgetService.GetAnnualBudgetYearsAsync());
            return Page();
        }
    }
}