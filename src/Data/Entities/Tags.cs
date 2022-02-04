using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashTrack.Data.Entities
{
    [Table("tags")]
    public class Tags : IEntity
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string tag_name { get; set; }
        public ICollection<ExpenseTags> expense_tags { get; set; }
    }
}