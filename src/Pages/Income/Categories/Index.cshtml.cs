using CashTrack.Common.Exceptions;
using CashTrack.Models.IncomeCategoryModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.IncomeCategoryService;
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
                var categorySuccess = IncomeCategoryModal.IsEdit ? await _categoryService.UpdateIncomeCategoryAsync(IncomeCategoryModal) : await _categoryService.CreateIncomeCategoryAsync(IncomeCategoryModal);
                if (categorySuccess < 1)
                {
                    ModelState.AddModelError("", "Error adding Category. Please try again");
                    return await PrepareAndRenderPage();
                }
            }
            catch (Exception ex)
            {
                InfoMessage = ex.Message;
                return await PrepareAndRenderPage();
            }

            SuccessMessage = IncomeCategoryModal.IsEdit ? "Successfully edited a Category!" : "Successfully added a new Category!";
            return LocalRedirect(Url.Content(IncomeCategoryModal.ReturnUrl));
        }
        private async Task<IActionResult> PrepareAndRenderPage()
        {
            IncomeCategoryResponse = await _categoryService.GetIncomeCategoriesAsync(new IncomeCategoryRequest());
            return Page();
        }
    }
}
