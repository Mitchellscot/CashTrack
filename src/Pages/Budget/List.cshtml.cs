using CashTrack.Models.BudgetModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.BudgetService;
using CashTrack.Services.SubCategoryService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;

namespace CashTrack.Pages.Budget
{

    public class ListModel : PageModelBase
    {
        private readonly IBudgetService _budgetService;
        private readonly ISubCategoryService _subCategoryService;

        public SubCategoryDropdownSelection[] CategoryList { get; set; }
        [BindProperty]
        public AddBudgetAllocationModal AddBudgetModal { get; set; }
        public BudgetListResponse BudgetListResponse { get; set; }
        [BindProperty(SupportsGet = true)]
        public BudgetOrderBy Query { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public bool Q2 { get; set; }
        [BindProperty]
        public int BudgetId { get; set; }
        public ListModel(IBudgetService budgetService, ISubCategoryService subCategoryService) => (_budgetService, _subCategoryService) = (budgetService, subCategoryService);

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
                    return RedirectToPage(this.AddBudgetModal.ReturnUrl);
                }
                else
                {
                    TempData["InfoMessage"] = "Budget was not saved.";
                    return RedirectToPage(this.AddBudgetModal.ReturnUrl);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return RedirectToPage(this.AddBudgetModal.ReturnUrl, new { PageNumber = this.AddBudgetModal.PageNumber, Query = this.AddBudgetModal.Query, Q2 = this.AddBudgetModal.Q2 });
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
            return RedirectToPage("./List", new { PageNumber = this.PageNumber, Query = this.Query, Q2 = this.Q2 });
        }
        private async Task<IActionResult> PrepareAndRenderPage()
        {
            CategoryList = await _subCategoryService.GetSubCategoryDropdownListAsync();
            BudgetListResponse = await _budgetService.GetBudgetList(new BudgetListRequest()
            {
                Reversed = Q2,
                Order = Query,
                PageNumber = this.PageNumber
            });
            return Page();
        }
    }
}
