using CashTrack.Models.Common;
using CashTrack.Models.ExportModels;
using CashTrack.Repositories.ExportRepository;
using CashTrack.Repositories.MainCategoriesRepository;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Environment;

namespace CashTrack.Services.ExportService;

public interface IExportService
{
    //As Readable?
    //Task<string> ExportTransactions(ExportTransactionsRequest request);
    Task<string> ExportData(int exportFileOption);
}
public class ExportService : IExportService
{
    private readonly IExportRepository _exportRepo;

    public ExportService(IExportRepository exportRepo)
    {
        _exportRepo = exportRepo;
    }
    public async Task<string> ExportData(int exportFileOption)
    {
        var fileName = ExportFileOptions.GetAll[exportFileOption];
        var filePath = Path.ChangeExtension(Path.Combine(Path.GetTempPath(), fileName), ".csv");

        switch (exportFileOption)
        {
            case 1:
                await CreateExpensesFile(filePath);
                return filePath;
            case 2:
                await CreateImportRulesFile(filePath);
                return filePath;
            case 3:
                await CreateIncomeFile(filePath);
                return filePath;
            case 4:
                await CreateIncomeCategoriesFile(filePath);
                return filePath;
            case 5:
                await CreateIncomeSourcesFile(filePath);
                return filePath;
            case 6:
                await CreateMainCategoriesFile(filePath);
                return filePath;
            case 7:
                await CreateMerchantsFile(filePath);
                return filePath;
            case 8:
                await CreateSubCategoriesFile(filePath);
                return filePath;
            default:
                //all data
                await CreateAllDataZip();
                return filePath;
        }
    }

    private async Task CreateExpensesFile(string filePath)
    {
        var expenses = await _exportRepo.GetExpenses();

        if (expenses.Length == 0)
            return;

        WriteFile<ExpenseExport>(filePath, expenses);
    }
    private async Task CreateImportRulesFile(string filePath)
    {
        var importRules = await _exportRepo.GetImportRules();

        if (importRules.Length == 0)
            return;

        WriteFile<ImportRuleExport>(filePath, importRules);
    }
    private async Task CreateIncomeFile(string filePath)
    {
        var incomes = await _exportRepo.GetIncome();

        if (incomes.Length == 0)
            return;

        WriteFile<IncomeExport>(filePath, incomes);
    }
    private async Task CreateIncomeCategoriesFile(string filePath)
    {
        var incomeCategories = await _exportRepo.GetIncomeCategories();

        if (incomeCategories.Length == 0)
            return;

        WriteFile<IncomeCategoryExport>(filePath, incomeCategories);
    }
    private async Task CreateIncomeSourcesFile(string filePath)
    {
        var incomeSources = await _exportRepo.GetIncomeSources();

        if (incomeSources.Length == 0)
            return;

        WriteFile<IncomeSourceExport>(filePath, incomeSources);
    }
    private async Task CreateMerchantsFile(string filePath)
    {
        var merchants = await _exportRepo.GetMerchants();

        if (merchants.Length == 0)
            return;

        WriteFile<MerchantExport>(filePath, merchants);
    }
    private async Task CreateSubCategoriesFile(string filePath)
    {
        var subCategories = await _exportRepo.GetSubCategories();

        if (subCategories.Length == 0)
            return;

        WriteFile<SubCategoryExport>(filePath, subCategories);
    }
    private async Task CreateMainCategoriesFile(string filePath)
    {
        var mainCategories = await _exportRepo.GetMainCategories();

        if (mainCategories.Length == 0)
            return;

        WriteFile<MainCategoryExport>(filePath, mainCategories);
    }
    private void WriteFile<T>(string filePath, IEnumerable<T> exports) where T : notnull
    {
        using (var writer = new StreamWriter(filePath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(exports);
        }
    }
    private async Task CreateAllDataZip()
    {
        var filePath = Path.ChangeExtension(Path.Combine(Path.GetTempPath(), DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")), ".csv");
        var expenses = await _exportRepo.GetExpenses();

        if (expenses.Length == 0)
            return;

        WriteFile<ExpenseExport>(filePath, expenses);
    }
}

//downloading multiple files with zip archive
//public (string fileType, byte[] archiveData, string archiveName) DownloadFiles(string subDirectory)
//{
//    var zipName = $"archive-{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.zip";

//    var files = Directory.GetFiles(Path.Combine(_hostingEnvironment.ContentRootPath, subDirectory)).ToList();

//    using (var memoryStream = new MemoryStream())
//    {
//        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
//        {
//            files.ForEach(file =>
//            {
//                var theFile = archive.CreateEntry(file);
//                using (var streamWriter = new StreamWriter(theFile.Open()))
//                {
//                    streamWriter.Write(File.ReadAllText(file));
//                }

//            });
//        }

//        return ("application/zip", memoryStream.ToArray(), zipName);
//    }

//}