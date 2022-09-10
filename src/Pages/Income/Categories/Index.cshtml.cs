using CashTrack.Common.Exceptions;
using CashTrack.Models.Common;
using CashTrack.Models.IncomeCategoryModels;
using CashTrack.Models.MainCategoryModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.IncomeCategoryService;
using CashTrack.Services.MainCategoriesService;
using CashTrack.Services.SubCategoryService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CashTrack.Pages.Income.Categories
{
    public class IndexModel : PageModelBase
    {
        private readonly IIncomeCategoryService _categoryService;

        public IndexModel(IIncomeCategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [BindProperty(SupportsGet = true)]
        public IncomeCategoryOrderBy Query { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool Q2 { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        [BindProperty]
        public AddEditIncomeCategoryModal IncomeCategoryModal { get; set; }
        public IncomeCategoryResponse IncomeCategoryResponse { get; set; }

        public async Task<IActionResult> OnGet()
        {

            if (SearchTerm != null)
            {
                try
                {
                    var categoryId = (await _categoryService.GetIncomeCategoryByNameAsync(SearchTerm)).Id;
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
        public async Task<IActionResult> OnPostAddEditIncomeCategoryModal()
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Error adding Category. Please try again");
                return await PrepareAndRenderPage();
            }
            try
            {
                var newCategory = new IncomeCategory()
                {
                    Name = IncomeCategoryModal.Name,
                    InUse = IncomeCategoryModal.InUse,
                    Notes = IncomeCategoryModal.Notes
                };
                if (IncomeCategoryModal.IsEdit)
                {
                    newCategory.Id = IncomeCategoryModal.Id;
                }

                var categorySuccess = IncomeCategoryModal.IsEdit ? await _categoryService.UpdateIncomeCategoryAsync(newCategory) : await _categoryService.CreateIncomeCategoryAsync(newCategory);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return await PrepareAndRenderPage();
            }

            TempData["SuccessMessage"] = IncomeCategoryModal.IsEdit ? "Successfully edited a Category!" : "Successfully added a new Category!";
            return LocalRedirect(IncomeCategoryModal.Returnurl);
        }
        private async Task<IActionResult> PrepareAndRenderPage()
        {
            IncomeCategoryResponse = await _categoryService.GetIncomeCategoriesAsync(new IncomeCategoryRequest() { PageNumber = this.PageNumber, Order = this.Query, Reversed = this.Q2 });
            return Page();
        }
    }
}
