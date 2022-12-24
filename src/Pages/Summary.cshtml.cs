using CashTrack.Models.SummaryModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.SummaryService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace CashTrack.Pages
{
    public class SummaryModel : PageModelBase
    {
        private ISummaryService _summaryService;
        public AllTimeSummaryResponse SummaryResponse { get; set; }

        public SummaryModel(ISummaryService summaryService)
        {
            _summaryService = summaryService;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            SummaryResponse = await _summaryService.GetAllTimeSummaryAsync();
            return Page();
        }
    }
}
