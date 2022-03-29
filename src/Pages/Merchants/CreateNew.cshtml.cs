using CashTrack.Models.ExpenseModels;
using CashTrack.Models.MerchantModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;

namespace CashTrack.Pages.Merchants
{
    public class CreateNewModel : PageModel
    {
        [TempData]
        public string ExpenseDate { get; set; }
        [TempData]
        public string Amount { get; set; }
        [TempData]
        public string ExpenseMerchant { get; set; }
        [TempData]
        public string SubCategory { get; set; }
        [TempData]
        public string Notes { get; set; }
        public bool PrintTempData { get; set; }
        [BindProperty]
        public Expense ExpenseToAdd { get; set; }
        [BindProperty]
        public Merchant Merchant { get; set; }
        public ActionResult OnGet()
        {
            PrintTempData = CheckForTempData();
            return Page();
        }
        private bool CheckForTempData()
        {
            if (!string.IsNullOrEmpty(ExpenseDate) && !string.IsNullOrEmpty(Amount) && !string.IsNullOrEmpty(ExpenseMerchant)
                && !string.IsNullOrEmpty(SubCategory))
                return true;

            return false;
        }
    }
}
