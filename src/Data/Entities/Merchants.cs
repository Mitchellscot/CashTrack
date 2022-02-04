using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashTrack.Data.Entities
{
    [Table("merchants")]
    public class Merchants : IEntity
    {
        public int id { get; set; }
        [StringLength(250)]
        [Required]
        public string name { get; set; }
        public bool suggest_on_lookup { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public bool is_online { get; set; }
        public string notes { get; set; }
        public ICollection<Expenses> expenses { get; set; }
    }
}