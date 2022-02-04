using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashTrack.Data.Entities
{
    [Table("income_categories")]
    public class IncomeCategories : IEntity
    {
        public int id { get; set; }
        [StringLength(50)]
        [Required]
        public string category { get; set; }
        public string description { get; set; }
        public bool in_use { get; set; }

    }
}
