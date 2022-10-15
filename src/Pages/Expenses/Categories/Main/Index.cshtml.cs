using CashTrack.Models.Common;
using CashTrack.Models.ExpenseModels;
using CashTrack.Models.MainCategoryModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.MainCategoriesService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CashTrack.Pages.Expenses.Categories.Main
{
    public class IndexModel : PageModelBase
    {
        private readonly IMainCategoriesService _mainCategoryService;
        public MainCategoryResponse MainCategoryResponse { get; set; }
        [BindProperty(SupportsGet = true)]
        public string TimeSpan { get; set; } = "All Time";
        public string[] TimeSpans = new[] { "All Time", "Five Years", "Three Years", "One Year", "Six Months" };
        [BindProperty]
        public AddEditMainCategoryModal MainCategoryModal { get; set; }
        public IndexModel(IMainCategoriesService mainCategoryService)
        {
            _mainCategoryService = mainCategoryService;
        }
        public async Task<ActionResult> OnGetAsync()
        {
            return await PrepareAndRenderPage();
        }
        public async Task<IActionResult> OnPostAddEditMainCategoryModal(AddEditMainCategoryModal request)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Error adding Category. Please try again");
                return await PrepareAndRenderPage();
            }
            try
            {
                var categorySuccess = MainCategoryModal.IsEdit ? await _mainCategoryService.UpdateMainCategoryAsync(MainCategoryModal) : await _mainCategoryService.CreateMainCategoryAsync(MainCategoryModal);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return await PrepareAndRenderPage();
            }

            TempData["SuccessMessage"] = MainCategoryModal.IsEdit ? "Successfully edited a Category!" : "Successfully added a new Category!";
            return LocalRedirect(MainCategoryModal.ReturnUrl);
        }
        public async Task<IActionResult> OnPostDelete(int id)
        {
            try
            {
                var deleteSuccess = await _mainCategoryService.DeleteMainCategoryAsync(id);
            }
            catch (Exception ex)
            {
                TempData["InfoMessage"] = ex.Message;
                return LocalRedirect("/Expenses/Categories/Main/Index");
            }
            TempData["SuccessMessage"] = "Successfully Deleted a Main Category!";
            return LocalRedirect("/Expenses/Categories/Main/Index");
        }

        private async Task<ActionResult> PrepareAndRenderPage()
        {
            var timeSpanIndex = Array.IndexOf(TimeSpans, TimeSpan);
            timeSpanIndex = timeSpanIndex < 0 || timeSpanIndex >= TimeSpans.Length ? 0 : timeSpanIndex;
            MainCategoryResponse = await _mainCategoryService.GetMainCategoriesAsync(new MainCategoryRequest() { TimeOption = (MainCategoryTimeOptions)timeSpanIndex });
            return Page();
        }

    }
}
