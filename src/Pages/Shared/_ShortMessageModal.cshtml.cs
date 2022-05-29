using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CashTrack.Pages.Shared
{
    public class _ShortMessageModal : PageModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
    }
}
