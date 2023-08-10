#nullable enable
using CashTrack.Models.ImportRuleModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashTrack.Data.Entities
{
    [Table("ImportProfiles")]
    public class ImportProfileEntity : IEntity
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? DateColumnName { get; set; }
        [Required]
        public string? AmountColumnName { get; set; }
        [Required]
        public string? NotesColumnName { get; set; }
        //some banks have a seperate column for income
        public string? IncomeColumnName { get; set; }
        //some credit cards indicate a negative value for income
        public bool? ContainsNegativeValue { get; set; }
        public TransactionType? NegativeValueTransactionType { get; set; }
    }
}
