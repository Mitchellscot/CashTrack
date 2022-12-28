using CashTrack.Models.IncomeCategoryModels;
using CashTrack.Models.MainCategoryModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Models.SummaryModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.IncomeCategoryService;
using CashTrack.Services.MainCategoriesService;
using CashTrack.Services.SubCategoryService;
using CashTrack.Services.SummaryService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace CashTrack.Pages
{
    public class SummaryModel : PageModelBase
    {
        private ISummaryService _summaryService;
        private readonly ISubCategoryService _subCategoryService;
        private readonly IMainCategoriesService _mainCategoryService;
        private readonly IIncomeCategoryService _incomeCategoryService;
        public AllTimeSummaryResponse SummaryResponse { get; set; }
        public SubCategoryDropdownSelection[] SubCategoryList { get; set; }
        public MainCategoryDropdownSelection[] MainCategoryList { get; set; }
        public IncomeCategoryDropdownSelection[] IncomeCategoryList { get; set; }

        public SummaryModel(ISummaryService summaryService, ISubCategoryService subCategoryService, IMainCategoriesService mainCategoryService, IIncomeCategoryService incomeCategoryService)
        {
            _summaryService = summaryService;
            _subCategoryService = subCategoryService;
            _mainCategoryService = mainCategoryService;
            _incomeCategoryService = incomeCategoryService;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            IncomeCategoryList = await _incomeCategoryService.GetIncomeCategoryDropdownListAsync();
            SubCategoryList = await _subCategoryService.GetSubCategoryDropdownListAsync();
            MainCategoryList = await _mainCategoryService.GetMainCategoriesForDropdownListAsync();
            SummaryResponse = await _summaryService.GetAllTimeSummaryAsync();
            return Page();
        }
    }
}
