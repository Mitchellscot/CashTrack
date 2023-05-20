using CashTrack.Common.Exceptions;
using CashTrack.Models.IncomeCategoryModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.IncomeCategoryService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;

namespace CashTrack.Pages.Income.Categories
{

    public class DetailModel : PageModelBase
    {
        private readonly IIncomeCategoryService _incomeCategoryService;

        public DetailModel(IIncomeCategoryService categoryService) => (_incomeCategoryService) = (categoryService);
        [BindProperty(SupportsGet = true)]
        public int id { get; set; }
        public IncomeCategoryDetail IncomeCategory { get; set; }
        public SelectList IncomeCategoryList { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }
        public async Task<IActionResult> OnGet()
        {
            if (SearchTerm != null)
            {
                try
                {
                    var categoryId = (await _incomeCategoryService.GetIncomeCategoryByNameAsync(SearchTerm)).Id;
                    return RedirectToPage("./Detail", new { id = categoryId });
                }
                catch (CategoryNotFoundException)
                {
                    InfoMessage = "No Categories found with the name " + SearchTerm;
                    return Page();
                }
            }
            IncomeCategory = await _incomeCategoryService.GetCategoryDetailAsync(id);
            IncomeCategoryList = new SelectList((await _incomeCategoryService.GetIncomeCategoryDropdownListAsync()), "Category", "Category", IncomeCategory.Name);
            return Page();
        }
        public async Task<IActionResult> OnPostDelete()
        {
            try
            {
                var deleteSuccess = await _incomeCategoryService.DeleteIncomeCategoryAsync(id);
            }
            catch (Exception ex)
            {
                InfoMessage = ex.Message;
                return LocalRedirect("~/Income/Categories/Index");
            }
            SuccessMessage = "Successfully Deleted a Category!";
            return LocalRedirect("~/Income/Categories/Index");
        }
    }
}
