using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CashTrack.Pages.Shared
{
    public class PageModelBase : PageModel
    {
        [TempData]
        public string InfoMessage { get; set; }
        [TempData]
        public string SuccessMessage { get; set; }
    }
}
