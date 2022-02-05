using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashTrack.Data.Entities
{
    [Table("sub_categories")]
    public class SubCategories : IEntity
    {
        public int Id { get; set; }
        [StringLength(50)]
        [Required]
        public string sub_category_name { get; set; }
        public int main_categoryid { get; set; }
        public MainCategories main_category { get; set; }
        public string notes { get; set; }
        public bool in_use { get; set; } = true;
        public ICollection<Expenses> expenses { get; set; }
    }
}
