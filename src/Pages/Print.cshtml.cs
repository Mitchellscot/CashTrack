using CashTrack.Common.Extensions;
using CashTrack.Models.BudgetModels;
using CashTrack.Models.SummaryModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.BudgetService;
using CashTrack.Services.SummaryService;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CashTrack.Pages
{
    public class PrintModel : PageModelBase
    {
        private readonly IBudgetService _budgetService;
        private readonly ISummaryService _summaryService;
        [BindProperty(SupportsGet = true)]
        public string Type { get; set; }
        [BindProperty(SupportsGet = true)]
        public int Year { get; set; }
        [BindProperty(SupportsGet = true)]
        public int Month { get; set; }
        public List<TransactionBreakdown> Transactions { get; set; }
        public List<BudgetBreakdown> Budgets { get; set; }
        public PrintModel(ISummaryService service, IBudgetService budgetService)
        {
            _budgetService = budgetService;
            _summaryService = service;
        }
        public async Task<IActionResult> OnGetAsync(string type)
        {
            if (type.IsEqualTo("transaction"))
                Transactions = await _summaryService.GetTransactionsToPrint(new PrintTransactionsRequest() { Year = this.Year, Month = this.Month });
            else if (type.IsEqualTo("budget"))
                Budgets = await _budgetService.GetBudgetsToPrint(new PrintBudgetRequest() { Year = this.Year, Month = this.Month });

            return Page();
        }
    }
}
