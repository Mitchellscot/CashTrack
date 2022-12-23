using CashTrack.Models.SummaryModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.SummaryService;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CashTrack.Pages
{
    public class PrintModel : PageModelBase
    {
        private readonly ISummaryService _service;
        [BindProperty(SupportsGet = true)]
        public int Year { get; set; }
        [BindProperty(SupportsGet = true)]
        public int Month { get; set; }
        public List<TransactionBreakdown> Transactions { get; set; }
        public PrintModel(ISummaryService service)
        {
            _service = service;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            Transactions = await _service.GetTransactionsToPrint(new PrintTransactionsRequest() { Year = this.Year, Month = this.Month });
            return Page();
        }
    }
}
