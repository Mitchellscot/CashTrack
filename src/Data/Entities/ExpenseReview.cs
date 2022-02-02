using CashTrack.Data.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashTrack.Data.Entities
{
    [Table("expense_review")]
    public class ExpenseReview : Transactions
    {
        public SubCategories suggested_category { get; set; }
        public Merchants suggested_merchant { get; set; }
        public bool is_reviewed { get; set; }
    }
}
