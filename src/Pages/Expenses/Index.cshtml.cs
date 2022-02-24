using CashTrack.Models.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CashTrack.Pages.Expenses
{
    public class Index : PageModel
    {
        [BindProperty]
        public DateOptions QueryType { get; set; }
        public void OnGet()
        {
        }
    }
}
