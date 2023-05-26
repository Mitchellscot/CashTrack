using CashTrack.Models.IncomeCategoryModels;
using CashTrack.Models.MainCategoryModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Models.SummaryModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.ExpenseService;
using CashTrack.Services.IncomeCategoryService;
using CashTrack.Services.MainCategoriesService;
using CashTrack.Services.SubCategoryService;
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
        private readonly IExpenseService _expenseService;
        private readonly ISubCategoryService _subCategoryService;
        private readonly IMainCategoriesService _mainCategoryService;
        private readonly IIncomeCategoryService _incomeCategoryService;

        [BindProperty(SupportsGet = true)]
        public int Year { get; set; } = DateTime.Now.Year;
        public AnnualSummaryResponse SummaryResponse { get; set; }
        public SelectList YearSelectList { get; set; }
        public SubCategoryDropdownSelection[] SubCategoryList { get; set; }
        public MainCategoryDropdownSelection[] MainCategoryList { get; set; }
        public IncomeCategoryDropdownSelection[] IncomeCategoryList { get; set; }
        public Annual(IExpenseService expenseService, ISummaryService summaryService, ISubCategoryService subCategoryService, IMainCategoriesService mainCategoriesService, IIncomeCategoryService incomeCategoryService)
        {
            _summaryService = summaryService;
            _expenseService = expenseService;
            _subCategoryService = subCategoryService;
            _mainCategoryService = mainCategoriesService;
            _incomeCategoryService = incomeCategoryService;
        }
        public async Task<IActionResult> OnGet()
        {
            return await PrepareAndRenderPage();
        }

        private async Task<IActionResult> PrepareAndRenderPage()
        {
            int id = 1;
            var value = this.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            if (value != null && value.Value is string)
                id = Convert.ToInt32(value.Value);
            IncomeCategoryList = await _incomeCategoryService.GetIncomeCategoryDropdownListAsync();
            SubCategoryList = await _subCategoryService.GetSubCategoryDropdownListAsync();
            MainCategoryList = await _mainCategoryService.GetMainCategoriesForDropdownListAsync();
            SummaryResponse = await _summaryService.GetAnnualSummaryAsync(new AnnualSummaryRequest() 
            { Year = this.Year, UserId = id });
            YearSelectList = new SelectList(await _expenseService.GetAnnualSummaryYearsAsync());
            return Page();
        }
    }
}
