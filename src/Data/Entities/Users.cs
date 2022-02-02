using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashTrack.Data.Entities
{
    [Table("users")]
    public class Users : IEntity
    {
        public int id { get; set; }
        [StringLength(25)]
        public string first_name { get; set; }
        [StringLength(25)]
        public string last_name { get; set; }
        [Required]
        [StringLength(50)]
        public string email { get; set; }
        public string password_hash { get; set; }
    }
}