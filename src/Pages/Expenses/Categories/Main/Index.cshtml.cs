using CashTrack.Models.Common;
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
        [BindProperty(SupportsGet = true)]
        public int TimeSpan { get; set; }
        public string Title { get; set; } = "Main Categories";

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
            MainCategoryResponse = await _mainCategoryService.GetMainCategoriesAsync(new MainCategoryRequest() { TimeOption = (MainCategoryTimeOptions)TimeSpan });
            Title = DisplayTitleBasedOnTimeSpan(this.TimeSpan);
            return Page();
        }
        private string DisplayTitleBasedOnTimeSpan(int timeSpan) => timeSpan switch
        {
            0 => "Showing All Expenses",
            1 => "Expenses From the Past 5 Years",
            2 => "Expenses From the Past 3 Years",
            3 => "Expenses From the Year",
            4 => "Expenses From the Past 6 Months",
            _ => "Main Categories"
        };
    }
}
