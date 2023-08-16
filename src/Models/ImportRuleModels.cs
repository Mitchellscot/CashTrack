using CashTrack.Models.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace CashTrack.Models.ImportRuleModels
{
    public enum RuleType
    {
        Assignment,
        Filter
    }
    public class ImportRuleRequest : TransactionRequest
    {
        public ImportRuleOrderBy OrderBy { get; set; }
        public bool Reversed { get; set; }
    }
    public class ImportRuleResponse : PaginationResponse<ImportRuleListItem>
    {
        public ImportRuleResponse(int pageNumber, int pageSize, int totalCount, List<ImportRuleListItem> listItems) : base(pageNumber, pageSize, totalCount, listItems) { }
    }
    public class ImportRuleListItem
    {
        public int Id { get; set; }
        public string RuleType { get; set; }
        public string FileType { get; set; }
        public string TransactionType { get; set; }
        public string Rule { get; set; }
        public string MerchantSource { get; set; }
        public int? MerchantSourceId { get; set; }
        public string Category { get; set; }
        public int? CategoryId { get; set; }
    }
    public class AddEditImportRule
    {
        public int? Id { get; set; }
        public RuleType RuleType { get; set; }
        public string FileType { get; set; }
        public TransactionType TransactionType { get; set; }
        public string Rule { get; set; }
        public int? MerchantSourceId { get; set; }
        public int? CategoryId { get; set; }
    }
    public class AddEditImportRuleModal : AddEditImportRule
    {
        public bool IsEdit { get; set; }
        public string Returnurl { get; set; }
        public List<string> FileTypes { get; set; }
        public SelectList SubCategoryList { get; set; }
        public SelectList IncomeCategoryList { get; set; }
        public SelectList MerchantList { get; set; }
        public SelectList SourceList { get; set; }
    }
}