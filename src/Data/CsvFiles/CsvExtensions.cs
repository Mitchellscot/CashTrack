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
                    Date = DateTimeOffset.Parse(columns[1],
                    null
                    , DateTimeStyles.AdjustToUniversal),
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
                    City = null,
                    State = null,
                    IsOnline = ParseBoolean(columns[5]),
                    Notes = null
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
                    Description = columns[2],
                    InUse = true
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
                    Description = columns[2],
                    InUse = true
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
                    Date = DateTimeOffset.Parse(columns[1], null
                    , DateTimeStyles.AdjustToUniversal),
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
        private static bool ParseBoolean(string s) => s == "1";
    }
}
