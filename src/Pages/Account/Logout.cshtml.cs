using CashTrack.Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace CashTrack.Pages.Account
{
    public class Logout : PageModel
    {
        private readonly SignInManager<Users> _signInManager;

        public Logout(SignInManager<Users> signInManager) => _signInManager = signInManager;

        public async Task<IActionResult> OnPostAsync()
        {
            await _signInManager.SignOutAsync();
            return LocalRedirect("/login");
        }
    }
}
