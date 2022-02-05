#nullable enable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashTrack.Data.Entities.Common
{
    public abstract class Transactions : IEntity
    {
        private DateTimeOffset _date;
        [Required]
        public DateTimeOffset date
        {
            get => _date;
            set => _date = value.ToUniversalTime();
        }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal amount { get; set; }
        [StringLength(255)]
        public string? notes { get; set; }

        public int Id { get; set; }
    }
}
