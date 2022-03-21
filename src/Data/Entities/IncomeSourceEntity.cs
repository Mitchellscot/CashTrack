using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashTrack.Data.Entities
{
    [Table("IncomeSources")]
    public class IncomeSourceEntity : IEntity
    {
        public int Id { get; set; }
        [StringLength(100)]
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool InUse { get; set; }
    }
}
