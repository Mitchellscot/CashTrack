using CashTrack.Models.ExportModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.ExportService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Core;
using System.IO;

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
        public async Task<ActionResult> OnPost()
        {
            var filePath = await _exportService.ExportTransactions(new ExportTransactionsRequest() { IsIncome = false });
            byte[] fileBytes = GetFile(filePath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "export.csv");
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