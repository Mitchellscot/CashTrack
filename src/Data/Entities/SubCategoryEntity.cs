using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashTrack.Data.Entities
{
    [Table("SubCategories")]
    public class SubCategoryEntity : IEntity
    {
        public int Id { get; set; }
        [StringLength(50)]
        [Required]
        public string Name { get; set; }
        public int MainCategoryId { get; set; }
        public MainCategoryEntity MainCategory { get; set; }
        public string Notes { get; set; }
        public bool InUse { get; set; } = true;
        public ICollection<ExpenseEntity> Expenses { get; set; }
    }
}
