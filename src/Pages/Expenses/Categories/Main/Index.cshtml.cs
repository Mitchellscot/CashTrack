using CashTrack.Models.Common;
using CashTrack.Models.MainCategoryModels;
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
            var timeSpanIndex = Array.IndexOf(TimeSpans, TimeSpan);
            timeSpanIndex = timeSpanIndex < 0 || timeSpanIndex >= TimeSpans.Length ? 0 : timeSpanIndex;
            MainCategoryResponse = await _mainCategoryService.GetMainCategoriesAsync(new MainCategoryRequest() { TimeOption = (MainCategoryTimeOptions)timeSpanIndex });
            return Page();
        }
    }
}
