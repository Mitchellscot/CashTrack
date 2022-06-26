using CashTrack.Data;
using CashTrack.Data.Entities;
using CashTrack.Models.ExportModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CashTrack.Repositories.ExportRepository;
public interface IExportRepository
{
    Task<ExpenseEntity[]> GetExpenseTransactionsToExportAsync(ExportTransactionsRequest request);
    Task<IncomeEntity[]> GetIncomeTransactionsToExportAsync();
    //Task<T[]> GetTransactionsToExportAsync<T>() where T : ExportTransaction;
}
public class ExportRepository : IExportRepository
{
    private readonly AppDbContext _ctx;
    public ExportRepository(AppDbContext ctx) => _ctx = ctx;
    public async Task<ExpenseEntity[]> GetExpenseTransactionsToExportAsync(ExportTransactionsRequest request)
    {
        try
        {
            IQueryable<ExpenseEntity> expenses = _ctx.Expenses.Include(x => x.Merchant).Include(x => x.Category);

            expenses = expenses.OrderBy(x => x.Date);
            return await expenses.ToArrayAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IncomeEntity[]> GetIncomeTransactionsToExportAsync()
    {
        try
        {
            IQueryable<IncomeEntity> incomes = _ctx.Incomes.Include(x => x.Source).Include(x => x.Category);

            //can filter here with Where() if you decide to add date ranges to ExportTransactionsRequest
            //otherwise it just exports all of them
            incomes = incomes.OrderBy(x => x.Date);
            return await incomes.ToArrayAsync();
        }
        catch (Exception)
        {

            throw;
        }
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