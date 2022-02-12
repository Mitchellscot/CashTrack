using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

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
        //public async Task<IActionResult> OnPostAsync()
        //{
        //    if (!ModelState.IsValid)
        //        return Page();

        //    return Page();
        //}
    }
}
