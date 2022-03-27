using CashTrack.Models.ExpenseReviewModels;
using CashTrack.Services.ExpenseReviewService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.PowerShell;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;

namespace CashTrack.Pages.Review
{
    public class ExpensesModel : PageModel
    {
        private readonly IExpenseReviewService _expenseService;

        public ExpensesModel(IExpenseReviewService expenseService)
        {
            _expenseService = expenseService;
        }
        [TempData]
        public string ScriptResults { get; set; }
        public ExpenseReviewResponse ExpenseReviewResponse { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        public async Task<IActionResult> OnGet(int pageNumber)
        {
            PageNumber = ExpenseReviewResponse != null ? ExpenseReviewResponse.PageNumber : pageNumber == 0 ? 1 : pageNumber;

            ExpenseReviewResponse = await _expenseService.GetExpenseReviewsAsync(new ExpenseReviewRequest() { PageNumber = PageNumber });
            return Page();
        }
        public async Task<IActionResult> OnPostRunScript()
        {
            var initialSessionState = InitialSessionState.CreateDefault();
            initialSessionState.ExecutionPolicy = ExecutionPolicy.Unrestricted;
            var runspace = RunspaceFactory.CreateRunspace(initialSessionState);
            runspace.Open();
            using (PowerShell ps = PowerShell.Create())
            {
                ps.Runspace = runspace;
                ps.Streams.Information.DataAdded += delegate (object sender, DataAddedEventArgs e)
                {
                    var streamObjectsReceived = sender as PSDataCollection<InformationRecord>;
                    var currentStreamRecord = streamObjectsReceived[e.Index];
                    ScriptResults += $"{currentStreamRecord.MessageData.ToString()} ";
                };
                ps.Commands.AddScript(@". C:\Users\Mitch\Code\CashTrack\ct-data\Scripts\Run-Transactions.ps1");
                var result = await ps.InvokeAsync().ConfigureAwait(false);
            }
            PageNumber = ExpenseReviewResponse != null ? ExpenseReviewResponse.PageNumber : PageNumber == 0 ? 1 : PageNumber;

            ExpenseReviewResponse = await _expenseService.GetExpenseReviewsAsync(new ExpenseReviewRequest() { PageNumber = PageNumber });
            return Page();
        }
    }
}