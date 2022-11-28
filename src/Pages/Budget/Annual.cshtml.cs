using CashTrack.Models.BudgetModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Services.BudgetService;
using CashTrack.Services.SubCategoryService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        [BindProperty]
        public AddBudgetAllocationModal AddBudgetModal { get; set; }
        public AnnualBudgetPageResponse BudgetPageResponse { get; set; }

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
                    return RedirectToPage("./Annual");
                }
                else
                {
                    TempData["InfoMessage"] = "Budget was not saved.";
                    return RedirectToPage("./Annual");
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
            return RedirectToPage("./Annual");
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
