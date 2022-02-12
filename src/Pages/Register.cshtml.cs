using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using CashTrack.Services.UserService;
using CashTrack.Models.UserModels;
using System;

namespace CashTrack.Pages
{
    [AllowAnonymous]
    public class Register : PageModel
    {
        private readonly IUserService _userService;

        [BindProperty, Display(Name = "Email")]
        public string Email { get; set; }
        [BindProperty, Display(Name = "Password")]
        public string Password { get; set; }
        [BindProperty, Display(Name = "First Name")]
        public string FirstName { get; set; }
        [BindProperty, Display(Name = "Last Name")]
        public string LastName { get; set; }
        public Register(IUserService userService)
        {
            _userService = userService;
        }

        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();
            try
            {
                var result = await _userService.CreateUserAsync(new UserModels.AddEditUser()
                {
                    Email = Email,
                    FirstName = FirstName,
                    LastName = LastName,
                    Password = Password
                });
            }
            catch (Exception ex)
            {
                //find a way to display the error message
                ModelState.AddModelError("", ex.Message);
                return Page();
            }


            //var claims = new List<Claim>
            //{
            //    new Claim(ClaimTypes.Email, Email),
            //    new Claim(ClaimTypes.Name, FirstName)
            //};
            //var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            //var principal = new ClaimsPrincipal(identity);
            //await HttpContext.SignInAsync(principal);

            return Page();
        }
    }
}
