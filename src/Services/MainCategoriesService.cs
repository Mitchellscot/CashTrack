using CashTrack.Data.Entities;
using CashTrack.Common.Exceptions;
using CashTrack.Models.MainCategoryModels;
using CashTrack.Repositories.MainCategoriesRepository;
using CashTrack.Repositories.SubCategoriesRepository;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;
using CashTrack.Repositories.IncomeRepository;
using CashTrack.Services.Common;
using CashTrack.Common;
using CashTrack.Common.Extensions;

namespace CashTrack.Services.MainCategoriesService
{
    public interface IMainCategoriesService
    {
        Task<MainCategoryResponse> GetMainCategoriesAsync(MainCategoryRequest request);
        Task<string> GetMainCategoryNameBySubCategoryIdAsync(int id);
        Task<int> CreateMainCategoryAsync(AddEditMainCategoryModal request);
        Task<MainCategoryDropdownSelection[]> GetMainCategoriesForDropdownListAsync();
        Task<int> UpdateMainCategoryAsync(AddEditMainCategoryModal request);
        Task<bool> DeleteMainCategoryAsync(int id);
    }
    public class MainCategoriesService : IMainCategoriesService
    {
        private readonly IMainCategoriesRepository _mainCategoryRepo;
        private readonly ISubCategoryRepository _subCategoryRepository;
        private readonly IIncomeRepository _incomeRepo;

        public MainCategoriesService(IMainCategoriesRepository mainCategoryRepository, ISubCategoryRepository subCategoryRepository, IIncomeRepository incomeRepo)
        {
            _mainCategoryRepo = mainCategoryRepository;
            _subCategoryRepository = subCategoryRepository;
            _incomeRepo = incomeRepo;
        }

        public async Task<int> CreateMainCategoryAsync(AddEditMainCategoryModal request)
        {
            var categories = await _mainCategoryRepo.Find(x => true);
            if (categories.Any(x => x.Name == request.Name))
                throw new DuplicateNameException(request.Name);

            var categoryEntity = new MainCategoryEntity()
            {
                Name = request.Name,
            };

            return await _mainCategoryRepo.Create(categoryEntity);
        }

        public async Task<bool> DeleteMainCategoryAsync(int id)
        {
            var category = await _mainCategoryRepo.FindById(id);
            if (category == null)
                throw new CategoryNotFoundException(id.ToString());

            if (category.Name.IsEqualTo("Other"))
                throw new Exception("You cannot delete this category. It's kind of important.");
            var otherCategory = (await _mainCategoryRepo.Find(x => x.Name == "Other")).FirstOrDefault();
            if (otherCategory == null)
            {
                throw new CategoryNotFoundException("You need to create a new category called 'Other' before you can delete a main category. This will assigned all exepenses associated with this main category to the 'Other' category.");
            }
            var subCategories = await _subCategoryRepository.Find(x => x.MainCategoryId == id);
            foreach (var subCategory in subCategories)
            {
                subCategory.MainCategoryId = otherCategory.Id;
            }

            return await _mainCategoryRepo.Delete(category);
        }

