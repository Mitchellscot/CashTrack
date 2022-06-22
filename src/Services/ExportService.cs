using CashTrack.Models.ExportModels;
using CashTrack.Repositories.ExportRepository;
using CashTrack.Repositories.MainCategoriesRepository;
using CsvHelper;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Environment;

namespace CashTrack.Services.ExportService;

public interface IExportService
{
    Task<string> ExportTransactions(ExportTransactionsRequest request);
    Task<string> ExportData();
}
public class ExportService : IExportService
{
    private readonly IExportRepository _exportRepo;
    private readonly IMainCategoriesRepository _mainCategoriesRepo;

    public ExportService(IExportRepository exportRepo, IMainCategoriesRepository mainCategoriesRepo)
    {
        _exportRepo = exportRepo;
        _mainCategoriesRepo = mainCategoriesRepo;
    }
    public async Task<string> ExportData() 
    {
        //TODO: create a hardcoded enum of these values
        var fileNames = new string[]
        {
            "MainCategories",
            "SubCategories",
            "Merchants",
            "Expenses",
            "IncomeCategories",
            "IncomeSources",
            "Income",
            "ImportRules"
        };
        var filePaths = fileNames.Select(x => Path.ChangeExtension(Path.Combine(Path.GetTempPath(), x), ".csv")).ToArray();
        foreach (var filePath in filePaths)
        {
            switch (filePath)
            {
                case "MainCategories":
                    await CreateMainCategoriesTempFile(filePath);
                    break;
                case "SubCategories":
                    //do stuff
                    break;
                case "Merchants":
                    //do stuff
                    break;
                case "Expenses":
                    //do stuff
                    break;
                case "IncomeCategories":
                    //do stuff
                    break;
                case "IncomeSources":
                    //do stuff
                    break;
                case "Income":
                    //do stuff
                    break;
                case "ImportRules":
                    //do stuff
                    break;
                default:
                    //do default thing
                    break;
            }
                
        }
    }
    public record ExportMainCategory(int Id, string Name);

    public async Task<string> ExportTransactions(ExportTransactionsRequest request)
    {
        var filePath = Path.ChangeExtension(Path.GetTempFileName(), ".csv");

        if (request.IsIncome)
        {
            var incomes = await _exportRepo.GetIncomeTransactionsToExportAsync();
            if (incomes.Length == 0)
                return String.Empty;

            var incomeExports = incomes.Select(x => new ExportIncome
            {
                Date = x.Date.DateTime.ToShortDateString(),
                Amount = x.Amount.ToString(),
                Source = x.Source.Name,
                Category = x.Category.Name,
                Notes = x.Notes + " - " + x.RefundNotes
            });

            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(incomeExports);
            }
        }
        else
        {
            var expenses = await _exportRepo.GetExpenseTransactionsToExportAsync(request);
            if (expenses.Length == 0)
                return String.Empty;

            var expenseExports = expenses.Select(x => new ExportExpense
            {
                Date = x.Date.DateTime.ToShortDateString(),
                Amount = x.Amount.ToString(),
                Merchant = x.Merchant?.Name,
                Category = x.Category.Name,
                Notes = x.Notes + " - " + x.RefundNotes
            });

            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(expenseExports);
            }
        }
        return filePath;
    }
    public async Task CreateMainCategoriesTempFile(string filePath)
    {
        var mainCategories = await _mainCategoriesRepo.Find(x => true);

        if (mainCategories.Length == 0)
            //Just create an empty file and return. So the user would download an empty file with the file name.
            return;

        var mainCategoryExports = mainCategories.Select(x => new ExportMainCategory(Id: x.Id, Name: x.Name)).ToArray();

        using (var writer = new StreamWriter(filePath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(mainCategoryExports);
        }
    }
}