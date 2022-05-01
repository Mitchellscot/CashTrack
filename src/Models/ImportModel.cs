using CashTrack.Models.Common;
using Microsoft.AspNetCore.Http;


namespace CashTrack.Models.ImportCsvModels
{
    public class ImportModel
    {
        public IFormFile File { get; set; }
        //might have to change this to a list in the future, predifined in settings, where each transaction source has a name and a set of rules associated with it (bank, credit card 1, credit card 2, etc.)
        public bool IsBankFile { get; set; }
        public string ReturnUrl { get; set; }
    }
    public class TransactionImport : Transaction
    {
        public string Notes { get; set; }
        public bool IsExpense { get; set; }
        public int MerchantSourceId { get; set; }
        public int CategoryId { get; set; }
    }
}
