using CashTrack.Common;
using CashTrack.Models.ExpenseModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.ExpenseService;
using CashTrack.Services.MerchantService;
using CashTrack.Services.SubCategoryService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashTrack.Pages.Expenses
{
    public class SplitModel : PageModelBase
    {
        private readonly IExpenseService _expenseService;
        private readonly ISubCategoryService _subCategoryService;
        private readonly IMerchantService _merchantService;
        private IOptions<AppSettingsOptions> _appSettings;

        public SplitModel(IExpenseService expenseService, ISubCategoryService subCategoryService, IMerchantService merchantService, IOptions<AppSettingsOptions> appSettings) => (_expenseService, _subCategoryService, _merchantService, _appSettings) = (expenseService, subCategoryService, merchantService, appSettings);

        [BindProperty]
        public List<ExpenseSplit> ExpenseSplits { get; set; }
        public decimal Total { get; set; }
        public DateTime Date { get; set; }
        public string Merchant { get; set; }
        [BindProperty]
        public int Split { get; set; }
        [BindProperty]
        public decimal Tax { get; set; }
        public int Id { get; set; }
        public int SubCategoryId { get; set; }
        public SelectList SplitOptions { get; set; }
        public SelectList SubCategories { get; set; }
        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; }
        public async Task<IActionResult> OnGet(int id, int? Split, decimal? Tax, string? ReturnUrl)
        {
            var originalExpense = await _expenseService.GetExpenseByIdAsync(id);
            if (originalExpense == null)
            {
                TempData["Message"] = $"Unable to find expense with Id {id}";
                return LocalRedirect("./Index");
            }
            SubCategoryId = originalExpense.SubCategoryId;
            var categories = await _subCategoryService.GetSubCategoryDropdownListAsync();
            SubCategories = new SelectList(categories, nameof(SubCategoryDropdownSelection.Id), nameof(SubCategoryDropdownSelection.Category), originalExpense.SubCategoryId);
            Id = id;
            Total = originalExpense.Amount;
            Date = originalExpense.Date;
            Merchant = originalExpense.Merchant;
            this.Tax = Tax ?? _appSettings.Value.DefaultTax;
            this.Split = Split ?? 2;
            SplitOptions = new SelectList(Enumerable.Range(2, 7));
            this.ReturnUrl = ReturnUrl ?? "~/Expenses/Index";
            return Page();
        }
        public async Task<IActionResult> OnPost(List<ExpenseSplit> expenseSplits, string? ReturnUrl)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                foreach (var expenseSplit in expenseSplits)
                {
                    if (expenseSplit.Amount > 0)
                    {
                        var expenseId = await _expenseService.CreateExpenseFromSplitAsync(expenseSplit);
                    }
                }
                var deleteSuccess = await _expenseService.DeleteExpenseAsync(int.Parse(RouteData.Values["Id"].ToString()));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Page();
            }
            TempData["Message"] = "Sucessfully Split the Expense!";
            return LocalRedirect(ReturnUrl ?? "~/Expenses/Index");
        }
    }
}
