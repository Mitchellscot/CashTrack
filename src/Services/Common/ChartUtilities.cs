using CashTrack.Common;
using CashTrack.Data.Entities;
using CashTrack.Models.BudgetModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CashTrack.Services.Common
{
    public static class ChartUtilities
    {
        public static string[] GenerateMonthlyChartLabels(bool incomeExists, string[] mainCategoryLabels, bool savingsExists, bool unallocatedExists = false)
        {
            Queue<string> data = new Queue<string>();
            if (incomeExists)
                data.Enqueue("Income");

            mainCategoryLabels.OrderBy(x => x).ToList().ForEach(x => data.Enqueue(x));

            if (savingsExists)
                data.Enqueue("Savings");

            if (unallocatedExists)
                data.Enqueue("Unallocated");

            return data.ToArray();
        }
        public static int[] GetMonthlyIncomeData(int incomeAmount, int numberOfMainCategories, bool savingsExists, bool unallocatedExists = false)
        {
            Queue<int> data = new Queue<int>();

            if (incomeAmount > 0)
                data.Enqueue(incomeAmount);

            Enumerable.Repeat(0, numberOfMainCategories).ToList().ForEach(x => data.Enqueue(x));

            if (savingsExists)
                data.Enqueue(0);

            if (unallocatedExists)
                data.Enqueue(0);

            return data.ToArray();
        }
        public static int[] GetMonthlySavingsData(bool incomeExists, int numberOfMainCategories, int adjustedSavingsAmount, bool unallocatedExists = false)
        {
            Queue<int> data = new Queue<int>();
            if (incomeExists)
                data.Enqueue(0);

            Enumerable.Repeat(0, numberOfMainCategories).ToList().ForEach(x => data.Enqueue(x));

            if (adjustedSavingsAmount != 0)
                data.Enqueue(adjustedSavingsAmount);

            if (unallocatedExists)
                data.Enqueue(0);

            return data.ToArray();
        }
        public static int[] GetMonthlyUnallocatedData(bool incomeExists, int numberOfMainCategories, bool savingsExists, int unallocatedAmount)
        {
            Queue<int> data = new Queue<int>();
            if (incomeExists)
                data.Enqueue(0);

            Enumerable.Repeat(0, numberOfMainCategories).ToList().ForEach(x => data.Enqueue(x));

            if (savingsExists)
                data.Enqueue(0);

            if (unallocatedAmount > 0)
                data.Enqueue(unallocatedAmount);

            return data.ToArray();
        }

        public static List<ExpenseDataset> GetMonthlyBudgetExpenseData(BudgetEntity[] budgets, bool incomeExists, bool savingsExists, bool unallocatedExists, string[] mainCategoryLabels)
        {
            var arraySize = mainCategoryLabels.Length;
            arraySize = incomeExists ? arraySize + 1 : arraySize;
            arraySize = savingsExists ? arraySize + 1 : arraySize;
            arraySize = unallocatedExists ? arraySize + 1 : arraySize;

            var amountsAndLabels = budgets.Where(x => x.SubCategoryId != null && x.BudgetType == BudgetType.Need || x.BudgetType == BudgetType.Want && x.Amount > 0).GroupBy(x => x.SubCategory.Name)
                .Select(x => (x.Key,
                Amount: x.Sum(x => x.Amount),
                MainCategory: x.Select(x =>
                    x.SubCategory.MainCategory.Name).FirstOrDefault(),
                MainCategoryId: x.Select(x => x.SubCategory.MainCategoryId).FirstOrDefault()
                )).OrderBy(x => x.Key).ToList();
            var expenseList = new List<ExpenseDataset>();
            foreach (var expense in amountsAndLabels)
            {
                var adjustIndexForIncome = incomeExists ? 1 : 0;
                var index = Array.IndexOf(mainCategoryLabels, expense.MainCategory);
                var dataSet = new int[arraySize];
                dataSet[index + adjustIndexForIncome] = expense.Amount;
                var data = new ExpenseDataset()
                {
                    DataSet = dataSet,
                    SubCategoryName = expense.Key,
                    MainCategoryId = expense.MainCategoryId
                };
                expenseList.Add(data);
            }
            return AssignColorsToChartData(expenseList);
        }
        public static List<ExpenseDataset> AssignColorsToChartData(List<ExpenseDataset> chartData)
        {
            var chartDataWithColors = new List<ExpenseDataset>();
            var mainCategories = chartData.Select(x => x.MainCategoryId).Distinct().ToList();
            foreach (var id in mainCategories)
            {
                var matchingExpenses = chartData.Where(x => x.MainCategoryId == id);
                var coloredData = matchingExpenses.Select((x, index) =>
                {
                    x.Color = GetColorForExpenseDataset(index);
                    return x;
                }).ToList();
                chartDataWithColors.AddRange(coloredData);
            }
            return chartDataWithColors;
        }
        public static string GetColorForExpenseDataset(int index)
        {
            var colors = new[]
            {
                LightChartColors.Pink,
                LightChartColors.Orange,
                LightChartColors.Yellow,
                LightChartColors.Cyan,
                LightChartColors.Azure,
                LightChartColors.Purple
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
    }
}
