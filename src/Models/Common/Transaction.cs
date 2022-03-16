using CashTrack.Data.Entities;
using System;
using System.Collections.Generic;

namespace CashTrack.Models.Common
{
    public abstract class Transaction
    {
        private decimal _amount;
        public int Id { get; set; }
        public DateTimeOffset Date { get; set; }
        public decimal Amount
        {
            get => _amount;
            set => _amount = Decimal.Round(value, 2);
        }
    }
    public abstract class TransactionRequest : PaginationRequest
    {
        public DateOptions DateOptions { get; set; }
        public DateTimeOffset BeginDate { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset EndDate { get; set; } = DateTimeOffset.UtcNow;
    }
    public abstract class TransactionResponse<T> : PaginationResponse<T> where T : Transaction
    {
        public decimal TotalAmount { get; private set; }

        protected TransactionResponse(int pageNumber, int pageSize, int count, IEnumerable<T> listItems, decimal amount) : base(pageNumber, pageSize, count, listItems)
        {
            TotalAmount = Math.Round(amount, 2);
        }
    }
}