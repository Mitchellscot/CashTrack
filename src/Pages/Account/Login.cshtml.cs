using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using CashTrack.Data.Entities;
using Microsoft.AspNetCore.Identity;
using CashTrack.Pages.Shared;
using CashTrack.Models.AuthenticationModels;
using FluentValidation;
using System;
using FluentValidation.AspNetCore;
using CashTrack.Common.Extensions;
using Microsoft.Extensions.Logging;

namespace CashTrack.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModelBase
    {
        private readonly SignInManager<UserEntity> _signInManager;
        private readonly UserManager<UserEntity> _userManager;
        private readonly ILogger<LoginModel> _logger;

        public string ReturnUrl { get; set; }
        [BindProperty]
        public AuthenticationModels.Request LoginRequest { get; set; }
        public LoginModel(SignInManager<UserEntity> signInManager, UserManager<UserEntity> userManager, ILogger<LoginModel> logger) => (_signInManager, _userManager, _logger) = (signInManager, userManager, logger);

        public async Task<IActionResult> OnPostAsync()
        {
            ReturnUrl ??= Url.Content("~/");

            var user = await _userManager.FindByNameAsync(LoginRequest.UserName);
            if (user == null)
            {
                ModelState.AddModelError("", "User Not Found");
                return Page();
            };

            var result = await _signInManager.PasswordSignInAsync(LoginRequest.UserName, LoginRequest.Password, true, false);
            if (result.Succeeded)
            {
                _logger.LogInformation($"{LoginRequest.UserName} has logged in at {DateTime.Now} CST Time");
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
