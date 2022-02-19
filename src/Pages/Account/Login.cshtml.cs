using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using CashTrack.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CashTrack.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<Users> _signInManager;
        private readonly UserManager<Users> _userManager;
        public string ReturnUrl { get; set; }
        [BindProperty, Display(Name = "User Name")]
        public string UserName { get; set; }
        [BindProperty, Display(Name = "Password")]
        public string Password { get; set; }
        public LoginModel(SignInManager<Users> signInManager, UserManager<Users> userManager) => (_signInManager, _userManager) = (signInManager, userManager);

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

            //var claims = new List<Claim>
            //{
            //    new Claim(ClaimTypes.Name, UserName),
            //};
            //var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            //var principal = new ClaimsPrincipal(identity);
            //await HttpContext.SignInAsync(principal);

            return Page();
        }
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return LocalRedirect("/login");
        }
    }
}
