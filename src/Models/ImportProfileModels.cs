﻿using CashTrack.Models.Common;

namespace CashTrack.Models.ImportProfileModels
{
    public class ImportProfileListItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DateColumn { get; set; }
        public string AmountColumn { get; set; }
        public string NotesColumn { get; set; }
        public string IncomeColumn { get; set; }
        public bool ContainsNegativeValue { get; set; }
        public TransactionType NegativeValueTransactionType { get; set; }
        public TransactionType DefaultTransactionType { get; set; }
    }
    public class AddEditImportProfile
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string DateColumn { get; set; }
        public string AmountColumn { get; set; }
        public string NotesColumn { get; set; }
        public string IncomeColumn { get; set; }
        public bool ContainsNegativeValue { get; set; }
        public TransactionType NegativeValueTransactionType { get; set; }
        public TransactionType DefaultTransactionType { get; set; }
    }
    public class AddEditImportProfileModal : AddEditImportProfile
    {
        public bool IsEdit { get; set; }
        public string ReturnUrl { get; set; }
    }
}
