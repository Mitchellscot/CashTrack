using CashTrack.Common.Extensions;
using CashTrack.Models.IncomeCategoryModels;
using CashTrack.Models.IncomeReviewModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.ImportProfileService;
using CashTrack.Services.IncomeCategoryService;
using CashTrack.Services.IncomeReviewService;
using CashTrack.Services.IncomeService;
using CashTrack.Services.IncomeSourceService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashTrack.Pages.Import
{
    public class IncomeModel : PageModelBase
    {
        private readonly IIncomeReviewService _incomeReviewService;
        private readonly IIncomeSourceService _incomeSourceService;
        private readonly IIncomeCategoryService _incomeCategoryService;
        private readonly IIncomeService _incomeService;
        private readonly IImportProfileService _profileService;

        public IncomeModel(IIncomeReviewService incomeReviewService, IIncomeSourceService incomeSourceService, IIncomeCategoryService incomeCategoryService, IIncomeService incomeService, IImportProfileService profileService)
        {
            _incomeReviewService = incomeReviewService;
            _incomeSourceService = incomeSourceService;
            _incomeCategoryService = incomeCategoryService;
            _incomeService = incomeService;
            _profileService = profileService;
        }
        public IncomeReviewResponse IncomeReviewResponse { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        public IncomeCategoryDropdownSelection[] CategoryList { get; set; }
        [BindProperty]
        public CashTrack.Models.IncomeModels.Income SelectedIncome { get; set; }
        [BindProperty]
        public int SelectedIncomeId { get; set; }
        [BindProperty]
        public bool IsRefund { get; set; }
        public List<string> FileTypes { get; set; }
        public async Task<IActionResult> OnGet()
        {
            await PrepareData();
            return Page();
        }
        public async Task<IActionResult> OnPostAddIncome()
        {
            if (SelectedIncome.CategoryId == 0)
            {
                ModelState.AddModelError("", "Income must have an assigned category");
                await PrepareData();
                return Page();
            }
            if (!string.IsNullOrEmpty(SelectedIncome.Source) &&
                !(await _incomeSourceService.GetAllIncomeSourceNames()).Any(x => x == SelectedIncome.Source))
            {
                ModelState.AddModelError("", "That income Source does not exist.");
                await PrepareData();
                return Page();
            }
            var incomeId = 0;
            try
            {
                incomeId = await _incomeService.CreateIncomeAsync(SelectedIncome);
                var incomeReviewId = await _incomeReviewService.SetIncomeReviewToIgnoreAsync(SelectedIncomeId);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                await PrepareData();
                return Page();
            }
            if (IsRefund || (SelectedIncome.Category != null && SelectedIncome.Category.IsEqualTo("Refund")))
                return RedirectToPage("../Income/Refund", new { id = incomeId });

            SuccessMessage = "Sucessfully Added New Income!";
            return RedirectToPage("../Import/Income", new { pageNumber = PageNumber });
        }
        public async Task<IActionResult> OnPostRemoveIncome(int pageNumber)
        {
            try
            {
                var deleteSuccess = await _incomeReviewService.SetIncomeReviewToIgnoreAsync(SelectedIncomeId);
            }
            catch (Exception ex)
            {
                InfoMessage = ex.Message;
                return Page();
            }
            SuccessMessage = "Successfully Removed the Income!";
            return RedirectToPage("../Import/Income", new { pageNumber = pageNumber });
        }
        private async Task PrepareData()
        {
            CategoryList = await _incomeCategoryService.GetIncomeCategoryDropdownListAsync();
            PageNumber = IncomeReviewResponse != null ? IncomeReviewResponse.PageNumber : PageNumber == 0 ? 1 : PageNumber;
            FileTypes = await _profileService.GetImportProfileNames();
            IncomeReviewResponse = await _incomeReviewService.GetIncomeReviewsAsync(new IncomeReviewRequest() { PageNumber = PageNumber });
        }
    }
}
