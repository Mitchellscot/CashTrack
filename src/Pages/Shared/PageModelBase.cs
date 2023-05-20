using CashTrack.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CashTrack.Pages.Shared
{
    public class PageModelBase : PageModel
    {
        protected readonly string _env;
        protected bool UserIsAuthenticated =>  User?.Identity != null && User.Identity.IsAuthenticated;
        public PageModelBase()
        {
            _env = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        }
        [TempData]
        public string InfoMessage { get; set; }
        [TempData]
        public string SuccessMessage { get; set; }

        protected bool IsDemoApp()
        {
            if (_env.Equals(CashTrackEnv.Production, System.StringComparison.InvariantCultureIgnoreCase))
            {
                InfoMessage ="This feature is disabled in the demo app.";
                return true;
            }
            else return false;
        }
    }
}
