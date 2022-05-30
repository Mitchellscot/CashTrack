using CashTrack.Common.Exceptions;
using CashTrack.Models.Common;
using CashTrack.Models.ExpenseModels;
using CashTrack.Models.IncomeCategoryModels;
using CashTrack.Models.IncomeModels;
using CashTrack.Models.IncomeSourceModels;
using CashTrack.Pages.Shared;
using CashTrack.Services.IncomeCategoryService;
using CashTrack.Services.IncomeService;
using CashTrack.Services.IncomeSourceService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;

namespace CashTrack.Pages.Incomes
{
    public class IndexModel : PageModelBase
    {
        private readonly IIncomeSourceService _sourceService;
        private readonly IIncomeService _incomeService;
        private readonly IIncomeCategoryService _categoryService;

        public IndexModel(IIncomeService incomeService, IIncomeSourceService sourceService, IIncomeCategoryService categoryService)
        {
            _sourceService = sourceService;
            _incomeService = incomeService;
            _categoryService = categoryService;
        }
        [BindProperty(SupportsGet = true)]
        public string Q { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public int Query { get; set; }
        public string InputType { get; set; }
        public IncomeResponse IncomeResponse { get; set; }
        public SelectList QueryOptions { get; set; }
        public IncomeCategoryDropdownSelection[] CategoryList { get; set; }
        [BindProperty]
        public AddEditIncomeModal Income { get; set; }

        public async Task<IActionResult> OnGet(string Q, int Query, int PageNumber)
        {
            await PrepareForm(Query);
            if (Query == 0 && Q != null)
            {
                IncomeResponse = await _incomeService.GetIncomeAsync(new IncomeRequest() { DateOptions = DateOptions.SpecificDate, BeginDate = DateTimeOffset.Parse(Q), PageNumber = PageNumber });
                return Page();
            }
            if (Query == 1 && Q != null)
            {
                IncomeResponse = await _incomeService.GetIncomeAsync(new IncomeRequest() { DateOptions = DateOptions.SpecificMonthAndYear, BeginDate = DateTimeOffset.Parse(Q), PageNumber = PageNumber });
                return Page();
            }
            if (Query == 2 && Q != null)
            {
                IncomeResponse = await _incomeService.GetIncomeAsync(new IncomeRequest() { DateOptions = DateOptions.SpecificQuarter, BeginDate = DateTime.Parse(Q), PageNumber = PageNumber });
                return Page();
            }
            if (Query == 3 && Q != null)
            {
                int year;
                if (int.TryParse(Q, out year))
                {
                    IncomeResponse = await _incomeService.GetIncomeAsync(new IncomeRequest() { DateOptions = DateOptions.SpecificYear, BeginDate = new DateTime(year, 1, 1), PageNumber = PageNumber });
                    return Page();
                }
                else
                {
                    ModelState.AddModelError("", "Unable to convert the given year. Please try again.");
                    return Page();
                }
            }
            if (Query == 4 && Q != null)
            {
                decimal amount;
                if (Decimal.TryParse(Q, out amount))
                {
                    IncomeResponse = await _incomeService.GetIncomeByAmountAsync(new AmountSearchRequest() { Query = amount, PageNumber = PageNumber });
                    return Page();
                }
                else
                {
                    ModelState.AddModelError("", "Unable to convert the given amount. Please try again.");
                    return Page();
                }
            }
            if (Query == 5 && Q != null)
            {
                IncomeResponse = await _incomeService.GetIncomeByNotesAsync(new IncomeRequest() { Query = Q, PageNumber = PageNumber });
                return Page();
            }
            if (Query == 6 && Q != null)
            {
                IncomeResponse = await _incomeService.GetIncomeBySourceAsync(new IncomeRequest() { Query = Q, PageNumber = PageNumber });
                return Page();
            }
            if (Query == 7 && Q != null)
            {
                IncomeResponse = await _incomeService.GetIncomeByIncomeCategoryIdAsync(new IncomeRequest() { Query = Q, PageNumber = PageNumber });
                return Page();
            }
            if (Query == 8)
            {
                IncomeResponse = await _incomeService.GetIncomeAsync(new IncomeRequest() { DateOptions = DateOptions.CurrentMonth, PageNumber = PageNumber });
                return Page();
            }
            if (Query == 9)
            {
                IncomeResponse = await _incomeService.GetIncomeAsync(new IncomeRequest() { DateOptions = DateOptions.CurrentQuarter, PageNumber = PageNumber });
                return Page();
            }
            if (Query == 10)
            {
                IncomeResponse = await _incomeService.GetIncomeAsync(new IncomeRequest() { DateOptions = DateOptions.CurrentYear, PageNumber = PageNumber });
                return Page();
            }
            if (Query == 11)
            {
                IncomeResponse = await _incomeService.GetIncomeAsync(new IncomeRequest() { DateOptions = DateOptions.Last30Days, PageNumber = PageNumber });
                return Page();
            }

            IncomeResponse = await _incomeService.GetIncomeAsync(new IncomeRequest() { DateOptions = DateOptions.All, PageNumber = this.PageNumber });
            return Page();
        }
        public async Task<IActionResult> OnPostAddEditIncomeModal()
        {

            if (!ModelState.IsValid)
            {
                await PrepareForm(Query);
                return Page();
            }
            try
            {
                var incomeId = 0;
                //even if you don't click the refund switch, if the category is refund then we redirect to refund page.
                var isRefund = await _categoryService.CheckIfIncomeCategoryIsRefund(Income.CategoryId);
                if (Income.IsRefund == false && isRefund == true)
                {
                    Income.IsRefund = isRefund;
                }

                if (Income.CreateNewSource && !string.IsNullOrEmpty(Income.Source))
                {
                    var incomeSourceCreationSuccess = await _sourceService.CreateIncomeSourceAsync(new IncomeSource() { Name = Income.Source, SuggestOnLookup = true });
                }
                if (Income.Amount > 0)
                {
                    incomeId = Income.IsEdit ? await _incomeService.UpdateIncomeAsync(Income) : await _incomeService.CreateIncomeAsync(Income);
                }
                if (Income.IsRefund || isRefund)
                {
                    return RedirectToPage("./Refund", new { id = incomeId });
                }
                TempData["SuccessMessage"] = Income.IsEdit ? "Sucessfully updated the Income!" : "Sucessfully added new Income!";
                return RedirectToPage("./Index", new { Query = Query, Q = Q, PageNumber = PageNumber == 0 ? 1 : PageNumber });
            }
            catch (CategoryNotFoundException)
            {
                ModelState.AddModelError("", "Please select a category and try again");
                await PrepareForm(Query);
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                await PrepareForm(Query);
                return Page();
            }
        }
        public async Task<IActionResult> OnPostDelete(int incomeId, int query, string q, int pageNumber)
        {
            var success = await _incomeService.DeleteIncomeAsync(incomeId);
            if (!success)
            {
                ModelState.AddModelError("", "Unable to delete the Income");
                return Page();
            }
            TempData["SuccessMessage"] = "Sucessfully deleted Income!";
            return RedirectToPage("./Index", new { Query = query, Q = q, PageNumber = pageNumber });
        }
        private async Task PrepareForm(int query)
        {
            PageNumber = IncomeResponse != null ? IncomeResponse.PageNumber : PageNumber == 0 ? 1 : PageNumber;
            CategoryList = await _categoryService.GetIncomeCategoryDropdownListAsync();
            QueryOptions = new SelectList(IncomeQueryOptions.GetAll, "Key", "Value", query);

            switch (query)
            {
                case 0:
                    InputType = "date";
                    break;
                case 1 or 2:
                    InputType = "month";
                    break;
                case 3 or 4:
                    InputType = "number";
                    break;
                case 5 or 6 or 7:
                    InputType = "text";
                    break;
                default:
                    InputType = "date";
                    break;
            }
        }
    }
}
