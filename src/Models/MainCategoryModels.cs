using CashTrack.Models.Common;
using CashTrack.Models.SubCategoryModels;

namespace CashTrack.Models.MainCategoryModels;

public record MainCategoryRequest
{
    public string Query { get; set; }
}
public record MainCategoryResponse
{
    public int TotalMainCategories { get; set; }
    public MainCategoryListItem[] MainCategories { get; set; }
}
public class MainCategoryListItem : Category
{
    public int NumberOfSubCategories { get; set; }
}
public class AddEditMainCategory : Category
{
    new public int? Id { get; set; }
}
public class MainCategoryDetail : Category
{
    public SubCategoryListItem[] SubCategories { get; set; }
    //Think of a stats object like merchant detail with every year and a bar graph of expenses by sub category for each year,
}
public class MainCategoryDropdownSelection
{
    public int Id { get; set; }
    public string Category { get; set; }
}

