using CashTrack.Models.MainCategoryModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.MainCategoriesService;
using Microsoft.AspNetCore.Mvc;
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
