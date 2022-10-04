using CashTrack.Common.Exceptions;
using CashTrack.Models.MainCategoryModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.MainCategoriesService;
using CashTrack.Services.SubCategoryService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;

namespace CashTrack.Pages.Expenses.Categories.Main
{
    public class IndexModel : PageModelBase
    {
        private readonly IMainCategoriesService _mainCategoryService;
        public MainCategoryResponse MainCategoryResponse { get; set; }

        public IndexModel(IMainCategoriesService mainCategoryService)
        {
            _mainCategoryService = mainCategoryService;
        }
        public async Task<ActionResult> OnGetAsync()
        {
            return await PrepareAndRenderPage();
        }

        private async Task<ActionResult> PrepareAndRenderPage()
        {
            MainCategoryResponse = await _mainCategoryService.GetMainCategoriesAsync(new MainCategoryRequest() {  });
            return Page();
        }
    }
}
