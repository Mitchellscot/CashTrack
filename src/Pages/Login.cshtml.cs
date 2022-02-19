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

        [BindProperty, Display(Name = "User Name")]
        public string UserName { get; set; }
        public string Password { get; set; }
        public LoginModel(SignInManager<Users> signInManager, UserManager<Users> userManager) => (_signInManager, _userManager) = (signInManager, userManager);

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                ModelState.AddModelError("", "User Not Found");
                return Page();
            };

            await _signInManager.SignInAsync(user, true);

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
