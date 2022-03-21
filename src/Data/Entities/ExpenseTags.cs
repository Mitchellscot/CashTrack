using System.ComponentModel.DataAnnotations.Schema;

namespace CashTrack.Data.Entities
{
    [Table("ExpenseTags")]
    public class ExpenseTags
    {
        public int ExpenseId { get; set; }
        public ExpenseEntity Expense { get; set; }
        public int TagId { get; set; }
        public TagEntity Tag { get; set; }
    }
}
