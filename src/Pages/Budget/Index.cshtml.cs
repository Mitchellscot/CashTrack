using CashTrack.Models.BudgetModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.BudgetService;
using CashTrack.Services.SubCategoryService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CashTrack.Pages.Budget
{
    public class IndexModel : PageModelBase
    {
        private readonly IBudgetService _budgetService;
        private readonly ISubCategoryService _subCategoryService;

        public IndexModel(IBudgetService budgetService, ISubCategoryService subCategoryService) => (_budgetService, _subCategoryService) = (budgetService, subCategoryService);
        public SubCategoryDropdownSelection[] CategoryList { get; set; }
        [BindProperty]
        public AddBudgetAllocationModal AddBudgetModal { get; set; }
        public BudgetPageResponse BudgetPageResponse { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            return await PrepareAndRenderPage();
        }
        public async Task<IActionResult> OnPostAddBudgetModal()
        {
            try
            {
                var success = await _budgetService.CreateBudgetItem(this.AddBudgetModal);
                if (success > 0)
                {
                    TempData["SuccessMessage"] = "Successfully Budgeted!";
                    return RedirectToPage("./Index");
                }
                else
                {
                    TempData["InfoMessage"] = "Budget was not saved.";
                    return RedirectToPage("./Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return await PrepareAndRenderPage();
            }
        }
        public async Task<IActionResult> OnPostDelete(int budgetId)
        {
            var success = await _budgetService.DeleteBudgetAsync(budgetId);
            if (!success)
            {
                ModelState.AddModelError("", "Unable to delete the budget");
                return await PrepareAndRenderPage();
            }
            TempData["SuccessMessage"] = "Successfully deleted the budget!";
            return RedirectToPage("./Index");
        }
        private async Task<IActionResult> PrepareAndRenderPage()
        {
            CategoryList = await _subCategoryService.GetSubCategoryDropdownListAsync();
            BudgetPageResponse = await _budgetService.GetBudgetPageAsync(new BudgetPageRequest());
            return Page();
        }
    }
}