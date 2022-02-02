#nullable enable
using CashTrack.Data.Entities.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashTrack.Data.Entities
{
    [Table("incomes")]
    public class Incomes : Transactions
    {
        public int? categoryid { get; set; }
        public IncomeCategories? category { get; set; }
        public int? sourceid { get; set; }
        public IncomeSources? source { get; set; }
    }
}
