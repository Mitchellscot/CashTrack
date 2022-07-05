using CashTrack.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace CashTrack.Pages
{

    public class IndexModel : PageModelBase
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            return Page();
        }
    }
}