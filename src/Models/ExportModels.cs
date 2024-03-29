﻿using CashTrack.Models.BudgetModels;
using CashTrack.Models.Common;
using CashTrack.Models.ImportRuleModels;

namespace CashTrack.Models.ExportModels;

public abstract record TransactionExport(string Id, string Date, string Amount, string CategoryId, string Notes);
public abstract record SourceExport(string Id, string Name, string SuggestOnLookup, string City, string State, string IsOnline, string Notes);
public abstract record CategoryExport(string Id, string Name, string InUse, string Notes);
public record BudgetExport(string Id, string Month, string Year, string Amount, string SubCategoryId, string BudgetType);
public record SubCategoryExport(string Id, string Name, string MainCategoryId, string InUse, string Notes)
    : CategoryExport(Id, Name, InUse, Notes);
public record IncomeCategoryExport(string Id, string Name, string InUse, string Notes)
    : CategoryExport(Id, Name, InUse, Notes);
public record MainCategoryExport(string Id, string Name);
public record IncomeSourceExport(string Id, string Name, string SuggestOnLookup, string City, string State, string IsOnline, string Notes) : SourceExport(Id, Name, SuggestOnLookup, City, State, IsOnline, Notes);
public record MerchantExport(string Id, string Name, string SuggestOnLookup, string City, string State, string IsOnline, string Notes) : SourceExport(Id, Name, SuggestOnLookup, City, State, IsOnline, Notes);
public record ExpenseExport(string Id, string MerchantId, string RefundNotes, string Date, string Amount, string CategoryId, string Notes, string ExcludeFromStatistics)
    : TransactionExport(Id, Date, Amount, CategoryId, Notes);
public record IncomeExport(string Id, string Date, string Amount, string CategoryId, string SourceId, string Notes, string IsRefund, string RefundNotes)
    : TransactionExport(Id, Date, Amount, CategoryId, Notes);
public record ImportRuleExport(string Id, RuleType RuleType, string FileType, TransactionType TransactionType, string Rule, string MerchantSourceId, string CategoryId);
public record ReadableBudgetExport(string Month, string Year, string Amount, string SubCategory, string MainCategory, BudgetType BudgetType);
public record ReadableExpenseExport(string Date, string Amount, string Category, string Merchant, string Notes);
public record ReadableIncomeExport(string Date, string Amount, string Category, string Source, string Notes, string IsRefund);
public record ReadableIncomeCategoryExport(string Name, string InUse, string Notes);
public record ReadableIncomeSourceExport(string Name, string City, string State, string Notes);
public record ReadableMainCategoryExport(string Name);
public record ReadableMerchantExport(string Name, string City, string State, string Notes);
public record ReadableSubCategoryExport(string Name, string MainCategory, string InUse, string Notes);
public class ReadableImportRuleExport
{
    public string Id { get; set; }
    public RuleType RuleType { get; set; }
    public string FileType { get; set; }
    public TransactionType TransactionType { get; set; }
    public string Rule { get; set; }
    public string MerchantSource { get; set; }
    public string Category { get; set; }
}