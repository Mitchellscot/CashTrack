using CashTrack.Models.Common;
using System.Collections.Generic;

namespace CashTrack.Models.ExpenseReviewModels;

public class ExpenseReviewRequest : PaginationRequest
{
}
public class ExpenseReviewResponse : PaginationResponse<ExpenseReviewListItem>
{
    public ExpenseReviewResponse(int pageNumber, int pageSize, int count, IEnumerable<ExpenseReviewListItem> listItems) : base(pageNumber, pageSize, count, listItems)
    {
    }
}

public class ExpenseReviewListItem : Transaction
{
    public string Notes { get; set; }
    public int? SuggestedCategoryId { get; set; }
    public string SuggestedCategory { get; set; }
    public int? SuggestedMerchantId { get; set; }
    public string SuggestedMerchant { get; set; }
}
