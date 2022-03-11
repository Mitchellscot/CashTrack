using CashTrack.Models.ExpenseModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Services.ExpenseService;
using CashTrack.Services.MerchantService;
using CashTrack.Services.SubCategoryService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace CashTrack.Pages.Expenses
{
    public class EditModel : PageModel
    {
        private readonly IExpenseService _expenseService;
        private readonly ISubCategoryService _subCategoryService;
        public IMerchantService _merchantService { get; }

        public EditModel(IExpenseService expenseService, ISubCategoryService subCategoryService, IMerchantService merchantService)
        {
            _expenseService = expenseService;
            _subCategoryService = subCategoryService;
            _merchantService = merchantService;
        }
        [BindProperty]
        public AddEditExpense ExpenseEdit { get; set; }
        public SelectList SubCategories { get; set; }
        public string InitialMainCategory { get; set; }
        [BindProperty(SupportsGet = true)]
        public int Split { get; set; }
        [BindProperty]
        public string MerchantName { get; set; }
        [BindProperty]
        public bool CreateNewMerchant { get; set; }

        public async Task<ActionResult> OnGet(int id, [FromQuery] int split)
        {
            var expense = await _expenseService.GetExpenseByIdAsync(id);
            ExpenseEdit = new AddEditExpense()
            {
                Date = expense.date,
                Amount = expense.amount,
                SubCategoryId = expense.categoryid.Value,
                MerchantId = expense.merchantid ?? expense.merchantid.Value,
                ExcludeFromStatistics = expense.exclude_from_statistics,
                Notes = expense.notes,
                Id = expense.Id
            };
            var categories = await _subCategoryService.GetAllSubCategoriesAsync();
            SubCategories = new SelectList(categories, nameof(SubCategoryListItem.Id), nameof(SubCategoryListItem.Name), expense.categoryid);
            MerchantName = expense.merchant.name ?? expense.merchant.name;
            InitialMainCategory = expense.category.main_category.main_category_name;
            return Page();
        }
        //public async Task<ActionResult> OnPost(AddEditExpense expense)
        //{ 

        //}
    }
}
