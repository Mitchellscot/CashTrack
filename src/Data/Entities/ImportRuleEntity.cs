#nullable enable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashTrack.Data.Entities
{
    [Table("ImportRules")]
    public class ImportRuleEntity : IEntity
    {
        public int Id { get; set; }
        [Required]
        public string? Transaction { get; set; }
        [Required]
        public string? Rule { get; set; }
        public int? MerchantSourceId { get; set; }
        public int? CategoryId { get; set; }

    }
}
