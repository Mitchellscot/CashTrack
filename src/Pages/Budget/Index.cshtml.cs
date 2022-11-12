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
    public class IndexModel : PageModelBase
    {
        private readonly IBudgetService _budgetService;
        private readonly ISubCategoryService _subCategoryService;

        public IndexModel(IBudgetService budgetService, ISubCategoryService subCategoryService) => (_budgetService, _subCategoryService) = (budgetService, subCategoryService);
        public SubCategoryDropdownSelection[] CategoryList { get; set; }
        [BindProperty]
        public AddBudgetAllocationModal AddBudgetModal { get; set; }

        public Task<IActionResult> OnGetAsync()
        {
            return PrepareAndRenderPage();
        }
        public Task<IActionResult> OnPostAddBudgetModal()
        {
            var validation = ValidateAddBudgetModal(this.AddBudgetModal);
            if (!string.IsNullOrEmpty(validation))
            {
                ModelState.AddModelError("", validation);
                return PrepareAndRenderPage();
            }

            return PrepareAndRenderPage();
        }
        private async Task<IActionResult> PrepareAndRenderPage()
        {
            CategoryList = await _subCategoryService.GetSubCategoryDropdownListAsync();
            return Page();
        }
        private string ValidateAddBudgetModal(AddBudgetAllocationModal modal)
        {
            var validationMessage = "";
            if (modal.Amount < 1)
                validationMessage += "Amount Must be greater than zero. ";
            if (!modal.IsIncome && modal.SubCategoryId < 1)
                validationMessage += "You must supply a category if you are budgeting for an expense. ";
            return validationMessage;
        }
    }
}
