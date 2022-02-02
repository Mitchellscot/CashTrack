using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashTrack.Data.Entities
{
    [Table("main_categories")]
    public class MainCategories : IEntity
    {
        public int id { get; set; }
        [StringLength(50)]
        [Required]
        public string main_category_name { get; set; }
        public ICollection<SubCategories> sub_categories { get; set; }
    }
}
