using System.Collections.Generic;
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
        public string Notes { get; set; }
        public bool SuggestOnLookup { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public bool IsOnline { get; set; }
        public ICollection<IncomeEntity> Incomes { get; set; }
    }
}
