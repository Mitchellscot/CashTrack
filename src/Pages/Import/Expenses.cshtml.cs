using CashTrack.Models.ExpenseModels;
using CashTrack.Models.ExpenseReviewModels;
using CashTrack.Models.ImportCsvModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.ExpenseReviewService;
using CashTrack.Services.ExpenseService;
using CashTrack.Services.ImportService;
using CashTrack.Services.MerchantService;
using CashTrack.Services.SubCategoryService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CashTrack.Pages.Import
{
    public class ExpensesModel : PageModelBase
    {
        private readonly IMerchantService _merchantService;
        private readonly IExpenseService _expenseService;
        private readonly ISubCategoryService _subCategoryService;
        private readonly IExpenseReviewService _expenseReviewService;
        private readonly IImportService _importService;

        public ExpensesModel(IExpenseReviewService expenseReviewService, ISubCategoryService subCategoryService, IExpenseService expenseService, IMerchantService merchantService, IImportService importService)
        {
            _merchantService = merchantService;
            _expenseService = expenseService;
            _subCategoryService = subCategoryService;
            _expenseReviewService = expenseReviewService;
            _importService = importService;
        }
        public ExpenseReviewResponse ExpenseReviewResponse { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        public SubCategoryDropdownSelection[] CategoryList { get; set; }
        [BindProperty]
        public Expense SelectedExpense { get; set; }
        [BindProperty]
        public int SelectedExpenseId { get; set; }

        [BindProperty]
        public ImportModel Import { get; set; }
        public async Task<IActionResult> OnGet(int pageNumber)
        {
            await PrepareData();
            return Page();
        }
        public async Task<IActionResult> OnPostImportCsv(int pageNumber)
        {
            if (Import.File == null)
            {
                ModelState.AddModelError("", "Please choose a csv file and try again.");
                await PrepareData();
                return Page();
            }
            //For Bank and file types I have hardcoded objects to match the csv files and use a library just to make the process quicker, however it's tightly coupled to my bank and credit card file formats.
            //If for any reason I wanted to import a csv file from another institution I have a third option, but the csv
            //file has to be in a specific format.
            if (string.IsNullOrEmpty(Import.File.ContentType) || Import.File.ContentType != "text/csv")
            {
                ModelState.AddModelError("", "File must be a CSV file.");
                await PrepareData();
                return Page();
            }
            var result = await _importService.ImportTransactions(Import);
            if (result.ToString().Contains("Added"))
                SuccessMessage += result;
            else
                InfoMessage += result;

            await PrepareData();
            return LocalRedirect(Import.ReturnUrl);
        }
        public async Task<IActionResult> OnPostExpenseAdd()
        {
            if (SelectedExpense.SubCategoryId == 0)
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
            return RedirectToPage($"../Import/Expenses", new { pageNumber = PageNumber });
        }
        public async Task<IActionResult> OnPostRemoveExpense(int pageNumber)
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
            return RedirectToPage($"../Import/Expenses", new { pageNumber = pageNumber });
        }
        public async Task<IActionResult> OnPostSplitExpense(int pageNumber)
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