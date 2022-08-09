using CashTrack.Common.Exceptions;
using CashTrack.Models.Common;
using CashTrack.Models.MainCategoryModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.MainCategoriesService;
using CashTrack.Services.SubCategoryService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CashTrack.Pages.Expenses.Categories
{
    public class IndexModel : PageModelBase
    {
        private readonly ISubCategoryService _subCategoryService;
        private readonly IMainCategoriesService _mainCategoryService;

        public IndexModel(ISubCategoryService subcategoryService, IMainCategoriesService mainCategoryService)
        {
            _subCategoryService = subcategoryService;
            _mainCategoryService = mainCategoryService;
        }
        [BindProperty(SupportsGet = true)]
        public SubCategoryOrderBy Query { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool Q2 { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        [BindProperty]
        public AddEditSubCategoryModal SubCategoryModal { get; set; }
        public MainCategoryDropdownSelection[] MainCategoryList { get; set; }
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
                    return await PrepareAndRenderPage();
                }
            }

            return await PrepareAndRenderPage();
        }
        public async Task<IActionResult> OnPostAddEditSubCategoryModal()
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Error adding Category. Please try again");
                return await PrepareAndRenderPage();
            }
            try
            {
                var newCategory = new SubCategory()
                {
                    Name = SubCategoryModal.Name,
                    InUse = SubCategoryModal.InUse,
                    MainCategoryId = SubCategoryModal.MainCategoryId,
                    Notes = SubCategoryModal.Notes
                };
                if (SubCategoryModal.IsEdit)
                {
                    newCategory.Id = SubCategoryModal.Id;
                }

                var categorySuccess = SubCategoryModal.IsEdit ? await _subCategoryService.UpdateSubCategoryAsync(newCategory) : await _subCategoryService.CreateSubCategoryAsync(newCategory);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return await PrepareAndRenderPage();
            }

            TempData["SuccessMessage"] = SubCategoryModal.IsEdit ? "Successfully edited a Category!" : "Successfully added a new Category!";
            return LocalRedirect(SubCategoryModal.Returnurl);
        }
        private async Task<IActionResult> PrepareAndRenderPage()
        {
            MainCategoryList = await _mainCategoryService.GetMainCategoriesForDropdownListAsync();
            SubCategoryResponse = await _subCategoryService.GetSubCategoriesAsync(new SubCategoryRequest() { PageNumber = this.PageNumber, Order = this.Query, Reversed = this.Q2 });
            return Page();
        }
    }
}
