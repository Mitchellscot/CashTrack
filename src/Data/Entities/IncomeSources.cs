using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashTrack.Data.Entities
{
    [Table("income_sources")]
    public class IncomeSources : IEntity
    {
        public int Id { get; set; }
        [StringLength(100)]
        [Required]
        public string source { get; set; }
        public string description { get; set; }
        public bool in_use { get; set; }
    }
}
