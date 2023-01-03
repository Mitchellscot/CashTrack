using CashTrack.Models.Common;
using CashTrack.Models.ImportRuleModels;
using CashTrack.Models.IncomeCategoryModels;
using CashTrack.Models.IncomeSourceModels;
using CashTrack.Models.MerchantModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.ImportRulesService;
using CashTrack.Services.IncomeCategoryService;
using CashTrack.Services.IncomeSourceService;
using CashTrack.Services.MerchantService;
using CashTrack.Services.SubCategoryService;
using Microsoft.AspNetCore.Mvc;
using System;
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
        public MerchantDropdownSelection[] MerchantList { get; set; }
        public SourceDropdownSelection[] SourceList { get; set; }
        [BindProperty(SupportsGet = true)]
        public ImportRuleOrderBy Query { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public bool Q2 { get; set; }
        public ImportRuleResponse RuleRespose { get; set; }
        [BindProperty]
        public AddEditImportRuleModal ImportRule { get; set; }

        public async Task<IActionResult> OnGet()
        {
            return await PrepareAndRenderPage();
        }
        public async Task<IActionResult> OnPostAddEditImportRuleModal()
        {
            if (ImportRule.RuleType == RuleType.Assignment &&
                ImportRule.MerchantSourceId == null &&
                ImportRule.CategoryId == null)
            {
                ModelState.AddModelError("", "Assignment rules must have either a Category or Merchant/Source assigned.");
                return await PrepareAndRenderPage();
            }
            try
            {
                var success = ImportRule.IsEdit ? await _service.UpdateImportRuleAsync(ImportRule) : await _service.CreateImportRuleAsync(ImportRule);

                if (success > 0)
                {
                    TempData["SuccessMessage"] = ImportRule.IsEdit ? "Successfully edited an Import Rule!" : "Successfully added a new Import Rule!";
                    return RedirectToPage("./Rules", new { query = Query, q2 = Q2, pageNumber = PageNumber });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return await PrepareAndRenderPage();
            }
            return LocalRedirect(ImportRule.Returnurl);
        }
        public async Task<IActionResult> OnPostDelete(int ruleId, int Query, string q2, int pageNumber)
        {
            var success = await _service.DeleteImportRuleAsync(ruleId);
            if (!success)
            {
                ModelState.AddModelError("", "Unable to delete the import rule");
                return await PrepareAndRenderPage();
            }
            TempData["SuccessMessage"] = "Sucessfully deleted Import Rule!";
            return RedirectToPage("./Rules", new { query = Query, q2 = q2, pageNumber = pageNumber });
        }
        private async Task<IActionResult> PrepareAndRenderPage()
        {
            RuleRespose = await _service.GetImportRulesAsync(
                new ImportRuleRequest()
                {
                    PageNumber = this.PageNumber,
                    OrderBy = this.Query,
                    Reversed = this.Q2
                });
            PageNumber = RuleRespose != null ? RuleRespose.PageNumber : PageNumber == 0 ? 1 : PageNumber;
            SubCategoryList = await _subCategoryService.GetSubCategoryDropdownListAsync();
            IncomeCategoryList = await _incomeCategoryService.GetIncomeCategoryDropdownListAsync();
            MerchantList = await _merchantService.GetMerchantDropdownListAsync();
            SourceList = await _sourceService.GetSourceDropdownListAsync();

            return Page();
        }
    }
}