        public async Task<MainCategoryResponse> GetMainCategoriesAsync(MainCategoryRequest request)
        {
            var expensePredicate = CategoryTimeOption<ExpenseEntity>.Parse(request);
            var subCategories = await _mainCategoryRepo.GetCategoriesWithExpenses(expensePredicate);
            var incomePredicate = CategoryTimeOption<IncomeEntity>.Parse(request);
            var totalIncome = await _incomeRepo.GetAmountOfIncome(incomePredicate);
            var overallTotal = subCategories.SelectMany(x => x.Expenses).Sum(x => x.Amount);

            var listItems = GetMainCategoryListItems(subCategories);

            return new MainCategoryResponse()
            {
                TotalMainCategories = listItems.Length,
                MainCategories = listItems,
                MainCategoryPercentages = GetMainCategoryPercentages(subCategories, expensePredicate),
                SubCategoryPercentages = GetSubCategoryPercentages(subCategories, expensePredicate, overallTotal),
                CategoryPurchaseOccurances = GetSubCategoryOccurances(subCategories, expensePredicate),
                MainCategoryChartData = GetDataForMainGraph(subCategories),
                SavingsPercentages = GetSavingsPercentage(totalIncome, overallTotal)
            };
        }
        public async Task<MainCategoryDropdownSelection[]> GetMainCategoriesForDropdownListAsync()
        {
            return (await _mainCategoryRepo.Find(x => true)).Select(x => new MainCategoryDropdownSelection()
            {
                Id = x.Id,
                Category = x.Name
            }).ToArray();
        }
        public async Task<string> GetMainCategoryNameBySubCategoryIdAsync(int id)
        {
            var subCategory = await _subCategoryRepository.FindById(id);
            if (subCategory == null)
                throw new CategoryNotFoundException(id.ToString());
            var mainCategory = await _mainCategoryRepo.FindById(subCategory.MainCategoryId);
            if (mainCategory == null)
                throw new CategoryNotFoundException(id.ToString());
            return mainCategory.Name;
        }
        public async Task<int> UpdateMainCategoryAsync(AddEditMainCategoryModal request)
        {
            var categories = await _mainCategoryRepo.Find(x => x.Name == request.Name);
            if (categories.Any(x => x.Id != request.Id))
                throw new DuplicateNameException(request.Name);

            var category = await _mainCategoryRepo.FindById(request.Id.Value);
            if (category == null)
                throw new CategoryNotFoundException(request.Id.Value.ToString());

            category.Name = request.Name;
            return await _mainCategoryRepo.Update(category);
        }
        private Dictionary<string, int> GetSavingsPercentage(decimal totalIncome, decimal totalExpense)
        {
            var total = totalIncome + totalExpense;
            if (totalIncome < totalExpense)
                return new Dictionary<string, int>()
                {
                    { "Expenses", 100},
                };
            var expensePercentage = (int)decimal.Round((totalExpense / totalIncome) * 100);

            return new Dictionary<string, int>()
            {
                { "Expenses", expensePercentage},
                { "Savings", Math.Abs(expensePercentage - 100)}
            };
        }
        private MainCategoryChartData GetDataForMainGraph(SubCategoryEntity[] subCategories)
        {
            var mainCategoryNames = subCategories.Where(x => x.Expenses.Count > 0).Select(x => x.MainCategory.Name).Distinct().OrderBy(x => x).ToArray();
            var chartData = subCategories.Where(x => x.Expenses.Count > 0).Select(x =>
            {
                var sumOfTotals = x.Expenses.Sum(x => x.Amount);
                var mainCategoryIndex = Array.IndexOf(mainCategoryNames, x.MainCategory.Name);
                return new SubCategoryAmountDataset(
                    x.Name,
                    mainCategoryNames.Length,
                    sumOfTotals,
                    mainCategoryIndex,
                    x.MainCategory.Id
                    );
            }).ToList();
            var coloredChartData = AssignColorsToChartData(chartData);
            return new MainCategoryChartData()
            {
                MainCategoryNames = mainCategoryNames,
                SubCategoryData = chartData
            };
        }

