using CashTrack.Models.ImportCsvModels;

namespace CashTrack.Models.ImportRuleModels
{
    public enum TransactionType
    {
        Expense,
        Income
    }
    public enum RuleType 
    { 
        Assignment,
        Filter
    }
    public enum CsvFileType
    {
        Bank,
        Credit,
        Other
    }
    public class ImportRuleAssignmentModel
    {
        public int? Id { get; set; }
        public RuleType RuleType = RuleType.Assignment;
        public CsvFileType FileType { get; set; }
        public TransactionType TransactionType { get; set; }
        public string Rule { get; set; }
        public int? MerchantSourceId { get; set; }
        public int CategoryId { get; set; }
    }
    public class ImportRuleFilterModel
    {
        public int? Id { get; set; }
        public RuleType RuleType = RuleType.Filter;
        public CsvFileType FileType { get; set; }
        public TransactionType TransactionType { get; set; }
        public string Rule { get; set; }
    }
}

//Id	Transaction	Rule	MerchantSourceId	CategoryId
