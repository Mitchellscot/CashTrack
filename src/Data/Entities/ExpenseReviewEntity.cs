using CashTrack.Data.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashTrack.Data.Entities
{
    [Table("ExpenseReview")]
    public class ExpenseReviewEntity : Transactions
    {
        public SubCategoryEntity SuggestedCategory { get; set; }
        public MerchantEntity SuggestedMerchant { get; set; }
        public bool IsReviewed { get; set; }
    }
}
