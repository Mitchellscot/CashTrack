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
    Task<string> ExportData(int exportFileOption, bool asReadable, string zipFolder = null);
}
public class ExportService : IExportService
{
    private readonly IExportRepository _exportRepo;

    public ExportService(IExportRepository exportRepo)
    {
        _exportRepo = exportRepo;
    }
    public async Task<string> ExportData(int exportFileOption, bool asReadable, string zipFolder = null)
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
                await CreateBudgetsFile(filePath, asReadable);
                return filePath;
            case 2:
                await CreateExpensesFile(filePath, asReadable);
                return filePath;
            case 3:
                await CreateImportRulesFile(filePath, asReadable);
                return filePath;
            case 4:
                await CreateIncomeFile(filePath, asReadable);
                return filePath;
            case 5:
                await CreateIncomeCategoriesFile(filePath, asReadable);
                return filePath;
            case 6:
                await CreateIncomeSourcesFile(filePath, asReadable);
                return filePath;
            case 7:
                await CreateMainCategoriesFile(filePath, asReadable);
                return filePath;
            case 8:
                await CreateMerchantsFile(filePath, asReadable);
                return filePath;
            case 9:
                await CreateSubCategoriesFile(filePath, asReadable);
                return filePath;
            default:
                var zipFileName = await CreateAllDataZip(asReadable);
                return zipFileName;
        }
    }
    private async Task CreateBudgetsFile(string filePath, bool asReadable)
    {
        if (asReadable)
        {
            var readableBudgets = await _exportRepo.GetReadableBudgetExport();

            if (readableBudgets.Length == 0)
                return;

            await WriteFileAsync(filePath, readableBudgets);
        }
        else
        {
            var budgets = await _exportRepo.GetBudgets();

            if (budgets.Length == 0)
                return;

            await WriteFileAsync<BudgetExport>(filePath, budgets);
        }
    }
    private async Task CreateExpensesFile(string filePath, bool asReadable)
    {
        if (asReadable)
        {
            var readableExpenses = await _exportRepo.GetReadableExpenses();

            if (readableExpenses.Length == 0)
                return;

            await WriteFileAsync(filePath, readableExpenses);
        }
        else
        {
            var expenses = await _exportRepo.GetExpenses();

            if (expenses.Length == 0)
                return;

            await WriteFileAsync<ExpenseExport>(filePath, expenses);
        }
    }
    private async Task CreateImportRulesFile(string filePath, bool asReadable)
    {
        if (asReadable)
        {
            var readableImportRules = await _exportRepo.GetReadableImportRules();

            if (readableImportRules.Length == 0)
                return;

            await WriteFileAsync(filePath, readableImportRules);
        }
        else
        {
            var importRules = await _exportRepo.GetImportRules();

            if (importRules.Length == 0)
                return;

            await WriteFileAsync<ImportRuleExport>(filePath, importRules);
        }
    }
    private async Task CreateIncomeFile(string filePath, bool asReadable)
    {
        if (asReadable)
        {
            var readableIncomes = await _exportRepo.GetReadableIncome();

            if (readableIncomes.Length == 0)
                return;

            await WriteFileAsync(filePath, readableIncomes);
        }
        else
        {
            var incomes = await _exportRepo.GetIncome();

            if (incomes.Length == 0)
                return;

            await WriteFileAsync<IncomeExport>(filePath, incomes);
        }
    }
    private async Task CreateIncomeCategoriesFile(string filePath, bool asReadable)
    {
        if (asReadable)
        {
            var readableIncomeCategories = await _exportRepo.GetReadableIncomeCategories();

            if (readableIncomeCategories.Length == 0)
                return;

            await WriteFileAsync(filePath, readableIncomeCategories);
        }
        else
        {
            var incomeCategories = await _exportRepo.GetIncomeCategories();

            if (incomeCategories.Length == 0)
                return;

            await WriteFileAsync<IncomeCategoryExport>(filePath, incomeCategories);
        }

    }
    private async Task CreateIncomeSourcesFile(string filePath, bool asReadable)
    {
        if (asReadable)
        {
            var readableIncomeSources = await _exportRepo.GetReadableIncomeSources();

            if (readableIncomeSources.Length == 0)
                return;

            await WriteFileAsync(filePath, readableIncomeSources);
        }
        else
        {
            var incomeSources = await _exportRepo.GetIncomeSources();

            if (incomeSources.Length == 0)
                return;

            await WriteFileAsync<IncomeSourceExport>(filePath, incomeSources);
        }

    }
    private async Task CreateMerchantsFile(string filePath, bool asReadable)
    {
        if (asReadable)
        {
            var readableMerchants = await _exportRepo.GetReadableMerchants();

            if (readableMerchants.Length == 0)
                return;

            await WriteFileAsync(filePath, readableMerchants);
        }
        else
        {
            var merchants = await _exportRepo.GetMerchants();

            if (merchants.Length == 0)
                return;

            await WriteFileAsync<MerchantExport>(filePath, merchants);
        }
    }
    private async Task CreateSubCategoriesFile(string filePath, bool asReadable)
    {
        if (asReadable)
        {
            var readableSubCategories = await _exportRepo.GetReadableSubCategories();

            if (readableSubCategories.Length == 0)
                return;

            await WriteFileAsync(filePath, readableSubCategories);
        }
        else
        {
            var subCategories = await _exportRepo.GetSubCategories();

            if (subCategories.Length == 0)
                return;

            await WriteFileAsync<SubCategoryExport>(filePath, subCategories);
        }

    }
    private async Task CreateMainCategoriesFile(string filePath, bool asReadable)
    {
        if (asReadable)
        {
            var readableMainCategories = await _exportRepo.GetReadableMainCategories();

            if (readableMainCategories.Length == 0)
                return;

            await WriteFileAsync(filePath, readableMainCategories);
        }
        else
        {
            var mainCategories = await _exportRepo.GetMainCategories();

            if (mainCategories.Length == 0)
                return;

            await WriteFileAsync<MainCategoryExport>(filePath, mainCategories);
        }
    }
    private async Task WriteFileAsync<T>(string filePath, IEnumerable<T> exports) where T : notnull
    {
        using (var writer = new StreamWriter(filePath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            await csv.WriteRecordsAsync(exports);
        }
    }
    private async Task<string> CreateAllDataZip(bool asReadable)
    {
        var exportFolderPath = Path.Combine(Path.GetTempPath(), "export_" + DateTime.Now.ToString("MM-dd-yyyy_HH_mm_ss"));
        var directoryInfo = Directory.CreateDirectory(exportFolderPath);
        var filePaths = new List<string>();
        foreach (var fileType in ExportFileOptions.GetAll.Keys.ToArray())
        {
            if (fileType == 0)
                continue;

            var filePath = await ExportData(fileType, asReadable, exportFolderPath);
            filePaths.Add(filePath);
        }
        var zipFolderPath = Path.Combine(Path.GetTempPath(), "archive_" + DateTime.Now.ToString("MM-dd-yyyy_HH_mm_ss"));
        ZipFile.CreateFromDirectory(exportFolderPath, zipFolderPath);
        CleanupFiles(exportFolderPath, directoryInfo);
        return zipFolderPath;
    }

    private void CleanupFiles(string folderPath, DirectoryInfo di = null)
    {
        foreach (var file in Directory.GetFiles(folderPath, "*.csv", SearchOption.TopDirectoryOnly))
        {
            File.Delete(file);
        }
        if (di != null)
        {
            di.Delete();
        }
    }
}