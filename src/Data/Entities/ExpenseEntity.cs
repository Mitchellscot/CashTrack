#nullable enable
using CashTrack.Data.Entities.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashTrack.Data.Entities
{
    [Table("Expenses")]
    public class ExpenseEntity : Transactions
    {
        [StringLength(50)]
        public int? MerchantId { get; set; }
        public MerchantEntity? Merchant { get; set; }
        [StringLength(255)]
        public int? CategoryId { get; set; }
        public SubCategoryEntity? Category { get; set; }
        public bool ExcludeFromStatistics { get; set; }
        public string? RefundNotes { get; set; }
        public ICollection<ExpenseTags>? ExpenseTags { get; set; }
    }
}
