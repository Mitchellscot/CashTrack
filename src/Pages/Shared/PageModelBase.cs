using CashTrack.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CashTrack.Pages.Shared
{
    public class PageModelBase : PageModel
    {
        protected readonly IWebHostEnvironment _env;
        public PageModelBase(IWebHostEnvironment env)
        {
            _env = env;
        }
        [TempData]
        public string InfoMessage { get; set; }
        [TempData]
        public string SuccessMessage { get; set; }

        protected bool IsDemoApp()
        {
            if (_env.EnvironmentName.Equals(CashTrackEnv.Production, System.StringComparison.InvariantCultureIgnoreCase))
            {
                ModelState.AddModelError("", "This feature is disabled in the demo app.");
                return true;
            }
            else return false;
        }
    }
}
