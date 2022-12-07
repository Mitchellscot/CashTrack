using CashTrack.Models.BudgetModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.BudgetService;
using CashTrack.Services.MainCategoriesService;
using CashTrack.Services.SubCategoryService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CashTrack.Pages.Budget
{
    public class MonthlyModel : PageModelBase
    {
        private readonly IBudgetService _budgetService;
        private readonly ISubCategoryService _subCategoryService;
        private readonly IMainCategoriesService _mainCategoryService;

        public MonthlyModel(IBudgetService budgetService, ISubCategoryService subCategoryService, IMainCategoriesService mainCategoryService) => (_budgetService, _subCategoryService, _mainCategoryService) = (budgetService, subCategoryService, mainCategoryService);

        [BindProperty(SupportsGet = true)]
        public int Year { get; set; } = DateTime.Now.Year;
        [BindProperty(SupportsGet = true)]
        public int Month { get; set; } = DateTime.Now.Month;
        public MonthlyBudgetPageResponse BudgetPageResponse { get; set; }
        public SubCategoryDropdownSelection[] CategoryList { get; set; }
        public SelectList YearSelectList { get; set; }
        public SelectList MainCategoryList { get; set; }
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

        public async Task<IActionResult> OnGetAsync()
        {
            return await PrepareAndRenderPage();
        }
        private async Task<IActionResult> PrepareAndRenderPage()
        {
            CategoryList = await _subCategoryService.GetSubCategoryDropdownListAsync();
            BudgetPageResponse = await _budgetService.GetMonthlyBudgetPageAsync(new MonthlyBudgetPageRequest() { Year = Year, Month = Month });
            MainCategoryList = new SelectList(await _mainCategoryService.GetMainCategoriesForDropdownListAsync(), "Id", "Category");
            YearSelectList = new SelectList(await _budgetService.GetAnnualBudgetYearsAsync());
            return Page();
        }
    }
}