        private List<SubCategoryAmountDataset> AssignColorsToChartData(List<SubCategoryAmountDataset> chartData)
        {
            var chartDataWithColors = new List<SubCategoryAmountDataset>();
            for (int i = 1; i <= chartData.Count; i++)
            {
                var categoriesByMainCategory = chartData.Where(x => x.MainCategoryId == i).ToList();
                var coloredData = categoriesByMainCategory.Select((x, index) =>
                {
                    x.Color = GetColorForSubCategoryDataset(index);
                    return x;
                }).ToList();
                chartDataWithColors.AddRange(coloredData);
            }
            return chartDataWithColors;
        }
        private string GetColorForSubCategoryDataset(int index)
        {
            var colors = new[]
            {
                DarkChartColors.Red,
                DarkChartColors.Orange,
                DarkChartColors.Yellow,
                DarkChartColors.Green,
                DarkChartColors.Blue,
                DarkChartColors.Purple
            };
            if (index > colors.Length - 1)
            {
                var localIndex = index;
                while (localIndex > colors.Length - 1)
                    localIndex = (localIndex - colors.Length);
                return colors[localIndex];
            }
            else return colors[index];
        }
        private Dictionary<string, int> GetSubCategoryOccurances(SubCategoryEntity[] subCategories, Expression<Func<ExpenseEntity, bool>> predicate)
        {
            return subCategories.Select(x =>
            {
                var totalOfSubCategory = x.Expenses
                .AsQueryable()
                .Where(predicate)
                .Count();

                return (Name: x.Name, Count: totalOfSubCategory);
            }).Where(x => x.Count > 0).ToDictionary(k => k.Name, v => v.Count);
        }
        private Dictionary<string, int> GetSubCategoryPercentages(SubCategoryEntity[] subCategories, Expression<Func<ExpenseEntity, bool>> predicate, decimal overallTotal)
        {
            var amounts = subCategories.Select(x =>
            {
                var totalAmountofSubCategory = x.Expenses
                .AsQueryable()
                .Where(predicate)
                .Sum(x => x.Amount);

                return (Name: x.Name, Amount: totalAmountofSubCategory);
            }).ToDictionary(k => k.Name, v => v.Amount);
            return amounts.Select(x =>
            {
                var percentageOfTotalForCategory =
                (int)decimal.Round((x.Value / overallTotal) * 100);
                return (Name: x.Key, Percentage: percentageOfTotalForCategory);
            }).Where(x => x.Percentage > 0).ToDictionary(k => k.Name, v => v.Percentage);
        }
        private Dictionary<string, int> GetMainCategoryPercentages(SubCategoryEntity[] subCategories, Expression<Func<ExpenseEntity, bool>> predicate)
        {
            var mainCategoryAmounts = subCategories.GroupBy(x => x.MainCategory.Name).Select(x =>
            {
                return (Name: x.Key, Amount: x.SelectMany(x => x.Expenses)
                .AsQueryable()
                .Where(predicate)
                .Sum(x => x.Amount));
            }).ToDictionary(k => k.Name, v => v.Amount);
            var total = mainCategoryAmounts.Values.Sum();
            return mainCategoryAmounts.Where(x => x.Value > 0).Select(x =>
            {
                return (Name: x.Key, Percentage: (int)decimal.Round((x.Value / total) * 100));
            }).Where(x => x.Percentage > 0).ToDictionary(k => k.Name, v => v.Percentage);

        }
        private MainCategoryListItem[] GetMainCategoryListItems(SubCategoryEntity[] subCategories)
        {
            return subCategories.GroupBy(x => x.MainCategory.Id).Select(g =>
            {
                var subCategoriesByMainCategory = subCategories.Where(x => x.MainCategoryId == g.Key).ToList();

                var amounts = subCategoriesByMainCategory.Select(x => (Name: x.Name, Amount: x.Expenses.Sum(x => x.Amount))).ToDictionary(k => k.Name, v => v.Amount);

                var mainCategoryTotal = (int)decimal.Round(amounts.Values.Sum(), 0);

                var subCategoryPercentages = amounts.Where(x => x.Value > 0).Select(x => (Name: x.Key, Percentage: (int)decimal.Round((x.Value / mainCategoryTotal) * 100))).ToDictionary(k => k.Name, v => v.Percentage);
                return new MainCategoryListItem()
                {
                    Id = g.Key,
                    Name = subCategoriesByMainCategory.First().MainCategory.Name,
                    NumberOfSubCategories = subCategoryPercentages.Count,
                    SubCategoryExpenses = subCategoryPercentages
                };
            }).Where(x => x.NumberOfSubCategories > 0).OrderBy(x => x.Name).ToArray();
        }
    }
}
