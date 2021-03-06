using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using CashTrack.Data.Entities;
using Microsoft.AspNetCore.Identity;
using CashTrack.Pages.Shared;

namespace CashTrack.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModelBase
    {
        private readonly SignInManager<UserEntity> _signInManager;
        private readonly UserManager<UserEntity> _userManager;
        public string ReturnUrl { get; set; }
        [BindProperty, Display(Name = "User Name")]
        public string UserName { get; set; }
        [BindProperty, Display(Name = "Password")]
        public string Password { get; set; }
        public LoginModel(SignInManager<UserEntity> signInManager, UserManager<UserEntity> userManager) => (_signInManager, _userManager) = (signInManager, userManager);

        public async Task<IActionResult> OnPostAsync()
        {
            ReturnUrl ??= Url.Content("~/");
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                ModelState.AddModelError("", "User Not Found");
                return Page();
            };

            var result = await _signInManager.PasswordSignInAsync(UserName, Password, true, false);
            if (result.Succeeded)
            {
                return LocalRedirect(ReturnUrl);
            }
            else
            {
                ModelState.AddModelError("", "Invalid password");
                return Page();
            }
        }
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return LocalRedirect("/login");
        }
    }
}
