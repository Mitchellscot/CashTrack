using CashTrack.Models.ExportModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.ExportService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Core;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace CashTrack.Pages
{

    public class IndexModel : PageModelBase
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _config;
        private readonly IExportService _exportService;

        public IndexModel(ILogger<IndexModel> logger, IConfiguration config, IExportService exportService)
        {
            _logger = logger;
            _config = config;
            _exportService = exportService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }
        public async Task<ActionResult> OnPostExportTransactions()
        {
            var filePath = await _exportService.ExportTransactions(new ExportTransactionsRequest() { IsIncome = false });
            if (string.IsNullOrEmpty(filePath))
            {
                ModelState.AddModelError("", "You don't have any transactions to export.");
                return Page();
            }
            byte[] fileBytes = GetFile(filePath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "export.csv");
        }
        public async Task<ActionResult> OnPostExportData()
        {
            List<string> filePaths = await _exportService.ExportData();
            if (!filePaths.Any())
            {
                ModelState.AddModelError("", "You don't have any data to export.");
                return Page();
            }
            foreach (var path in filePaths)
            {
                //Find a way to download multiple files... this might not work
                byte[] fileBytes = GetFile(path);
                //find a way to make the file name match the entity type or whatever.
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "export.csv");
            }
        }
        private byte[] GetFile(string filePath)
        {
            FileStream fs = System.IO.File.OpenRead(filePath);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new System.IO.IOException(filePath);
            return data;
        }
    }
}