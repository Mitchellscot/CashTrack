#nullable enable
using CashTrack.Data.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashTrack.Data.Entities
{
    [Table("Income")]
    public class IncomeEntity : Transactions
    {
        public int? CategoryId { get; set; }
        public IncomeCategoryEntity? Category { get; set; }
        public int? SouceId { get; set; }
        public IncomeSourceEntity? Source { get; set; }
        public bool IsRefund { get; set; }
        public string? RefundNotes { get; set; }
    }
}
