using CashTrack.Common.Exceptions;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.SubCategoryService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;

namespace CashTrack.Pages.Expenses.Categories
{

    public class DetailModel : PageModelBase
    {
        private readonly ISubCategoryService _subCategoryService;
        public DetailModel(ISubCategoryService categoryService) => _subCategoryService = categoryService;
        [BindProperty(SupportsGet = true)]
        public int id { get; set; }
        public SubCategoryDetail SubCategory { get; set; }
        public SelectList SubCategorySelectList { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }
        public async Task<IActionResult> OnGet()
        {
            if (SearchTerm != null)
            {
                try
                {
                    var categoryId = (await _subCategoryService.GetSubCategoryByNameAsync(SearchTerm)).Id;
                    return RedirectToPage("./Detail", new { id = categoryId });
                }
                catch (CategoryNotFoundException)
                {
                    InfoMessage = "No Categories found with the name " + SearchTerm;
                    return Page();
                }
            }
            var categoryNames = await _subCategoryService.GetSubCategoryDropdownListAsync();
            SubCategory = await _subCategoryService.GetSubCategoryDetailsAsync(id);
            SubCategorySelectList = new SelectList(categoryNames, SubCategory.Name);
            return Page();
        }
        public async Task<IActionResult> OnPostDelete()
        {
            try
            {
                var deleteSuccess = await _subCategoryService.DeleteSubCategoryAsync(id);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return LocalRedirect("~/Expenses/Categories/Index");
            }
            TempData["SuccessMessage"] = "Successfully Deleted a Category!";
            return LocalRedirect("~/Expenses/Categories/Index");
        }
    }
}
