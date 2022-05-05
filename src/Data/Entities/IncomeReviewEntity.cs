using CashTrack.Data.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashTrack.Data.Entities
{

    [Table("IncomeReview")]
    public class IncomeReviewEntity : Transactions
    {
        public int? SuggestedCategoryId { get; set; }
        public IncomeCategoryEntity SuggestedCategory { get; set; }
        public int? SuggestedSourceId { get; set; }
        public IncomeSourceEntity SuggestedSource { get; set; }
        public bool IsReviewed { get; set; }
    }
}
