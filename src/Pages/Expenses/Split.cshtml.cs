using CashTrack.Common;
using CashTrack.Models.ExpenseModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.ExpenseService;
using CashTrack.Services.MerchantService;
using CashTrack.Services.SubCategoryService;
using CashTrack.Services.UserService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
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
        private readonly IUserService _userService;

        public SplitModel(IExpenseService expenseService, 
            ISubCategoryService subCategoryService, 
            IUserService userService) => (_expenseService, _subCategoryService, _userService) = (expenseService, subCategoryService, userService);

        [BindProperty]
        public List<ExpenseSplit> ExpenseSplits { get; set; } = new List<ExpenseSplit>();
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
        public async Task<IActionResult> OnGet(int id, int? Split, decimal? Tax, string ReturnUrl)
        {
            return await PrepareAndRenderPage(id, Split, Tax, ReturnUrl);
        }
        public async Task<IActionResult> OnPost(List<ExpenseSplit> expenseSplits, string ReturnUrl)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var id = Convert.ToInt32(HttpContext.GetRouteData().Values["Id"]);
            var originalExpense = await _expenseService.GetExpenseByIdAsync(id);
            if (originalExpense == null)
            {
                InfoMessage = $"Unable to find expense with Id {id}";
                return LocalRedirect("./Index");
            }
            var totalAmount = expenseSplits.Sum(x => x.Amount);
            var originalAmount = originalExpense.Amount;
            if (totalAmount != originalAmount)
            {
                ModelState.AddModelError("", "Original amount was not split evenly among the new expenses.");
                return await PrepareAndRenderPage(id, expenseSplits.Count, expenseSplits.FirstOrDefault().Tax, ReturnUrl);
            }
            int expenseId = 0;
            var deleteSuccess = false;
            try
            {
                foreach (var expenseSplit in expenseSplits)
                {
                    if (expenseSplit.Amount > 0)
                    {
                        expenseId = await _expenseService.CreateExpenseFromSplitAsync(expenseSplit);
                    }
                }
                deleteSuccess = await _expenseService.DeleteExpenseAsync(int.Parse(RouteData.Values["Id"].ToString()));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return await PrepareAndRenderPage(id, expenseSplits.Count, expenseSplits.FirstOrDefault().Tax, ReturnUrl);
            }
            if (expenseId > 0 && deleteSuccess)
            {
                TempData["SuccessMessage"] = "Sucessfully Split the Expense!";
                return LocalRedirect(Url.Content(ReturnUrl) ?? "~/Expenses/Index");
            }
            else
            {   
                InfoMessage = "There was an error splitting the expense";
                return LocalRedirect(Url.Content(ReturnUrl) ?? "~/Expenses/Index");
            }
        }
        private async Task<IActionResult> PrepareAndRenderPage(int id, int? Split, decimal? Tax, string ReturnUrl)
        {
            var originalExpense = await _expenseService.GetExpenseByIdAsync(id);
            if (originalExpense == null)
            {
                InfoMessage = $"Unable to find expense with Id {id}";
                return LocalRedirect("./Index");    
            }
            SubCategoryId = originalExpense.SubCategoryId;
            var categories = await _subCategoryService.GetSubCategoryDropdownListAsync();
            SubCategories = new SelectList(categories, nameof(SubCategoryDropdownSelection.Id), nameof(SubCategoryDropdownSelection.Category), originalExpense.SubCategoryId);
            Id = id;
            Total = originalExpense.Amount;
            Date = originalExpense.Date;
            Merchant = originalExpense.Merchant;
            this.Tax = Tax ?? await _userService.GetDefaultTax(User.Identity.Name);
            this.Split = Split ?? 2;
            for (int i = 0; i < this.Split; i++)
            {
                ExpenseSplits.Add(new ExpenseSplit()
                {
                    SubCategoryId = SubCategoryId,
                    Amount = 0.00M,
                    Merchant = originalExpense.Merchant
                });
            };
            SplitOptions = new SelectList(Enumerable.Range(2, 7));
            this.ReturnUrl = ReturnUrl ?? "~/Expenses/Index";
            return Page();
        }
    }
}
