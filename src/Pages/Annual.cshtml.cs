using CashTrack.Models.SummaryModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.BudgetService;
using CashTrack.Services.SummaryService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CashTrack.Pages
{
    public class Annual : PageModelBase
    {
        private readonly ISummaryService _summaryService;
        private readonly IBudgetService _budgetService;

        [BindProperty(SupportsGet = true)]
        public int Year { get; set; } = DateTime.Now.Year;
        public AnnualSummaryResponse SummaryResponse { get; set; }
        public SelectList YearSelectList { get; set; }
        public Annual(IBudgetService budgetService, ISummaryService summaryService)
        {
            _summaryService = summaryService;
            _budgetService = budgetService;
        }
        public async Task<IActionResult> OnGet()
        {
            return await PrepareAndRenderPage();
        }

        private async Task<IActionResult> PrepareAndRenderPage()
        {
            SummaryResponse = await _summaryService.GetAnnualSummaryAsync(new AnnualSummaryRequest() { Year = this.Year, UserId = int.Parse(this.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value) });
            YearSelectList = new SelectList(await _budgetService.GetAnnualBudgetYearsAsync());
            return Page();
        }
    }
}
