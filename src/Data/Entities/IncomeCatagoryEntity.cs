﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashTrack.Data.Entities
{
    [Table("IncomeCategories")]
    public class IncomeCategoryEntity : IEntity
    {
        public int Id { get; set; }
        [StringLength(50)]
        [Required]
        public string Category { get; set; }
        public string Description { get; set; }
        public bool InUse { get; set; }

    }
}
