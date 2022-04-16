using CashTrack.Common.Exceptions;
using CashTrack.Models.IncomeSourceModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.IncomeSourceService;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CashTrack.Pages.Sources
{
    public class IndexModel : PageModelBase
    {
        private readonly IIncomeSourceService _sourceService;
        public IndexModel(IIncomeSourceService sourceService) => _sourceService = sourceService;
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        public IncomeSourceResponse SourceResponse { get; set; }

        public async Task<IActionResult> OnGet()
        {
            if (SearchTerm != null)
            {
                try
                {
                    var sourceId = (await _sourceService.GetIncomeSourceByName(SearchTerm)).Id;
                    return RedirectToPage("./Detail", new { id = sourceId });
                }
                catch (IncomeSourceNotFoundException)
                {
                    InfoMessage = "No income source found with the name " + SearchTerm;
                    SourceResponse = await _sourceService.GetIncomeSourcesAsync(new IncomeSourceRequest() { PageNumber = this.PageNumber });
                    return Page();
                }
            }

            SourceResponse = await _sourceService.GetIncomeSourcesAsync(new IncomeSourceRequest() { PageNumber = this.PageNumber });
            return Page();
        }
    }
}
