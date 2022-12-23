using CashTrack.Models.BudgetModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashTrack.Data.Entities
{
    [Table("Budgets")]
    public class BudgetEntity : IEntity
    {
        public int Id { get; set; }
        [Range(1, 12)]
        public int Month { get; set; }
        [Range(2010, 2099)]
        public int Year { get; set; }
        [Range(0, int.MaxValue)]
        public int Amount { get; set; }
        public int? SubCategoryId { get; set; }
        public SubCategoryEntity SubCategory { get; set; }
        public BudgetType BudgetType { get; set; }
    }
}
