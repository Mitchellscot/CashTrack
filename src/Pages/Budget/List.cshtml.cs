using CashTrack.Models.BudgetModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.BudgetService;
using CashTrack.Services.MainCategoriesService;
using CashTrack.Services.SubCategoryService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;

namespace CashTrack.Pages.Budget
{

    public class ListModel : PageModelBase
    {
        private readonly IBudgetService _budgetService;
        private readonly ISubCategoryService _subCategoryService;
        private readonly IMainCategoriesService _mainCategoryService;

        public SubCategoryDropdownSelection[] CategoryList { get; set; }
        [BindProperty]
        public AddEditBudgetAllocationModal BudgetModal { get; set; }
        public BudgetListResponse BudgetListResponse { get; set; }
        [BindProperty(SupportsGet = true)]
        public BudgetOrderBy Query { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public bool Q2 { get; set; }
        [BindProperty]
        public int BudgetId { get; set; }
        [BindProperty(SupportsGet =true)]
        public bool Q3 { get; set; }
        public SelectList MainCategoryList { get; set; }
        public ListModel(IBudgetService budgetService, ISubCategoryService subCategoryService, IMainCategoriesService mainCategoryService) => (_budgetService, _subCategoryService, _mainCategoryService) = (budgetService, subCategoryService, mainCategoryService);

        public async Task<IActionResult> OnGetAsync()
        {
            return await PrepareAndRenderPage();
        }
        public async Task<IActionResult> OnPostAddBudgetModal()
        {
            try
            {
                var success = await _budgetService.CreateBudgetItemAsync(this.BudgetModal);
                if (success > 0)
                {
                    SuccessMessage = "Successfully Budgeted!";
                    return LocalRedirect(Url.Content(BudgetModal.ReturnUrl));
                }
                else
                {
                    InfoMessage = "Budget was not saved.";
                    return LocalRedirect(Url.Content(BudgetModal.ReturnUrl));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return await PrepareAndRenderPage();
            }
        }
        public async Task<IActionResult> OnPostEditBudgetModal()
        {
            try
            {
                var success = await _budgetService.UpdateBudgetAsync(this.BudgetModal);
                if (success > 0)
                {
                    SuccessMessage = "Successfully Edited Budget!";
                    return LocalRedirect(Url.Content(BudgetModal.ReturnUrl));
                }
                else
                {
                    InfoMessage = "Budget was not saved.";
                    return LocalRedirect(Url.Content(BudgetModal.ReturnUrl));
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
            SuccessMessage = "Successfully deleted the budget!";
            return RedirectToPage("./List", new { PageNumber = this.PageNumber, Query = this.Query, Q2 = this.Q2 });
        }
        private async Task<IActionResult> PrepareAndRenderPage()
        {
            CategoryList = await _subCategoryService.GetSubCategoryDropdownListAsync();
            BudgetListResponse = await _budgetService.GetBudgetListAsync(new BudgetListRequest()
            {
                CurrentYearOnly = this.Q3,
                Reversed = Q2,
                Order = Query,
                PageNumber = this.PageNumber
            });
            MainCategoryList = new SelectList(await _mainCategoryService.GetMainCategoriesForDropdownListAsync(), "Id", "Category");
            return Page();
        }
    }
}
