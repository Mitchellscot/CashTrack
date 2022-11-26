using CashTrack.Data.CsvFiles;
using Shouldly;
using System.IO;
using System.Linq;
using Xunit;

namespace CashTrack.Tests.Unit
{
    public class CsvParserTests
    {
        private readonly string _csvFileDirectory;
        public CsvParserTests()
        {
            _csvFileDirectory = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.Parent!.Parent!.FullName, "ct-data", "TestData");
        }
        [Fact]
        public void Can_Parse_Users_File()
        {
            var result = CsvParser.ProcessUserFile(Path.Combine(_csvFileDirectory, "Users.csv"));
            result.FirstOrDefault()!.FirstName.ShouldBe("Test");
        }
        [Fact]
        public void Can_Parse_Main_Categories_File()
        {
            var result = CsvParser.ProcessMainCategoryFile(Path.Combine(_csvFileDirectory, "MainCategories.csv"));
            result.Count().ShouldBe(16);
        }
        [Fact]
        public void Can_Parse_Sub_Category_File()
        {
            var result = CsvParser.ProcessSubCategoryFile(Path.Combine(_csvFileDirectory, "SubCategories.csv"));
            result.Count().ShouldBe(38);
        }
        [Fact]
        public void Can_Parse_Merchants_File()
        {
            var result = CsvParser.ProcessMerchantFile(Path.Combine(_csvFileDirectory, "Merchants.csv"));
            result.Count().ShouldBe(16);
        }
        [Fact]
        public void Can_Parse_Expenses_File()
        {
            var result = CsvParser.ProcessExpenseFile(Path.Combine(_csvFileDirectory, "Expenses.csv"));
            result.Count().ShouldBe(462);
        }
        [Fact]
        public void Can_Parse_Income_Category_File()
        {
            var result = CsvParser.ProcessIncomeCategoryFile(Path.Combine(_csvFileDirectory, "IncomeCategories.csv"));
            result.Count().ShouldBe(11);
        }
        [Fact]
        public void Can_Parse_Source_File()
        {
            var result = CsvParser.ProcessIncomeSourceFile(Path.Combine(_csvFileDirectory, "IncomeSources.csv"));
            result.Count().ShouldBe(14);
        }
        [Fact]
        public void Can_Parse_Income_File()
        {
            var result = CsvParser.ProcessIncomeFile(Path.Combine(_csvFileDirectory, "Income.csv"));
            result.Count().ShouldBe(225);
        }
        [Fact]
        public void Can_Parse_Import_Rule_File()
        {
            var result = CsvParser.ProcessImportRuleFile(Path.Combine(_csvFileDirectory, "ImportRules.csv"));
            result.Count().ShouldBe(6);
        }
        [Fact]
        public void Can_Parse_Budgets_File()
        {
            var result = CsvParser.ProcessBudgetFile(Path.Combine(_csvFileDirectory, "Budgets.csv"));
            result.Count().ShouldBe(104);
        }
    }
}
