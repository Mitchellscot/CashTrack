using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashTrack.Data.Entities
{
    [Table("users")]
    public class Users : IdentityUser<int>, IEntity
    {
        new public int Id { get; set; }
    }

}