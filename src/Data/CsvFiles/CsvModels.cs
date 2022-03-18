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
            public DateTimeOffset? date
            {
                get => _date;
                set
                {
                    if (value != null)
                        _date = value.Value.ToUniversalTime();
                }
            }
            public decimal amount { get; set; }
            public int? categoryid { get; set; }
            public int? merchantid { get; set; }
            public bool? exclude_from_statistics { get; set; }
            public string? notes { get; set; }
        }
        public class CsvExpenseMainCategory
        {
            public int Id { get; set; }
            public string? main_category_name { get; set; }
        }
        public class CsvExpenseSubCategory
        {
            public int Id { get; set; }
            public string? sub_category_name { get; set; }
            public int main_categoryid { get; set; }
            public bool in_use { get; set; }
        }
        public class CsvMerchant
        {
            public int Id { get; set; }
            public string? name { get; set; }
            public bool suggest_on_lookup { get; set; }
            public string? city { get; set; }
            public string? state { get; set; }
            public string? notes { get; set; }
            public bool is_online { get; set; }

        }
        public class CsvIncomeCategory
        {
            public int Id { get; set; }
            public string? category { get; set; }
            public string? description { get; set; }
            public bool in_use { get; set; }
        }
        public class CsvIncomeSource
        {
            public int Id { get; set; }
            public string? source { get; set; }
            public string? description { get; set; }
            public bool in_use { get; set; }
        }
        public class CsvIncome
        {
            private DateTimeOffset? _date;
            public int Id { get; set; }
            public DateTimeOffset? date
            {
                get => _date;
                set
                {
                    if (value != null)
                        _date = value.Value.ToUniversalTime();
                }
            }
            public decimal amount { get; set; }
            public int categoryid { get; set; }
            public int sourceid { get; set; }
            public string? notes { get; set; }
            public bool is_refund { get; set; }
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