using CashTrack.Common.Exceptions;
using CashTrack.Models.MainCategoryModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.MainCategoriesService;
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
        private readonly IMainCategoriesService _mainCategoryService;

        public DetailModel(ISubCategoryService categoryService, IMainCategoriesService mainCategoryService) => (_subCategoryService, _mainCategoryService) = (categoryService, mainCategoryService);
        [BindProperty(SupportsGet = true)]
        public int id { get; set; }
        public SubCategoryDetail SubCategory { get; set; }
        public SelectList SubCategoryList { get; set; }
        public MainCategoryDropdownSelection[] MainCategoryList { get; set; }
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
            SubCategory = await _subCategoryService.GetSubCategoryDetailsAsync(id);
            SubCategoryList = new SelectList((await _subCategoryService.GetSubCategoryDropdownListAsync()), "Category", "Category", SubCategory.Name);
            MainCategoryList = await _mainCategoryService.GetMainCategoriesForDropdownListAsync();
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
                InfoMessage = ex.Message;
                return LocalRedirect("~/Expenses/Categories/Index");
            }
            SuccessMessage = "Successfully Deleted a Category!";
            return LocalRedirect("~/Expenses/Categories/Index");
        }
    }
}
