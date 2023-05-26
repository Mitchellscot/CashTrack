using CashTrack.Common;
using CashTrack.Models.Common;
using CashTrack.Models.UserModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.ExportService;
using CashTrack.Services.UserService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using System.Threading.Tasks;

namespace CashTrack.Pages.Settings
{
    public class Index : PageModelBase
    {
        private readonly IExportService _exportService;
        private readonly IUserService _userService;
        public Index(IExportService exportService, IUserService userService )
        {
            _exportService = exportService;
            _userService = userService;
        }
        public SelectList ExportOptions { get; set; }
        public int ExportOption { get; set; }
        public bool ExportAsReadable { get; set; }
        [BindProperty]
        public ChangePassword ChangePassword { get; set; }
        [BindProperty]
        public ChangeUsername ChangeUsername { get; set; }
        [BindProperty]
        public decimal DefaultTax { get; set; }
        [BindProperty]
        public decimal NewTax { get; set; }

        public async Task<IActionResult> OnGet()
        {
            ExportOptions = new SelectList(ExportFileOptions.GetAll, "Key", "Value");
            DefaultTax = await _userService.GetDefaultTax(User.Identity.Name ?? "demo");
            return Page();
        }
        public async Task<IActionResult> OnPostChangePassword()
        {
            if (IsDemoApp())
                return await OnGet();

            if (string.IsNullOrEmpty(ChangePassword.OldPassword) || string.IsNullOrEmpty(ChangePassword.NewPassword))
            {
                ModelState.AddModelError("", "Please fill out both the old and new Passwords");
                return Page();
            }
            if (ChangePassword.NewPassword != ChangePassword.ConfirmPassword)
            {
                ModelState.AddModelError("", "Confirm your new password matches and try again.");
                return Page();
            }
            ChangePassword.Username = User.Identity.Name;
            var result = await _userService.UpdatePasswordAsync(ChangePassword);

            if (!result)
            {
                ModelState.AddModelError("", "There was an error changing your password. Try again.");
                return Page();
            }
            SuccessMessage = "Successfully changed your password!";
            return LocalRedirect("/Settings");
        }
        public async Task<IActionResult> OnPostChangeUsername()
        {
            if (IsDemoApp())
                return await OnGet();

            if (string.IsNullOrEmpty(ChangeUsername.NewUsername) || string.IsNullOrEmpty(ChangeUsername.ConfirmUsername))
            {
                ModelState.AddModelError("", "Please fill out both the old and new Passwords");
                return Page();
            }
            if (ChangeUsername.NewUsername != ChangeUsername.ConfirmUsername)
            {
                ModelState.AddModelError("", "Confirm your new username matches and try again.");
                return Page();
            }
            ChangeUsername.Username = User.Identity.Name;
            var result = await _userService.UpdateUsernameAsync(ChangeUsername);

            if (!result)
            {
                ModelState.AddModelError("", "There was an error changing your username. Try again.");
                return Page();
            }
            SuccessMessage = "Successfully changed your username!";
            return LocalRedirect("/Settings");
        }
        public async Task<IActionResult> OnPostChangeDefaultTax()
        {
            if (IsDemoApp())
                return Page();

            if (NewTax == decimal.Zero)
            {
                ModelState.AddModelError("", "Please add a new default tax and try again.");
                return Page();
            }
            if (NewTax >= decimal.One || NewTax <= decimal.Zero)
            {
                ModelState.AddModelError("", "Tax rate must be betwee 0 and 1. Please enter a valid decimal.");
                return Page();
            }
            var result = await _userService.UpdateDefaultTax(User.Identity.Name, NewTax);
            if (!result)
            {
                ModelState.AddModelError("", "There was an error changing your default tax. Try again.");
                return Page();
            }
            SuccessMessage = "Successfully changed your default tax!";
            return LocalRedirect("/Settings");
        }
        public async Task<ActionResult> OnPostExport(int ExportOption, bool ExportAsReadable)
        {
            var filePath = await _exportService.ExportData(ExportOption, ExportAsReadable);
            byte[] fileBytes = FileUtilities.GetFileBytesAndDeleteFile(filePath);

            if (filePath.Contains("archive"))
            {
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Zip, Path.GetFileName(filePath) + ".zip");
            }

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(filePath));
        }
    }
}