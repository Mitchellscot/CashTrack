using CashTrack.Models.ExpenseModels;
using CashTrack.Models.ExpenseReviewModels;
using CashTrack.Models.ImportCsvModels;
using CashTrack.Models.MainCategoryModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.ExpenseReviewService;
using CashTrack.Services.ExpenseService;
using CashTrack.Services.ImportProfileService;
using CashTrack.Services.ImportService;
using CashTrack.Services.MainCategoriesService;
using CashTrack.Services.MerchantService;
using CashTrack.Services.SubCategoryService;
using CashTrack.Services.UserService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        private readonly IMainCategoriesService _mainCategoryService;
        private readonly IUserService _userService;
        private readonly IImportProfileService _profileService;

        public ExpensesModel(IExpenseReviewService expenseReviewService, ISubCategoryService subCategoryService, IExpenseService expenseService, IMerchantService merchantService, IImportService importService, IMainCategoriesService mainCategoryService, IUserService userService, IImportProfileService profileService)
        {
            _merchantService = merchantService;
            _expenseService = expenseService;
            _subCategoryService = subCategoryService;
            _expenseReviewService = expenseReviewService;
            _importService = importService;
            _mainCategoryService = mainCategoryService;
            _userService = userService;
            _profileService = profileService;
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
        public List<string> FileTypes { get; set; }
        public MainCategoryDropdownSelection[] MainCategoryList { get; set; }
        public async Task<IActionResult> OnGet(int pageNumber)
        {
            return await PrepareAndRenderPage();
        }
        public async Task<IActionResult> OnPostImportCsv(int pageNumber)
        {
            if (Import.File == null)
            {
                ModelState.AddModelError("", "Please choose a csv file and try again.");
                return await PrepareAndRenderPage();
            }

            if (string.IsNullOrEmpty(Import.File.ContentType) || Import.File.ContentType != "text/csv")
            {
                ModelState.AddModelError("", "File must be a CSV file.");
                return await PrepareAndRenderPage();
            }
            var result = await _importService.ImportTransactions(Import);
            if (result.ToString().Contains("Added"))
            {
                int id = 1;
                var value = this.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                if (value != null && value.Value is string)
                    id = Convert.ToInt32(value.Value);
                SuccessMessage += result;
                var updateSuccess = await _userService.UpdateLastImportDate(id);
                if (!updateSuccess)
                    InfoMessage = "Unable to update the last import date.";
            }
            else
                InfoMessage += result;

            return LocalRedirect(Url.Content(Import.ReturnUrl));
        }
        public async Task<IActionResult> OnPostExpenseAdd()
        {
            if (SelectedExpense.SubCategoryId == 0)
            {
                ModelState.AddModelError("", "Expense must have an assigned category");
                return await PrepareAndRenderPage();
            }
            if (!string.IsNullOrEmpty(SelectedExpense.Merchant) && !(await _merchantService.GetAllMerchantNames()).Any(x => x == SelectedExpense.Merchant))
            {
                ModelState.AddModelError("", $"Merchant name {SelectedExpense.Merchant} doesn't exist. Try adding a new merchant and try again.");
                return await PrepareAndRenderPage();
            }

            try
            {
                var expenseId = await _expenseService.CreateExpenseAsync(SelectedExpense);
                var expenseReviewId = await _expenseReviewService.SetExpenseReviewToIgnoreAsync(SelectedExpenseId);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return await PrepareAndRenderPage();
            }

            SuccessMessage = "Sucessfully Added A New Expense!";
            return RedirectToPage("../Import/Expenses", new { pageNumber = PageNumber });
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
                return await PrepareAndRenderPage();
            }
            SuccessMessage = "Successfully Removed the Expense!";
            return RedirectToPage("../Import/Expenses", new { pageNumber = pageNumber });
        }
        public async Task<IActionResult> OnPostSplitExpense(string ReturnUrl)
        {
            if (SelectedExpense.SubCategoryId == 0)
            {
                ModelState.AddModelError("", "Expense must have an assigned category before splitting");
                return await PrepareAndRenderPage();
            }
            if (!string.IsNullOrEmpty(SelectedExpense.Merchant) && !(await _merchantService.GetAllMerchantNames()).Any(x => x == SelectedExpense.Merchant))
            {
                return await PrepareAndRenderPage();
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
                return await PrepareAndRenderPage();
            }
            return RedirectToPage("../Expenses/Split", new { id = expenseId, ReturnUrl = ReturnUrl });
        }
        private async Task<IActionResult> PrepareAndRenderPage()
        {
            MainCategoryList = await _mainCategoryService.GetMainCategoriesForDropdownListAsync();
            CategoryList = await _subCategoryService.GetSubCategoryDropdownListAsync();
            PageNumber = ExpenseReviewResponse != null ? ExpenseReviewResponse.PageNumber : PageNumber == 0 ? 1 : PageNumber;
            FileTypes = await _profileService.GetImportProfileNames();
            ExpenseReviewResponse = await _expenseReviewService.GetExpenseReviewsAsync(new ExpenseReviewRequest() { PageNumber = PageNumber });
            return Page();
        }
    }
}