﻿using CashTrack.Models.Common;
using CashTrack.Models.TagModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CashTrack.Models.ExpenseModels;

public class ExpenseRequest : TransactionRequest
{
}
public class ExpenseResponse : TransactionResponse<Expense>
{
    public ExpenseResponse(int pageNumber, int pageSize, int count, IEnumerable<Expense> listItems, decimal amount) : base(pageNumber, pageSize, count, listItems, amount)
    {
    }
}
public class AmountSearchRequest : PaginationRequest
{
    private decimal _query;
    new public decimal Query
    {
        get => _query;
        set => _query = Decimal.Round(value, 2);
    }
}
public class Expense : Transaction
{
    new public int? Id { get; set; }
    public string Notes { get; set; }
    public string Merchant { get; set; }
    public int? MerchantId { get; set; }
    public ICollection<TagModel> Tags { get; set; }
    public string SubCategory { get; set; }
    public int SubCategoryId { get; set; }
    public string MainCategory { get; set; }
    public bool ExcludeFromStatistics { get; set; }
    public bool CreateNewMerchant { get; set; }
    public string RefundNotes { get; set; }
}
public class AddEditExpenseModal : Expense
{
    public bool IsEdit { get; set; }
    public string ReturnUrl { get; set; }
    public SelectList SubCategoryList { get; set; }
    public SelectList MainCategoryList { get; set; }
    public int? PageNumber { get; set; }
    public int? Query { get; set; }
    public string Q { get; set; }
    public string Q2 { get; set; }
    public bool ShowAddCategoryButton { get; set; }
}
public class ExpenseQuickView : Transaction
{
    new public string Date { get; set; }
    public string SubCategory { get; set; }
}
public class ExpenseQuickViewForSubCategoryDetail : Transaction
{
    new public string Date { get; set; }
    public string Merchant { get; set; }
}
public class ExpenseSplit
{
    public decimal _amount;
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public decimal Amount
    {
        get => _amount;
        set => _amount = Decimal.Round(value, 2);
    }
    [Required]
    public int SubCategoryId { get; set; }
    public string Notes { get; set; }
    public bool Taxed { get; set; }
    [Range(0.00001, 0.99999, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    [Required]
    public decimal Tax { get; set; }
    [Required]
    public DateTime Date { get; set; }
    public string Merchant { get; set; }
    public bool IsTaxed() => this.Taxed;
    public void SetTaxIfTaxed() => _amount = this.IsTaxed() ? Decimal.Round((_amount + (_amount * this.Tax)), 2) : _amount;
}
public class ExpenseRefund
{
    private decimal _originalAmount;
    private decimal _refundAmount;
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public decimal OriginalAmount
    {
        get => _originalAmount;
        set => _originalAmount = Decimal.Round(value, 2);
    }
    public string Merchant { get; set; }
    public string Category { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public decimal RefundAmount
    {
        get => _refundAmount;
        set => _refundAmount = Decimal.Round(value, 2);
    }
    public bool? ApplyFullAmount { get; set; }
}