using CashTrack.Models.Common;
using CashTrack.Models.ImportRuleModels;
using Microsoft.AspNetCore.Http;
using System;

namespace CashTrack.Models.ImportCsvModels
{
    public class ImportModel
    {
        public IFormFile File { get; set; }
        public CsvFileType FileType { get; set; }
        public string ReturnUrl { get; set; }
    }
    public class ImportTransaction : Transaction
    {
        private decimal _amount;
        new public decimal Amount
        {
            get => _amount;
            set
            {
                IsIncome = value > 0;
                _amount = Decimal.Round(Math.Abs(value), 2);
            }
        }
        private string _notes;
        public bool IsIncome { get; set; }
        public int? MerchantSourceId { get; set; }
        public int? CategoryId { get; set; }
        public string Notes
        {
            get => _notes;
            set => _notes = !string.IsNullOrEmpty(value) ? value.ToLower() : "";
        }
    }
    public class BankImport : ImportTransaction
    {

    }
    public class CreditImport : ImportTransaction
    {
        private decimal? _debit;
        private decimal? _credit;
        public decimal? Debit
        {
            get => _debit;
            set
            {
                _debit = value.HasValue ? Decimal.Round(Math.Abs(value.Value), 2) : null;
            }
        }
        public decimal? Credit
        {
            get => _credit;
            set
            {

                _credit = value.HasValue ? Decimal.Round(Math.Abs(value.Value), 2) : null;
            }
        }
    }
    public class OtherTransactionImport : ImportTransaction
    {
        //TODO: figure this one out
    }
}