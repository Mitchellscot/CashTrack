using CashTrack.Models.ImportCsvModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CashTrack.Data.CsvFiles
{
    public class CsvParser
    {
        public static List<CsvModels.CsvExpense> ProcessExpenseFile(string path)
        {
            return File.ReadAllLines(path).Skip(1).Where(line => line.Length > 1).ToExpenses().ToList();
        }
        public static List<CsvModels.CsvExpenseMainCategory> ProcessMainCategoryFile(string path)
        {
            return File.ReadAllLines(path).Skip(1).Where(l => l.Length > 1).ToExpenseMainCategory().ToList();
        }
        public static List<CsvModels.CsvExpenseSubCategory> ProcessSubCategoryFile(string path)
        {
            return File.ReadAllLines(path).Skip(1).Where(l => l.Length > 1).ToExpenseSubCategory().ToList();
        }
        public static List<CsvModels.CsvMerchant> ProcessMerchantFile(string path)
        {
            return File.ReadAllLines(path).Skip(1).Where(l => l.Length > 1).ToMerchant().ToList();
        }
        public static List<CsvModels.CsvIncomeCategory> ProcessIncomeCategoryFile(string path)
        {
            return File.ReadAllLines(path).Skip(1).Where(l => l.Length > 1).ToIncomeCategory().ToList();
        }
        public static List<CsvModels.CsvIncomeSource> ProcessIncomeSourceFile(string path)
        {
            return File.ReadAllLines(path).Skip(1).Where(l => l.Length > 1).ToIncomeSource().ToList();
        }
        public static List<CsvModels.CsvIncome> ProcessIncomeFile(string path)
        {
            return File.ReadAllLines(path).Skip(1).Where(l => l.Length > 1).ToIncome().ToList();
        }
        public static List<CsvModels.CsvUser> ProcessUserFile(string path)
        {
            return File.ReadAllLines(path).Skip(1).Where(l => l.Length > 1).ToUser().ToList();
        }
        public static List<CsvModels.CsvImportRule> ProcessImportRuleFile(string path)
        {
            return File.ReadAllLines(path).Skip(1).Where(l => l.Length > 1).ToImportRule().ToList();
        }
        public static List<TransactionImport> ProcessImportRuleFile(string path)
        {
            return File.ReadAllLines(path).Skip(1).Where(l => l.Length > 1).ToTransactionImport().ToList();
        }
    }
}
