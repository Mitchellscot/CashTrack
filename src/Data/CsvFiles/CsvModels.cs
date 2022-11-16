#nullable enable
using CashTrack.Data.Entities.Common;
using CashTrack.Models.BudgetModels;
using CashTrack.Models.ImportRuleModels;
using System;

namespace CashTrack.Data.CsvFiles
{
    public class CsvModels
    {
        //these models are used when pulling data from the CSV files and inserting it into the database
        public class CsvExpense
        {
            private DateTime? _date;
            public int Id { get; set; }
            public DateTime? Date
            {
                get => _date;
                set
                {
                    if (value != null)
                        _date = value.Value;
                }
            }
            public decimal Amount { get; set; }
            public int? CategoryId { get; set; }
            public int? MerchantId { get; set; }
            public bool? ExcludeFromStatistics { get; set; }
            public string? Notes { get; set; }
            public string? RefundNotes { get; set; }
        }
        public class CsvExpenseMainCategory
        {
            public int Id { get; set; }
            public string? Name { get; set; }
        }
        public class CsvExpenseSubCategory
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public int MainCategoryId { get; set; }
            public bool InUse { get; set; }
            public string? Notes { get; set; }
        }
        public class CsvMerchant
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public bool SuggestOnLookup { get; set; }
            public string? City { get; set; }
            public string? State { get; set; }
            public string? Notes { get; set; }
            public bool IsOnline { get; set; }

        }
        public class CsvIncomeCategory
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public string? Notes { get; set; }
            public bool InUse { get; set; }
        }
        public class CsvIncomeSource
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public bool SuggestOnLookup { get; set; }
            public string? City { get; set; }
            public string? State { get; set; }
            public string? Notes { get; set; }
            public bool IsOnline { get; set; }
        }
        public class CsvIncome
        {
            private DateTime? _date;
            public int Id { get; set; }
            public DateTime? Date
            {
                get => _date;
                set
                {
                    if (value != null)
                        _date = value.Value;
                }
            }
            public decimal Amount { get; set; }
            public int CategoryId { get; set; }
            public int SourceId { get; set; }
            public string? Notes { get; set; }
            public bool IsRefund { get; set; }
            public string? RefundNotes { get; set; }
        }
        public class CsvUser
        {
            public int Id { get; set; }
            public string? UserName { get; set; }
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? Email { get; set; }
            public string? NormalizedUserName { get; set; }
            public string? NormalizedEmail { get; set; }
            public string? PasswordHash { get; set; }
        }
        public class CsvImportRule
        {
            public int Id { get; set; }
            public RuleType RuleType { get; set; }
            public TransactionType TransactionType { get; set; }
            public CsvFileType FileType { get; set; }
            public string? Rule { get; set; }
            public int? MerchantSourceId { get; set; }
            public int? CategoryId { get; set; }
        }
        public class CsvBudget
        {
            public int Id { get; set; }
            public int Month { get; set; }
            public int Year { get; set; }
            public int Amount { get; set; }
            public int? SubCategoryId { get; set; }
            public BudgetType BudgetType { get; set; }
        }
    }
}