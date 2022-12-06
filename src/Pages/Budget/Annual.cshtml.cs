using CashTrack.Models.BudgetModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Services.BudgetService;
using CashTrack.Services.SubCategoryService;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using CashTrack.Pages.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CashTrack.Pages.Budget
{
    public class AnnualModel : PageModelBase
    {
        private readonly IBudgetService _budgetService;
        private readonly ISubCategoryService _subCategoryService;

        public AnnualModel(IBudgetService budgetService, ISubCategoryService subCategoryService) => (_budgetService, _subCategoryService) = (budgetService, subCategoryService);
        public SubCategoryDropdownSelection[] CategoryList { get; set; }
        [BindProperty(SupportsGet = true)]
        public int Year { get; set; } = DateTime.Now.Year;
        public SelectList YearSelectList { get; set; }
        public AnnualBudgetPageResponse BudgetPageResponse { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            return await PrepareAndRenderPage();
        }
        private async Task<IActionResult> PrepareAndRenderPage()
        {
            CategoryList = await _subCategoryService.GetSubCategoryDropdownListAsync();
            BudgetPageResponse = await _budgetService.GetAnnualBudgetPageAsync(new AnnualBudgetPageRequest() { Year = Year });
            YearSelectList = new SelectList(await _budgetService.GetAnnualBudgetYears());
            return Page();
        }
    }
}
