using CashTrack.Common;
using CashTrack.Models.Common;
using CashTrack.Models.ExportModels;
using CashTrack.Models.UserModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.ExportService;
using CashTrack.Services.UserService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CashTrack.Pages.Settings
{
    public class Index : PageModelBase
    {
        private readonly IConfiguration _config;
        private readonly IExportService _exportService;
        private readonly IUserService _userService;
        public Index(IConfiguration config, IExportService exportService, IUserService userService)
        {
            _config = config;
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
        public decimal Tax { get; set; }
        public IActionResult OnGet()
        {
            ExportOptions = new SelectList(ExportFileOptions.GetAll, "Key", "Value");
            return Page();
        }
        public async Task<IActionResult> OnPostChangePassword()
        {
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
            TempData["SuccessMessage"] = "Successfully changed your password!";
            return LocalRedirect("/Settings");
        }
        public async Task<IActionResult> OnPostChangeUsername()
        {
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
            TempData["SuccessMessage"] = "Successfully changed your username!";
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