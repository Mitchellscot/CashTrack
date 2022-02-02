#nullable enable
using System;
using System.ComponentModel.DataAnnotations;

namespace CashTrack.Data.Entities.Common
{
    public abstract class Transactions : IEntity
    {
        private DateTimeOffset _date;
        public int id { get; set; }
        [Required]
        public DateTimeOffset date
        {
            get => _date;
            set => _date = value.ToUniversalTime();
        }
        [Required]
        public decimal amount { get; set; }
        [StringLength(255)]
        public string? notes { get; set; }

    }
}
