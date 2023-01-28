using CashTrack.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace CashTrack.Pages
{

    public class IndexModel : PageModelBase
    {
        public IndexModel()
        {
        }

        public IActionResult OnGet()
        {
            return Page();
        }
        //private async Task<IActionResult> PrepareAndRenderPage()
        //{
        //    return Page();
        //}
    }
}