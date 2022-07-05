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
        public IActionResult OnGet()
        {
            ExportOptions = new SelectList(ExportFileOptions.GetAll, "Key", "Value");
            return Page();
        }
        public async Task<ActionResult> OnPostExport()
        {

            var filePath = await _exportService.ExportTransactions(new ExportTransactionsRequest() { IsIncome = false });
            if (string.IsNullOrEmpty(filePath))
            {
                ModelState.AddModelError("", "You don't have any data export.");
                return Page();
            }
            byte[] fileBytes = GetFile(filePath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "export.csv");
        }

        private byte[] GetFile(string filePath)
        {
            FileStream fs = System.IO.File.OpenRead(filePath);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new IOException(filePath);
            return data;
        }
    }
}

//code from earlier...

//    public async Task<ActionResult> OnPostExportData()
//    {
//        List<string> filePaths = await _exportService.ExportData();
//        if (!filePaths.Any())
//        {
//            ModelState.AddModelError("", "You don't have any data to export.");
//            return Page();
//        }
//        foreach (var path in filePaths)
//        {
//            //Find a way to download multiple files... this might not work
//            byte[] fileBytes = GetFile(path);
//            //find a way to make the file name match the entity type or whatever.
//            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "export.csv");
//        }
//    }