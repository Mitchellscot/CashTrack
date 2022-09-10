using CashTrack.Models.Common;
using System;

namespace CashTrack.Models.IncomeCategoryModels;

public class IncomeCategoryRequest : PaginationRequest
{
    public bool Reversed { get; set; }
    public IncomeCategoryOrderBy Order { get; set; }
}
public class IncomeCategoryResponse : PaginationResponse<IncomeCategoryListItem>
{
    public IncomeCategoryResponse(int pageNumber, int pageSize, int totalCount, IncomeCategoryListItem[] listItems) : base(pageNumber, pageSize, totalCount, listItems) { }
}
public class IncomeCategory : Category
{
    new public int? Id { get; set; }
    public string Notes { get; set; }
    public bool InUse { get; set; } = true;
}
public class IncomeCategoryListItem : Category
{
    public int Payments { get; set; }
    public decimal Amount { get; set; }
    public DateTime LastPayment { get; set; }
    public bool InUse { get; set; }
}
public class IncomeCategoryDetail : Category
{
    public string Notes { get; set; }
    public bool InUse { get; set; } = true;
    //maybe some other cool properties... how many incomes related to this, total amount... maybe a yearly graph?
    //Get all expenses by income category and compare with income sources. How many gifts came from my parents, etc.
}

public class AddEditIncomeCategoryModal : IncomeCategory
{
    public bool IsEdit { get; set; }
    public string Returnurl { get; set; }
}

public class IncomeCategoryDropdownSelection
{
    public int Id { get; set; }
    public string Category { get; set; }
}

