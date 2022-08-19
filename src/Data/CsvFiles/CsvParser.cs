using System.Collections.Generic;
using System.Linq;

namespace CashTrack.Data.CsvFiles
{
    public class CsvParser
    {
        public static List<CsvModels.CsvExpense> ProcessExpenseFile(string path)
        {
            var reader = new CsvUtility<CsvModels.CsvExpense>();
            return reader.GetEntitiesFromCSV(path).ToList();
        }
        public static List<CsvModels.CsvExpenseMainCategory> ProcessMainCategoryFile(string path)
        {
            var reader = new CsvUtility<CsvModels.CsvExpenseMainCategory>();
            return reader.GetEntitiesFromCSV(path).ToList();
        }
        public static List<CsvModels.CsvExpenseSubCategory> ProcessSubCategoryFile(string path)
        {
            var reader = new CsvUtility<CsvModels.CsvExpenseSubCategory>();
            return reader.GetEntitiesFromCSV(path).ToList();
        }
        public static List<CsvModels.CsvMerchant> ProcessMerchantFile(string path)
        {
            var reader = new CsvUtility<CsvModels.CsvMerchant>();
            return reader.GetEntitiesFromCSV(path).ToList();
        }
        public static List<CsvModels.CsvIncomeCategory> ProcessIncomeCategoryFile(string path)
        {
            var reader = new CsvUtility<CsvModels.CsvIncomeCategory>();
            return reader.GetEntitiesFromCSV(path).ToList();
        }
        public static List<CsvModels.CsvIncomeSource> ProcessIncomeSourceFile(string path)
        {
            var reader = new CsvUtility<CsvModels.CsvIncomeSource>();
            return reader.GetEntitiesFromCSV(path).ToList();
        }
        public static List<CsvModels.CsvIncome> ProcessIncomeFile(string path)
        {
            var reader = new CsvUtility<CsvModels.CsvIncome>();
            return reader.GetEntitiesFromCSV(path).ToList();
        }
        public static List<CsvModels.CsvUser> ProcessUserFile(string path)
        {
            var reader = new CsvUtility<CsvModels.CsvUser>();
            return reader.GetEntitiesFromCSV(path).ToList();
        }
        public static List<CsvModels.CsvImportRule> ProcessImportRuleFile(string path)
        {
            var reader = new CsvUtility<CsvModels.CsvImportRule>();
            return reader.GetEntitiesFromCSV(path).ToList();
        }
    }
}
