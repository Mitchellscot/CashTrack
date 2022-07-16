using CashTrack.Models.Common;
using CashTrack.Models.ExportModels;
using CashTrack.Services.ExportService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CashTrack.Pages.Settings
{
    public class Index : PageModel
    {
        private readonly ILogger<Index> _logger;
        private readonly IConfiguration _config;
        private readonly IExportService _exportService;
        public Index(ILogger<Index> logger, IConfiguration config, IExportService exportService)
        {
            _logger = logger;
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
            byte[] fileBytes = GetFileBytes(filePath);

            if (filePath.Contains("archive"))
            {
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Zip, Path.GetFileName(filePath) + ".zip");
            }

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(filePath));

        }

        private byte[] GetFileBytes(string filePath)
        {
            FileStream fileStream = System.IO.File.OpenRead(filePath);
            byte[] data = new byte[fileStream.Length];
            int buffer = fileStream.Read(data, 0, data.Length);
            if (buffer != fileStream.Length)
                throw new IOException(filePath);
            fileStream.Close();
            System.IO.File.Delete(filePath);
            return data;
        }
    }
}