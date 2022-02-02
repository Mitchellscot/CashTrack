using System.ComponentModel.DataAnnotations.Schema;

namespace CashTrack.Data.Entities
{
    [Table("expense_tags")]
    public class ExpenseTags
    {
        public int expense_id { get; set; }
        public Expenses expense { get; set; }
        public int tag_id { get; set; }
        public Tags tag { get; set; }
    }
}
