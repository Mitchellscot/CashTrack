using CashTrack.Models.Common;
using CashTrack.Models.SubCategoryModels;
using System.Collections.Generic;
using System.Linq;

namespace CashTrack.Models.MainCategoryModels;

public record MainCategoryRequest
{
    public string Query { get; set; }
    public MainCategoryTimeOptions TimeOption { get; set; }
}
public record MainCategoryResponse
{
    public int TotalMainCategories { get; set; }
    public MainCategoryListItem[] MainCategories { get; set; }
    public Dictionary<string, int> MainCategoryPercentages { get; set; }
    public Dictionary<string, int> SubCategoryPercentages { get; set; }
    public Dictionary<string, int> CategoryPurchaseOccurances { get; set; }
    public Dictionary<string, int> SavingsPercentages { get; set; }
    public MainCategoryChartData MainCategoryChartData { get; set; }
}
public class MainCategoryChartData
{
    public string[] MainCategoryNames { get; set; }
    public List<SubCategoryAmountDataset> SubCategoryData { get; set; }
}

public class SubCategoryAmountDataset
{
    public SubCategoryAmountDataset(string subCategoryName, int datasetLength, decimal Amount, int indexOfMainCategory, int mainCategoryId)
    {
        MainCategoryId = mainCategoryId;
        SubCategoryName = subCategoryName;
        DataSet = SetDataSet(datasetLength, Amount, indexOfMainCategory);
    }
    public int MainCategoryId { get; private set; }
    public string SubCategoryName { get; private set; }
    public decimal[] DataSet { get; set; }
    public string Color { get; set; }
    private decimal[] SetDataSet(int datasetLength, decimal amount, int indexOfMainCategory)
    {
        var dataSet = Enumerable.Repeat(0m, datasetLength).ToArray();
        dataSet[indexOfMainCategory] = amount;
        return dataSet;
    }
}
public class MainCategoryListItem : Category
{
    public int NumberOfSubCategories { get; set; }
    public Dictionary<string, int> SubCategoryExpenses { get; set; }

}
public class AddEditMainCategoryModal : Category
{
    new public int? Id { get; set; }
    public bool IsEdit { get; set; }
    public string ReturnUrl { get; set; }
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
