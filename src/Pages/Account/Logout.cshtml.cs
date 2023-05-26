using CashTrack.Data.Entities;
using CashTrack.Pages.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CashTrack.Pages.Account
{
    public class Logout : PageModelBase
    {
        private readonly SignInManager<UserEntity> _signInManager;

        public Logout(SignInManager<UserEntity> signInManager)
            => _signInManager = signInManager;

        public async Task<IActionResult> OnPostAsync()
        {
            await _signInManager.SignOutAsync();
            return LocalRedirect("/login");
        }
    }
}
