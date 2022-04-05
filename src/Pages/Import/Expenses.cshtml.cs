using CashTrack.Models.ExpenseModels;
using CashTrack.Models.ExpenseReviewModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.ExpenseReviewService;
using CashTrack.Services.ExpenseService;
using CashTrack.Services.MerchantService;
using CashTrack.Services.SubCategoryService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.PowerShell;
using System;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;

namespace CashTrack.Pages.Import
{
    public class ExpensesModel : PageModelBase
    {
        private readonly IMerchantService _merchantService;
        private readonly IExpenseService _expenseService;
        private readonly ISubCategoryService _subCategoryService;
        private readonly IExpenseReviewService _expenseReviewService;

        public ExpensesModel(IExpenseReviewService expenseReviewService, ISubCategoryService subCategoryService, IExpenseService expenseService, IMerchantService merchantService)
        {
            _merchantService = merchantService;
            _expenseService = expenseService;
            _subCategoryService = subCategoryService;
            _expenseReviewService = expenseReviewService;
        }
        public ExpenseReviewResponse ExpenseReviewResponse { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        public SubCategoryDropdownSelection[] CategoryList { get; set; }
        [BindProperty]
        public Expense SelectedExpense { get; set; }
        [BindProperty]
        public int SelectedExpenseId { get; set; }
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
                    var scriptMessage = currentStreamRecord.MessageData.ToString().Contains("Added") ? SuccessMessage += $"{currentStreamRecord.MessageData.ToString()}. " :
                    InfoMessage += $"{currentStreamRecord.MessageData.ToString()}. ";

                };
                ps.Commands.AddScript(@". C:\Users\Mitch\Code\CashTrack\ct-data\Scripts\Run-Transactions.ps1");
                var result = await ps.InvokeAsync().ConfigureAwait(false);
            }
            await PrepareData();
            return LocalRedirect("~/Import/Expenses");
        }
        public async Task<IActionResult> OnPostExpenseAdd()
        {
            if (string.IsNullOrEmpty(SelectedExpense.SubCategory))
            {
                ModelState.AddModelError("", "Expense must have an assigned category");
                await PrepareData();
                return Page();
            }
            if (!(await _merchantService.GetAllMerchantNames()).Any(x => x == SelectedExpense.Merchant))
            {
                await PrepareData();
                return Page();
            }

            try
            {
                var expenseId = await _expenseService.CreateExpenseAsync(SelectedExpense);

                var expenseReviewId = await _expenseReviewService.SetExpenseReviewToIgnoreAsync(SelectedExpenseId);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                await PrepareData();
                return Page();
            }
            TempData["SuccessMessage"] = "Sucessfully Added A New Expense!";
            return LocalRedirect("~/Import/Expenses");
        }
        public async Task<IActionResult> OnPostRemoveExpense()
        {
            try
            {
                var deleteSuccess = await _expenseReviewService.SetExpenseReviewToIgnoreAsync(SelectedExpenseId);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Page();
            }
            TempData["SuccessMessage"] = "Successfully Removed the Expense!";
            return LocalRedirect("~/Import/Expenses");
        }
        public async Task<IActionResult> OnPostSplitExpense()
        {
            if (string.IsNullOrEmpty(SelectedExpense.SubCategory))
            {
                ModelState.AddModelError("", "Expense must have an assigned category");
                await PrepareData();
                return Page();
            }
            if (!(await _merchantService.GetAllMerchantNames()).Any(x => x == SelectedExpense.Merchant))
            {
                await PrepareData();
                return Page();
            }
            int expenseId = 0;
            try
            {
                expenseId = await _expenseService.CreateExpenseAsync(SelectedExpense);

                var expenseReviewId = await _expenseReviewService.SetExpenseReviewToIgnoreAsync(SelectedExpenseId);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                await PrepareData();
                return Page();
            }
            return RedirectToPage($"../Expenses/Split", new { id = expenseId });
        }
        private async Task PrepareData()
        {
            CategoryList = await _subCategoryService.GetSubCategoryDropdownListAsync();
            PageNumber = ExpenseReviewResponse != null ? ExpenseReviewResponse.PageNumber : PageNumber == 0 ? 1 : PageNumber;

            ExpenseReviewResponse = await _expenseReviewService.GetExpenseReviewsAsync(new ExpenseReviewRequest() { PageNumber = PageNumber });
        }
    }
}