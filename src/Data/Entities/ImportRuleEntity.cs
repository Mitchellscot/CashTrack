#nullable enable
using CashTrack.Models.Common;
using CashTrack.Models.ImportRuleModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

//This entity represents two different rules
//I decided to keep them in one database table because they have similar functionality
//but do two slightly different things and I don't want to create two tables.
//One rule makes assignments and assigns either a merchant or income source and category when the notes contains
//a given string. 
//The other rule filters out expenses/income if a given transaciton
//contains a string and does not use the other two properties
namespace CashTrack.Data.Entities
{
    [Table("ImportRules")]
    public class ImportRuleEntity : IEntity
    {
        public int Id { get; set; }
        [Required]
        public RuleType RuleType { get; set; }
        [Required]
        public TransactionType TransactionType { get; set; }
        [Required]
        public string? FileType { get; set; }
        [Required]
        public string? Rule { get; set; }
        public int? MerchantSourceId { get; set; }
        public int? CategoryId { get; set; }

    }
}
