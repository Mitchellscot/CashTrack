using CashTrack.Data.Entities;
using System;
using System.Linq;

namespace CashTrack.Services.Common
{
    public class SubCategoryListItemAggregator
    {
        private readonly SubCategoryEntity[] _categories;

        public SubCategoryListItemAggregator(int subcategoryId, SubCategoryEntity[] categories)
        {
            _categories = categories;
            LastPurchase = DateTime.MinValue;
            SubcategoryId = subcategoryId;
        }
        public DateTime LastPurchase { get; private set; }
        public int SubcategoryId { get; private set; }
        public decimal Amount { get; private set; }
        public int Purchases { get; private set; }
        public SubCategoryEntity Category { get; private set; }
        public SubCategoryListItemAggregator Accumulate(ExpenseEntity e)
        {
            Amount += e.Amount;
            LastPurchase = e.Date > this.LastPurchase ? e.Date.DateTime : this.LastPurchase;
            Purchases++;
            return this;
        }
        public SubCategoryListItemAggregator Compute()
        {
            Category = _categories.Where(x => x.Id == SubcategoryId).FirstOrDefault();
            return this;
        }
    }
}
