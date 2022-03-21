using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashTrack.Data.Entities
{
    [Table("MainCategories")]
    public class MainCategoryEntity : IEntity
    {
        [StringLength(50)]
        [Required]
        public string Name { get; set; }
        public ICollection<SubCategoryEntity> SubCategories { get; set; }

        public int Id { get; set; }
    }
}
