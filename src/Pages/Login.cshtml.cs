using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CashTrack.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public void OnGet()
        {
        }
    }
}
