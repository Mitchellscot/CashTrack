using CashTrack.Models.Common;

namespace CashTrack.Models.IncomeSourceModels;

public class IncomeSourceRequest : PaginationRequest
{
}

public class IncomeSourceResponse : PaginationResponse<IncomeSourceListItem>
{
    public IncomeSourceResponse(int pageNumber, int pageSize, int totalCount, IncomeSourceListItem[] listItems) : base(pageNumber, pageSize, totalCount, listItems) { }
}
public record IncomeSource
{
    public int? Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool InUse { get; set; }
}
public record AddIncomeSourceModal : IncomeSource
{ 
    public string ReturnUrl { get; set; }
}
public record IncomeSourceListItem
{
    public int Id { get; set; }
    public string Name { get; set; }
}
public record IncomeSourceDetail : IncomeSource
{
    //some other stuff goes here
}