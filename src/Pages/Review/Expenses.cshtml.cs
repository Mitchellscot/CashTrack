using CashTrack.Common.Exceptions;
using CashTrack.Models.ExpenseModels;
using CashTrack.Models.ExpenseReviewModels;
using CashTrack.Models.MerchantModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Services.ExpenseReviewService;
using CashTrack.Services.ExpenseService;
using CashTrack.Services.MerchantService;
using CashTrack.Services.SubCategoryService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.PowerShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;

namespace CashTrack.Pages.Review
{
    public class ExpensesModel : PageModel
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
        [TempData]
        public string ScriptResults { get; set; }
        [TempData]
        public string Message { get; set; }
        public ExpenseReviewResponse ExpenseReviewResponse { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        [BindProperty]
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
                    ScriptResults += $"{currentStreamRecord.MessageData.ToString()} ";
                };
                ps.Commands.AddScript(@". C:\Users\Mitch\Code\CashTrack\ct-data\Scripts\Run-Transactions.ps1");
                var result = await ps.InvokeAsync().ConfigureAwait(false);
            }
            await PrepareData();
            return LocalRedirect("~/Review/Expenses");
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
                TempData["ExpenseDate"] = SelectedExpense.Date.ToString();
                TempData["Amount"] = SelectedExpense.Amount.ToString();
                TempData["Merchant"] = SelectedExpense.Merchant;
                TempData["SubCategory"] = SelectedExpense.SubCategory;
                TempData["Notes"] = SelectedExpense.Notes;
                return RedirectToPage("../Merchants/CreateNew");
            }

            if (SelectedExpense.Merchant != null && SelectedExpense.CreateNewMerchant)
            {
                var createMerchant = await _merchantService.CreateMerchantAsync(new AddEditMerchant() { Name = SelectedExpense.Merchant, SuggestOnLookup = true });
                if (!createMerchant)
                {
                    ModelState.AddModelError("", "Unable to create a new merchant. Please try again.");
                    await PrepareData();
                    return Page();
                }
                Message = $"Successfully Added A New Merchant {SelectedExpense.Merchant} And ";
            }

            try
            {
                var expenseCreationSuccess = await _expenseService.CreateExpenseAsync(SelectedExpense);
                if (!expenseCreationSuccess)
                {
                    ModelState.AddModelError("", "Unable to create the expense. Please try again.");
                    await PrepareData();
                    return Page();
                }
                var setExpenseReviewToIgnore = await _expenseReviewService.SetExpenseReviewToIgnoreAsync(SelectedExpenseId);
                if (!expenseCreationSuccess)
                {
                    ModelState.AddModelError("", "Error removing the selected expense. Please try again.");
                    await PrepareData();
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                await PrepareData();
                return Page();
            }
            Message += "Sucessfully Added New Expense!";
            return LocalRedirect("~/Review/Expenses");
        }
        private async Task PrepareData()
        {
            CategoryList = await _subCategoryService.GetSubCategoryDropdownListAsync();
            PageNumber = ExpenseReviewResponse != null ? ExpenseReviewResponse.PageNumber : PageNumber == 0 ? 1 : PageNumber;

            ExpenseReviewResponse = await _expenseReviewService.GetExpenseReviewsAsync(new ExpenseReviewRequest() { PageNumber = PageNumber });
        }
    }
}