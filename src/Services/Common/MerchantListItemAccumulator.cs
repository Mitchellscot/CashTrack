using CashTrack.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CashTrack.Services.Common
{

    public class MerchantListItemAccumulator
    {
        private readonly SubCategoryEntity[] _categories;
        private readonly MerchantEntity[] _merchants;
        public MerchantListItemAccumulator(int? merchantId, MerchantEntity[] merchants, SubCategoryEntity[] categories)
        {
            _categories = categories;
            _merchants = merchants;
            LastPurchase = DateTime.MinValue;
            Categories = new();
            MerchantId = merchantId;
        }
        public decimal Amount { get; private set; }
        public DateTime LastPurchase { get; private set; }
        public List<int> Categories { get; private set; }
        public int MostUsedCategoryId { get; private set; }
        public string MostUsedCategory { get; private set; }
        public int Purchases { get; private set; }
        public string Location { get; private set; }
        public MerchantEntity Merchant { get; private set; }
        public int? MerchantId { get; private set; }
        public MerchantListItemAccumulator Accumulate(ExpenseEntity e)
        {
            Amount += e.Amount;
            LastPurchase = e.Date > this.LastPurchase ? e.Date.DateTime : this.LastPurchase;
            Purchases++;
            Categories.Add(e.CategoryId.Value);
            return this;
        }
        public MerchantListItemAccumulator Compute()
        {
            MostUsedCategoryId = Categories.GroupBy(x => x).OrderByDescending(x => x.Count()).FirstOrDefault().Key;
            MostUsedCategory = _categories.Where(x => x.Id == MostUsedCategoryId).FirstOrDefault().Name;
            Merchant = _merchants.Where(x => x.Id == MerchantId).FirstOrDefault();
            Location = Merchant != null ?
                Merchant.IsOnline ? "Online" :
                Merchant.City == "Various" ? "Various" :
                Merchant.City != null && Merchant.State != null ? $"{Merchant.City}, {Merchant.State}"
            : null : null;
            return this;
        }
    }
}