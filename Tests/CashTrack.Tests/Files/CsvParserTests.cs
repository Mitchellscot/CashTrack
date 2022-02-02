using CashTrack.Data.CsvFiles;
using Microsoft.Extensions.Configuration;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CashTrack.Tests.Files
{
    public class CsvParserTests
    {
        private readonly string _filePath;

        public CsvParserTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Test.json")
                .Build();
            var appSettings = config.GetSection("CsvFileDirectory");
            _filePath = appSettings.Value;

        }
        [Fact]
        public void CanParseFromCsvFile()
        {
            var incomeFile = CsvParser.ProcessIncomeFile(_filePath + "incomes.csv");
            incomeFile.ShouldNotBeEmpty();
            var usersFile = CsvParser.ProcessUserFile(_filePath + "users.csv");
            usersFile.ShouldNotBeEmpty();
            var incomeSourcesFile = CsvParser.ProcessIncomeSourceFile(_filePath + "income-sources.csv");
            incomeSourcesFile.ShouldNotBeEmpty();
            var incomeCategoryFile = CsvParser.ProcessIncomeCategoryFile(_filePath + "income-categories.csv");
            incomeCategoryFile.ShouldNotBeEmpty();
            var expenseFile = CsvParser.ProcessExpenseFile(_filePath + "expenses.csv");
            expenseFile.ShouldNotBeEmpty();
            var merchantsFile = CsvParser.ProcessMerchantFile(_filePath + "merchants.csv");
            merchantsFile.ShouldNotBeEmpty();
            var mainCategoryFile = CsvParser.ProcessMainCategoryFile(_filePath + "main-categories.csv");
            mainCategoryFile.ShouldNotBeEmpty();
            var subCategoryFile = CsvParser.ProcessSubCategoryFile(_filePath + "sub-categories.csv");
            subCategoryFile.ShouldNotBeEmpty();
        }
    }
}
