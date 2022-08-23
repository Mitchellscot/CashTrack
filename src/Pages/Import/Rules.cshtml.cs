using CashTrack.Models.Common;
using CashTrack.Models.ExpenseModels;
using CashTrack.Models.ImportRuleModels;
using CashTrack.Models.IncomeCategoryModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Pages.Shared;
using CashTrack.Repositories.ImportRuleRepository;
using CashTrack.Repositories.IncomeCategoryRepository;
using CashTrack.Repositories.IncomeSourceRepository;
using CashTrack.Repositories.MerchantRepository;
using CashTrack.Repositories.SubCategoriesRepository;
using CashTrack.Services.ImportRulesService;
using CashTrack.Services.IncomeCategoryService;
using CashTrack.Services.IncomeSourceService;
using CashTrack.Services.MerchantService;
using CashTrack.Services.SubCategoryService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Threading.Tasks;

namespace CashTrack.Pages.Import
{
    public class RulesModel : PageModelBase
    {
        private readonly IImportRulesService _service;
        private readonly IMerchantService _merchantService;
        private readonly IIncomeSourceService _sourceService;
        private readonly ISubCategoryService _subCategoryService;
        private readonly IIncomeCategoryService _incomeCategoryService;

        public RulesModel(IImportRulesService service, IMerchantService merchantService, IIncomeSourceService sourceService, ISubCategoryService subCategoryService, IIncomeCategoryService incomeCategoryService)
        {
            _service = service;
            _merchantService = merchantService;
            _sourceService = sourceService;
            _subCategoryService = subCategoryService;
            _incomeCategoryService = incomeCategoryService;

        }
        public SubCategoryDropdownSelection[] SubCategoryList { get; set; }
        public IncomeCategoryDropdownSelection[] IncomeCategoryList { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        public  ImportRuleResponse RuleRespose { get; set; }
        [BindProperty]
        public AddEditImportRuleModal ImportRule { get; set; }
        public async Task<IActionResult> OnGet()
        {
            RuleRespose = await _service.GetImportRulesAsync(new ImportRuleRequest() { PageNumber = this.PageNumber});
            return await PrepareAndRenderPage();
        }
        public async Task<IActionResult> OnPostAddEditImportRuleModal()
        {

            return await PrepareAndRenderPage();
        }
        private async Task<IActionResult> PrepareAndRenderPage()
        {
            PageNumber = RuleRespose != null ? RuleRespose.PageNumber : PageNumber == 0 ? 1 : PageNumber;
            SubCategoryList = await _subCategoryService.GetSubCategoryDropdownListAsync();
            IncomeCategoryList = await _incomeCategoryService.GetIncomeCategoryDropdownListAsync();

            return Page();
        }
    }
}
