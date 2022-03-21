#nullable enable
using CashTrack.Data.Entities.Common;
using System;

namespace CashTrack.Data.CsvFiles
{
    public class CsvModels
    {
        //these models are used when pulling data from the CSV files and inserting it into the database
        public class CsvExpense
        {
            private DateTimeOffset? _date;
            public int Id { get; set; }
            public DateTimeOffset? Date
            {
                get => _date;
                set
                {
                    if (value != null)
                        _date = value.Value.ToUniversalTime();
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
            public string? Description { get; set; }
            public bool InUse { get; set; }
        }
        public class CsvIncomeSource
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public bool InUse { get; set; }
        }
        public class CsvIncome
        {
            private DateTimeOffset? _date;
            public int Id { get; set; }
            public DateTimeOffset? Date
            {
                get => _date;
                set
                {
                    if (value != null)
                        _date = value.Value.ToUniversalTime();
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
            public string? PasswordHash { get; set; }
        }
    }
}