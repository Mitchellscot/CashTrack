namespace CashTrack.Models.ExportModels;

public abstract record TransactionExport(string Id, string Date, string Amount, string CategoryId, string Notes);
public abstract record SourceExport(string Id, string Name, string SuggestOnLookup, string City, string State, string IsOnline);
public abstract record CategoryExport(string Id, string Name, string InUse, string Notes);


public record SubCategoryExport(string Id, string Name, string MainCategoryId, string InUse, string Notes)
    : CategoryExport(Id, Name, InUse, Notes);
public record IncomeCategoryExport(string Id, string Name, string InUse, string Notes)
    : CategoryExport(Id, Name, InUse, Notes);
public record MainCategoryExport(string Id, string Name);
public record IncomeSourceExport(string Id, string Name, string SuggestOnLookup, string City, string State, string IsOnline) : SourceExport(Id, Name, SuggestOnLookup, City, State, IsOnline);

public record MerchantExport(string Id, string Name, string SuggestOnLookup, string City, string State, string IsOnline) : SourceExport(Id, Name, SuggestOnLookup, City, State, IsOnline);

public record ExpenseExport(string Id, string MerchantId, string RefundNotes, string Date, string Amount, string CategoryId, string Notes, string ExcludeFromStatistics)
    : TransactionExport(Id, Date, Amount, CategoryId, Notes);

public record IncomeExport(string Id, string Date, string Amount, string CategoryId, string SourceId, string Notes, string IsRefund, string RefundNotes)
    : TransactionExport(Id, Date, Amount, CategoryId, Notes);
public record ImportRuleExport(string Id, string Transaction, string Rule, string MerchantSourceId, string CategoryId);


public record ReadableExpenseExport(string Date, string Amount, string Category, string Merchant, string Notes);
public record ReadableIncomeExport(string Date, string Amount, string Category, string Source, string Notes, string IsRefund);
public record ReadableIncomeCategoryExport(string Name, string InUse, string Notes);
public record ReadableIncomeSourceExport(string Name, string City, string State);
public record ReadableMainCategoryExport(string Name);
public record ReadableMerchantExport(string Name, string City, string State);
public record ReadableSubCategoryExport(string Name, string MainCategory, string InUse, string Notes);