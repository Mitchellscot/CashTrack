using CashTrack.Models.Common;
using CashTrack.Models.ExpenseModels;
using CashTrack.Models.IncomeModels;
using CashTrack.Services.IncomeService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;

namespace CashTrack.Pages.Income
{
    public class IndexModel : PageModel
    {
        private readonly IIncomeService _incomeService;

        public IndexModel(IIncomeService incomeService)
        {
            _incomeService = incomeService;
        }
        [TempData]
        public string Message { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Q { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public int Query { get; set; }
        public string InputType { get; set; }
        public IncomeResponse IncomeResponse { get; set; }
        public SelectList QueryOptions { get; set; }

        public async Task<IActionResult> OnGet(string Q, int Query, int PageNumber)
        {
            PrepareForm(Query);
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
        private void PrepareForm(int query)
        {
            if (IncomeResponse != null)
            {
                PageNumber = IncomeResponse.PageNumber;
            }
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
