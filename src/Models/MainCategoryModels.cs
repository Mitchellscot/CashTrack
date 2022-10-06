using CashTrack.Models.Common;
using CashTrack.Models.SubCategoryModels;
using System.Collections.Generic;

namespace CashTrack.Models.MainCategoryModels;

public record MainCategoryRequest
{
    public string Query { get; set; }
}
public record MainCategoryResponse
{
    public int TotalMainCategories { get; set; }
    public MainCategoryListItem[] MainCategories { get; set; }
    public Dictionary<string, int> CategoryPercentages { get; set; }
    public Dictionary<string, int> CategoryPurchaseOccurances { get; set; }

    //For the MainCategoryExpenseChart
    //You need a list of Main Category Names for the labels
    //You need a dataset for each sub category, and it will be an array the same size of how many main categories there are (16 right now)
    //and all numbers will be 0 except for the index number of the main category
    // so like, 
    //Career, Clothing, Entertainment, Food, Giving, Health, Hobbies, Household, Insurance, Kids, Mortgage, Other, Reimbursement, Transportation, Utilities, Vacation
    //and for dining out the dataset will be
    // [0, 0, 0, 25000.68, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]
    //for groceries it will be similar
    //[0, 0, 0, 55584.68, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]
    //and that way the datasets stack on the labels
    //So this needs to be a List of objects that have a string (SubCategoryName) and an array of strings, which is the dataset
    //You need to find a way to create an array of zeros with the length of the array of main category names
    //and then insert the right amount at the index of the main category name
    //lol not too hard
}
public class MainCategoryListItem : Category
{
    public int NumberOfSubCategories { get; set; }
    public Dictionary<string, int> SubCategoryExpenses { get; set; }

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

