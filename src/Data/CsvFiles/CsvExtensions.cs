using CashTrack.Models.ImportCsvModels;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace CashTrack.Data.CsvFiles
{
    public static class CsvExtensions
    {
        public static IEnumerable<CsvModels.CsvExpense> ToExpenses(this IEnumerable<string> source)
        {
            foreach (var line in source)
            {
                var columns = line.Split(',');
                yield return new CsvModels.CsvExpense()
                {
                    Id = Convert.ToInt32(columns[0]),
                    Date = DateTime.Parse(columns[1],
                    null),
                    Amount = Math.Round(Decimal.Parse(columns[2]), 2),
                    CategoryId = columns[3] == "" ? null : Convert.ToInt32(columns[3]),
                    MerchantId = columns[4] == "" ? null : Convert.ToInt32(columns[4]),
                    Notes = columns[5] == "" ? null : columns[5],
                    ExcludeFromStatistics = ParseBoolean(columns[6]),
                    RefundNotes = columns[7] == "" ? null : columns[7]
                };
            }
        }
        public static IEnumerable<CsvModels.CsvExpenseMainCategory> ToExpenseMainCategory(this IEnumerable<string> source)
        {
            foreach (var line in source)
            {
                var columns = line.Split(',');
                yield return new CsvModels.CsvExpenseMainCategory()
                {
                    Id = Convert.ToInt32(columns[0]),
                    Name = columns[1],
                };
            }
        }
        public static IEnumerable<CsvModels.CsvExpenseSubCategory> ToExpenseSubCategory(this IEnumerable<string> source)
        {
            foreach (var line in source)
            {
                var columns = line.Split(',');
                yield return new CsvModels.CsvExpenseSubCategory()
                {
                    Id = Convert.ToInt32(columns[0]),
                    Name = columns[1],
                    MainCategoryId = Convert.ToInt32(columns[2]),
                    InUse = ParseBoolean(columns[3])
                };
            }
        }
        public static IEnumerable<CsvModels.CsvMerchant> ToMerchant(this IEnumerable<string> source)
        {
            foreach (var line in source)
            {
                var columns = line.Split(',');
                yield return new CsvModels.CsvMerchant()
                {
                    Id = Convert.ToInt32(columns[0]),
                    Name = columns[1],
                    SuggestOnLookup = ParseBoolean(columns[2]),
                    City = columns[3] == "" ? null : columns[3],
                    State = columns[4] == "" ? null : columns[4],
                    IsOnline = ParseBoolean(columns[5]),
                    Notes = columns[6] == "" ? null : columns[6]
                };
            }
        }
        public static IEnumerable<CsvModels.CsvIncomeCategory> ToIncomeCategory(this IEnumerable<string> source)
        {
            foreach (var line in source)
            {
                var columns = line.Split(',');
                yield return new CsvModels.CsvIncomeCategory()
                {
                    Id = Convert.ToInt32(columns[0]),
                    Name = columns[1],
                    Notes = columns[2] == "" ? null : columns[2],
                    InUse = ParseBoolean(columns[3])
                };
            }
        }
        public static IEnumerable<CsvModels.CsvIncomeSource> ToIncomeSource(this IEnumerable<string> source)
        {
            foreach (var line in source)
            {
                var columns = line.Split(',');
                yield return new CsvModels.CsvIncomeSource()
                {
                    Id = Convert.ToInt32(columns[0]),
                    Name = columns[1],
                    Notes = columns[2] == "" ? null : columns[2],
                    SuggestOnLookup = ParseBoolean(columns[3]),
                    City = columns[4] == "" ? null : columns[4],
                    State = columns[5] == "" ? null : columns[5],
                    IsOnline = ParseBoolean(columns[6]),
                };
            }
        }
        public static IEnumerable<CsvModels.CsvIncome> ToIncome(this IEnumerable<string> source)
        {
            foreach (var line in source)
            {
                var columns = line.Split(',');
                yield return new CsvModels.CsvIncome()
                {
                    Id = Convert.ToInt32(columns[0]),
                    Date = DateTime.Parse(columns[1], null),
                    Amount = Math.Round(Convert.ToDecimal(columns[2]), 2),
                    CategoryId = Convert.ToInt32(columns[3]),
                    SourceId = Convert.ToInt32(columns[4]),
                    Notes = columns[5] == "" ? null : columns[5],
                    IsRefund = ParseBoolean(columns[6]),
                    RefundNotes = columns[7] == "" ? null : columns[7]
                };
            }
        }
        public static IEnumerable<CsvModels.CsvUser> ToUser(this IEnumerable<string> source)
        {
            foreach (var line in source)
            {
                var columns = line.Split(',');
                yield return new CsvModels.CsvUser()
                {
                    Id = Convert.ToInt32(columns[0]),
                    UserName = columns[1],
                    FirstName = columns[2],
                    LastName = columns[3],
                    Email = columns[4],
                    PasswordHash = columns[5]
                };
            }
        }
        public static IEnumerable<CsvModels.CsvImportRule> ToImportRule(this IEnumerable<string> source)
        {
            foreach (var line in source)
            {
                var columns = line.Split(',');
                yield return new CsvModels.CsvImportRule()
                {
                    Id = Convert.ToInt32(columns[0]),
                    Transaction = columns[1],
                    Rule = columns[2],
                    MerchantSourceId = columns[3] == "" ? null : Convert.ToInt32(columns[3]),
                    CategoryId = columns[4] == "" ? null : Convert.ToInt32(columns[4])
                };
            }
        }
        public static IEnumerable<OtherTransactionImport> ToTransactionImport(this IEnumerable<string> source)
        {
            //change this out for a csvhelper library
            foreach (var line in source)
            {
                var columns = line.Split(',');
                yield return new OtherTransactionImport()
                {
                    Amount = Convert.ToDecimal(columns[0]),
                    Date = DateTime.Parse(columns[1], null),
                    Notes = columns[2],
                    MerchantSourceId = columns[3] == "" ? 0 : Convert.ToInt32(columns[3]),
                    CategoryId = columns[4] == "" ? 0 : Convert.ToInt32(columns[4]),
                };
            }
        }
        private static bool ParseBoolean(string s) => s == "1";
    }
}
