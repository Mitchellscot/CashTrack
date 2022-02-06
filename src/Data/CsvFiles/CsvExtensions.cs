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
                    date = DateTimeOffset.Parse(columns[1],
                    null
                    , DateTimeStyles.AdjustToUniversal),
                    amount = Math.Round(Decimal.Parse(columns[2]), 2),
                    categoryid = columns[2] == "" ? null : Convert.ToInt32(columns[3]),
                    merchantid = columns[4] == "" ? null : Convert.ToInt32(columns[4]),
                    notes = columns[3] == "" ? null : columns[5],
                    exclude_from_statistics = ParseBoolean(columns[6])
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
                    main_category_name = columns[1],
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
                    sub_category_name = columns[1],
                    main_categoryid = Convert.ToInt32(columns[2]),
                    in_use = ParseBoolean(columns[3])
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
                    name = columns[1],
                    suggest_on_lookup = ParseBoolean(columns[2]),
                    city = null,
                    state = null,
                    is_online = ParseBoolean(columns[5]),
                    notes = null
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
                    category = columns[1],
                    description = columns[2],
                    in_use = true
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
                    source = columns[1],
                    description = columns[2],
                    in_use = true
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
                    date = DateTimeOffset.Parse(columns[1], null
                    , DateTimeStyles.AdjustToUniversal),
                    amount = Math.Round(Convert.ToDecimal(columns[2]), 2),
                    categoryid = Convert.ToInt32(columns[3]),
                    sourceid = Convert.ToInt32(columns[4]),
                    notes = columns[4] == "" ? null : columns[5]
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
