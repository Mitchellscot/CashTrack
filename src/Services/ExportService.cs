using CashTrack.Models.ExportModels;
using CashTrack.Repositories.ExportRepository;
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
}
public class ExportService : IExportService
{
    private readonly IExportRepository _exportRepo;

    public ExportService(IExportRepository exportRepo) => _exportRepo = exportRepo;

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
}