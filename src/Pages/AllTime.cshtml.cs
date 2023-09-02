using CashTrack.Models.IncomeCategoryModels;
using CashTrack.Models.MainCategoryModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Models.SummaryModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.ImportProfileService;
using CashTrack.Services.IncomeCategoryService;
using CashTrack.Services.MainCategoriesService;
using CashTrack.Services.SubCategoryService;
using CashTrack.Services.SummaryService;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CashTrack.Pages
{
    public class AllTimeModel : PageModelBase
    {
        private ISummaryService _summaryService;
        private readonly ISubCategoryService _subCategoryService;
        private readonly IMainCategoriesService _mainCategoryService;
        private readonly IIncomeCategoryService _incomeCategoryService;
        private readonly IImportProfileService _profileService;

        public AllTimeSummaryResponse SummaryResponse { get; set; }
        public SubCategoryDropdownSelection[] SubCategoryList { get; set; }
        public MainCategoryDropdownSelection[] MainCategoryList { get; set; }
        public IncomeCategoryDropdownSelection[] IncomeCategoryList { get; set; }
        public List<string> FileTypes { get; set; }

        public AllTimeModel(ISummaryService summaryService, ISubCategoryService subCategoryService, IMainCategoriesService mainCategoryService, IIncomeCategoryService incomeCategoryService, IImportProfileService profileService)
        {
            _summaryService = summaryService;
            _subCategoryService = subCategoryService;
            _mainCategoryService = mainCategoryService;
            _incomeCategoryService = incomeCategoryService;
            _profileService = profileService;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            IncomeCategoryList = await _incomeCategoryService.GetIncomeCategoryDropdownListAsync();
            SubCategoryList = await _subCategoryService.GetSubCategoryDropdownListAsync();
            MainCategoryList = await _mainCategoryService.GetMainCategoriesForDropdownListAsync();
            SummaryResponse = await _summaryService.GetAllTimeSummaryAsync();
            FileTypes = await _profileService.GetImportProfileNames();
            return Page();
        }
    }
}
