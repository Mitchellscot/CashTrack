using CashTrack.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CashTrack.Services.Common
{
    public class SourceListItemAggregator
    {
        private readonly IncomeSourceEntity[] _sources;
        private readonly IncomeCategoryEntity[] _categories;

        public SourceListItemAggregator(int? sourceId, IncomeCategoryEntity[] categories, IncomeSourceEntity[] sources)
        {
            _sources = sources;
            _categories = categories;
            LastPayment = DateTime.MinValue;
            Categories = new();
            SourceId = sourceId;
        }
        public decimal Amount { get; private set; }
        public DateTime LastPayment { get; private set; }
        public List<int> Categories { get; private set; }
        public int MostUsedCategoryId { get; private set; }
        public string MostUsedCategory { get; private set; }
        public int Payments { get; private set; }
        public int? SourceId { get; private set; }
        public IncomeSourceEntity Source { get; private set; }
        public SourceListItemAggregator Accumulate(IncomeEntity i)
        {
            Amount += i.Amount;
            LastPayment = i.Date > this.LastPayment ? i.Date : this.LastPayment;
            Payments++;
            Categories.Add(i.CategoryId.Value);
            return this;
        }
        public SourceListItemAggregator Compute()
        {
            MostUsedCategoryId = Categories.GroupBy(x => x).OrderByDescending(x => x.Count()).FirstOrDefault().Key;
            MostUsedCategory = _categories.Where(x => x.Id == MostUsedCategoryId).FirstOrDefault().Name;
            Source = SourceId.HasValue ? _sources.Where(x => x.Id == SourceId.Value).FirstOrDefault() : null;
            return this;
        }
    }
}
