using CashTrack.Models.Common;
using CsvHelper.Configuration.Attributes;
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
    public class BankImport
    {
        private decimal _amount;
        private string _notes;
        public DateTime Date { get; set; }
        public decimal Amount
        {
            get => _amount;
            set
            {
                IsIncome = value > 0;
                _amount = Decimal.Round(Math.Abs(value), 2);
            }
        }
        public string Notes
        {
            get => _notes;
            set => _notes = !string.IsNullOrEmpty(value) ? value.ToLower() : "";
        }
        public bool IsIncome { get; private set; }
        public int? MerchantSourceId { get; set; }
        public int? CategoryId { get; set; }
    }
    public class CreditImport
    {
        private decimal _debit;
        private decimal _credit;
        public DateTime Date { get; set; }
        public decimal Debit
        {
            get => _debit;
            set
            {
                this.IsIncome = false;
                _debit = value > 0 ? Decimal.Round(Math.Abs(value), 2) : 0;
            }
        }
        public decimal Credit
        {
            get => _credit;
            set
            {
                this.IsIncome = true;
                _credit = value > 0 ? Decimal.Round(Math.Abs(value), 2) : 0;
            }
        }
        public string Notes { get; set; }
        public bool IsIncome { get; set; }
        public int MerchantSourceId { get; set; }
        public int CategoryId { get; set; }
    }
    public class OtherTransactionImport
    {
        private decimal _amount;
        public DateTime Date { get; set; }
        public decimal Amount
        {
            get => _amount;
            set
            {
                IsIncome = value > 0;
                _amount = Decimal.Round(Math.Abs(value), 2);
            }
        }
        public string Notes { get; set; }
        public bool IsIncome { get; set; }
        public int MerchantSourceId { get; set; }
        public int CategoryId { get; set; }
    }
    public enum CsvFileType
    {
        Bank,
        Credit,
        Other
    }
}
