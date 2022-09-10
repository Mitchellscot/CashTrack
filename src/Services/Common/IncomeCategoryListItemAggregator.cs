using CashTrack.Data.Entities;
using System;
using System.Linq;

namespace CashTrack.Services.Common
{
    public class IncomeCategoryListItemAggregator
    {
        private readonly IncomeCategoryEntity[] _categories;

        public IncomeCategoryListItemAggregator(int incomecategoryId, IncomeCategoryEntity[] categories)
        {
            _categories = categories;
            LastPayment = DateTime.MinValue;
            IncomecategoryId = incomecategoryId;
        }
        public DateTime LastPayment { get; private set; }
        public int IncomecategoryId { get; private set; }
        public decimal Amount { get; private set; }
        public int Payments { get; private set; }
        public IncomeCategoryEntity Category { get; private set; }
        public IncomeCategoryListItemAggregator Accumulate(IncomeEntity i)
        {
            Amount += i.Amount;
            LastPayment = i.Date > this.LastPayment ? i.Date : this.LastPayment;
            Payments++;
            return this;
        }
        public IncomeCategoryListItemAggregator Compute()
        {
            Category = _categories.Where(x => x.Id == IncomecategoryId).FirstOrDefault();
            return this;
        }
    }
}
