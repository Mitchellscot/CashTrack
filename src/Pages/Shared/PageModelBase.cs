using CashTrack.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CashTrack.Pages.Shared
{
    public class PageModelBase : PageModel
    {
        protected bool UserIsAuthenticated =>  User?.Identity != null && User.Identity.IsAuthenticated;
        public PageModelBase()
        {

        }
        [TempData]
        public string InfoMessage { get; set; }
        [TempData]
        public string SuccessMessage { get; set; }
    }
}
