using CashTrack.Models.Common;
using CashTrack.Models.ExportModels;
using CashTrack.Repositories.ExportRepository;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace CashTrack.Services.ExportService;

public interface IExportService
{
    //As Readable
    //Task<string> ExportReadableData(ExportTransactionsRequest request);
    Task<string> ExportRawData(int exportFileOption, string zipFolder = null);
}
public class ExportService : IExportService
{
    private readonly IExportRepository _exportRepo;

    public ExportService(IExportRepository exportRepo)
    {
        _exportRepo = exportRepo;
    }
    public async Task<string> ExportRawData(int exportFileOption, string zipFolder = null)
    {
        var fileName = ExportFileOptions.GetAll[exportFileOption].Replace(" ", "");
        var filePath = Path.ChangeExtension(Path.Combine(Path.GetTempPath(), fileName), ".csv");
        if (!string.IsNullOrEmpty(zipFolder))
        {
            filePath = Path.ChangeExtension(Path.Combine(Path.GetTempPath(), zipFolder, fileName), ".csv");
        }
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
                var zipFileName = await CreateAllDataZip();
                return zipFileName;
        }
    }

    private async Task CreateExpensesFile(string filePath)
    {
        var expenses = await _exportRepo.GetExpenses();

        if (expenses.Length == 0)
            return;

        await WriteFile<ExpenseExport>(filePath, expenses);
    }
    private async Task CreateImportRulesFile(string filePath)
    {
        var importRules = await _exportRepo.GetImportRules();

        if (importRules.Length == 0)
            return;

        await WriteFile<ImportRuleExport>(filePath, importRules);
    }
    private async Task CreateIncomeFile(string filePath)
    {
        var incomes = await _exportRepo.GetIncome();

        if (incomes.Length == 0)
            return;

        await WriteFile<IncomeExport>(filePath, incomes);
    }
    private async Task CreateIncomeCategoriesFile(string filePath)
    {
        var incomeCategories = await _exportRepo.GetIncomeCategories();

        if (incomeCategories.Length == 0)
            return;

        await WriteFile<IncomeCategoryExport>(filePath, incomeCategories);
    }
    private async Task CreateIncomeSourcesFile(string filePath)
    {
        var incomeSources = await _exportRepo.GetIncomeSources();

        if (incomeSources.Length == 0)
            return;

        await WriteFile<IncomeSourceExport>(filePath, incomeSources);
    }
    private async Task CreateMerchantsFile(string filePath)
    {
        var merchants = await _exportRepo.GetMerchants();

        if (merchants.Length == 0)
            return;

        await WriteFile<MerchantExport>(filePath, merchants);
    }
    private async Task CreateSubCategoriesFile(string filePath)
    {
        var subCategories = await _exportRepo.GetSubCategories();

        if (subCategories.Length == 0)
            return;

        await WriteFile<SubCategoryExport>(filePath, subCategories);
    }
    private async Task CreateMainCategoriesFile(string filePath)
    {
        var mainCategories = await _exportRepo.GetMainCategories();

        if (mainCategories.Length == 0)
            return;

        await WriteFile<MainCategoryExport>(filePath, mainCategories);
    }
    private async Task WriteFile<T>(string filePath, IEnumerable<T> exports) where T : notnull
    {
        using (var writer = new StreamWriter(filePath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            await csv.WriteRecordsAsync(exports);
        }
        
    }
    private async Task<string> CreateAllDataZip()
    {
        var folderName = "export_" + DateTime.Now.ToString("MM-dd-yyyy_HH_mm_ss");
        var folderPath = Path.Combine(Path.GetTempPath(), folderName);
        Directory.CreateDirectory(folderPath);
        var fileTypes = ExportFileOptions.GetAll.Keys.ToArray();
        var filePaths = new List<string>();
        foreach (var fileType in fileTypes)
        {
            if (fileType == 0)
                continue;

            var filePath = await ExportRawData(fileType, folderPath);
            filePaths.Add(filePath);
        }

        var zipFolderPath = Path.Combine(Path.GetTempPath(), "archive_" + DateTime.Now.ToString("MM-dd-yyyy_HH_mm_ss"));
        if (Directory.Exists(zipFolderPath))
        {
            throw new Exception(zipFolderPath + " Already Exists");
        }
        ZipFile.CreateFromDirectory(folderPath, zipFolderPath);

        return zipFolderPath;
    }
}