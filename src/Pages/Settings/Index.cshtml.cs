using CashTrack.Common;
using CashTrack.Models.Common;
using CashTrack.Models.ExportModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.ExportService;
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
        public Index(IConfiguration config, IExportService exportService)
        {
            _config = config;
            _exportService = exportService;
        }
        public SelectList ExportOptions { get; set; }
        public int ExportOption { get; set; }
        public bool ExportAsReadable { get; set; }
        public IActionResult OnGet()
        {
            ExportOptions = new SelectList(ExportFileOptions.GetAll, "Key", "Value");
            return Page();
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