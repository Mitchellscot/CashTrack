using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CashTrack.Pages.Expenses
{
    [AllowAnonymous]
    public class Index : PageModel
    {
        public void OnGet()
        {
        }
    }
}
