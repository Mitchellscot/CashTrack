using CashTrack.Data.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashTrack.Data.Entities
{
    [Table("expenses")]
    public class Expenses : Transactions
    {
        [StringLength(50)]
        public int? merchantid { get; set; }
        public Merchants? merchant { get; set; }
        [StringLength(255)]
        public int? categoryid { get; set; }
        public SubCategories? category { get; set; }
        public bool exclude_from_statistics { get; set; }
        public ICollection<ExpenseTags>? expense_tags { get; set; }
    }
}
