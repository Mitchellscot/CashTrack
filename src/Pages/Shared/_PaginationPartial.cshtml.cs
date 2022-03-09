using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CashTrack.Pages.Shared
{
    public class _PaginationPartialModel : PageModel
    {
        public string PathLink { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public int Query { get; set; }
        public string q { get; set; }
        public string q2 { get; set; }
        public bool IsExpensePage { get; set; }
    }
}
