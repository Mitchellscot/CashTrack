using CashTrack.Models.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace CashTrack.Models.ImportCsvModels
{
    public class ImportModel
    {
        public IFormFile File { get; set; }
        public string FileType { get; set; }
        public string ReturnUrl { get; set; }
        public List<string> FileTypes { get; set; }
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
}