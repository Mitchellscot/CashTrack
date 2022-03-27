using CashTrack.Models.ExpenseReviewModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Services.ExpenseReviewService;
using CashTrack.Services.SubCategoryService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.PowerShell;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;

namespace CashTrack.Pages.Review
{
    public class ExpensesModel : PageModel
    {
        private readonly ISubCategoryService _subCategoryService;
        private readonly IExpenseReviewService _expenseService;

        public ExpensesModel(IExpenseReviewService expenseService, ISubCategoryService subCategoryService)
        {
            _subCategoryService = subCategoryService;
            _expenseService = expenseService;
        }
        [TempData]
        public string ScriptResults { get; set; }
        public ExpenseReviewResponse ExpenseReviewResponse { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        [BindProperty]
        //public SelectList SuggestedCategory { get; set; }
        public SubCategoryDropdownSelection[] CategoryList { get; set; }
        public async Task<IActionResult> OnGet()
        {
            await PrepareData();
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
            await PrepareData();
            return Page();
        }
        private async Task PrepareData()
        {
            CategoryList = await _subCategoryService.GetSubCategoryDropdownListAsync();
            PageNumber = ExpenseReviewResponse != null ? ExpenseReviewResponse.PageNumber : PageNumber == 0 ? 1 : PageNumber;

            ExpenseReviewResponse = await _expenseService.GetExpenseReviewsAsync(new ExpenseReviewRequest() { PageNumber = PageNumber });
        }
    }
}