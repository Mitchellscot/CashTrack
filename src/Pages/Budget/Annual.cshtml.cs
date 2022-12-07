using CashTrack.Models.BudgetModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Services.BudgetService;
using CashTrack.Services.SubCategoryService;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using CashTrack.Pages.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;
using CashTrack.Models.MainCategoryModels;
using CashTrack.Services.MainCategoriesService;

namespace CashTrack.Pages.Budget
{
    public class AnnualModel : PageModelBase
    {
        private readonly IBudgetService _budgetService;
        private readonly ISubCategoryService _subCategoryService;
        private readonly IMainCategoriesService _mainCategoryService;

        public AnnualModel(IBudgetService budgetService, ISubCategoryService subCategoryService, IMainCategoriesService mainCategoryService) => (_budgetService, _subCategoryService, _mainCategoryService) = (budgetService, subCategoryService, mainCategoryService);
        public SubCategoryDropdownSelection[] CategoryList { get; set; }
        [BindProperty(SupportsGet = true)]
        public int Year { get; set; } = DateTime.Now.Year;
        public SelectList YearSelectList { get; set; }
        public SelectList MainCategoryList { get; set; }
        public AnnualBudgetPageResponse BudgetPageResponse { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            return await PrepareAndRenderPage();
        }
        private async Task<IActionResult> PrepareAndRenderPage()
        {
            CategoryList = await _subCategoryService.GetSubCategoryDropdownListAsync();
            MainCategoryList = new SelectList(await _mainCategoryService.GetMainCategoriesForDropdownListAsync(), "Id", "Category");
            BudgetPageResponse = await _budgetService.GetAnnualBudgetPageAsync(new AnnualBudgetPageRequest() { Year = Year });
            YearSelectList = new SelectList(await _budgetService.GetAnnualBudgetYears());
            return Page();
        }
    }
}
