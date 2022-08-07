using CashTrack.Common.Exceptions;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.SubCategoryService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace CashTrack.Pages.Expenses.Categories
{
    public class IndexModel : PageModelBase
    {
        private readonly ISubCategoryService _subCategoryService;

        public IndexModel(ISubCategoryService subcategoryService)
        {
            _subCategoryService = subcategoryService;
        }
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        public SubCategoryResponse SubCategoryResponse { get; set; }
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
                    InfoMessage = "No category found with the name " + SearchTerm;
                    SubCategoryResponse = await _subCategoryService.GetSubCategoriesAsync(new SubCategoryRequest() { 
                        PageNumber = this.PageNumber });
                    return Page();
                }
            }

            SubCategoryResponse = await _subCategoryService.GetSubCategoriesAsync(new SubCategoryRequest() { PageNumber = this.PageNumber });
            return Page();
        }
    }
}
