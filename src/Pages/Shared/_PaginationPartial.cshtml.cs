
namespace CashTrack.Pages.Shared
{
    public class _PaginationPartialModel
    {
        public string PathLink { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public int Query { get; set; }
        public string q { get; set; }
        public string q2 { get; set; }
        public string q3 { get; set; } = null;
        public bool IsExpensePage { get; set; }
    }
}
