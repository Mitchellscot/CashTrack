using CashTrack.Models.Common;
using System.Collections.Generic;

namespace CashTrack.Models.IncomeReviewModels;

public class IncomeReviewRequest : PaginationRequest
{
}
public class IncomeReviewResponse : PaginationResponse<IncomeReviewListItem>
{
    public IncomeReviewResponse(int pageNumber, int pageSize, int count, IEnumerable<IncomeReviewListItem> listItems) : base(pageNumber, pageSize, count, listItems)
    {
    }
}

public class IncomeReviewListItem : Transaction
{
    public string Notes { get; set; }
    public int SuggestedCategoryId { get; set; }
    public string SuggestedCategory { get; set; }
    public int SuggestedSourceId { get; set; }
    public string SuggestedSource { get; set; }
}
