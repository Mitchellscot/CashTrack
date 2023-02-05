using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CashTrack.Data.Entities;
using Microsoft.AspNetCore.Identity;
using CashTrack.Pages.Shared;
using CashTrack.Models.AuthenticationModels;
using System;
using Microsoft.Extensions.Logging;

namespace CashTrack.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModelBase
    {
        private readonly SignInManager<UserEntity> _signInManager;
        private readonly UserManager<UserEntity> _userManager;
        private readonly ILogger<LoginModel> _logger;
        [BindProperty]
        public AuthenticationModels.Request LoginRequest { get; set; } = new AuthenticationModels.Request();
        public LoginModel(SignInManager<UserEntity> signInManager, UserManager<UserEntity> userManager, ILogger<LoginModel> logger) => (_signInManager, _userManager, _logger) = (signInManager, userManager, logger);

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByNameAsync(LoginRequest.UserName);
            if (user == null)
            {
                ModelState.AddModelError("", "User Not Found");
                return Page();
            };

            var result = await _signInManager.PasswordSignInAsync(LoginRequest.UserName, LoginRequest.Password, true, false);
            if (result.Succeeded)
            {
                _logger.LogInformation($"{LoginRequest.UserName} has logged in at {DateTime.Now} CST Time from {HttpContext.Connection.RemoteIpAddress}");
                return LocalRedirect("~/");
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
            _logger.LogInformation($"{LoginRequest.UserName} has logged out at {DateTime.Now} CST Time from {HttpContext.Request.Host.Host}");
            return RedirectToPage("/Monthly");
        }
    }
}
