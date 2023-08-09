using CashTrack.Common;
using CashTrack.Data.Entities;
using CashTrack.Models.BudgetModels;
using CashTrack.Models.SummaryModels;
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

        public static List<ExpenseDataset> GetMonthlyBudgetExpenseData(BudgetEntity[] budgets, bool incomeExists, bool savingsExists, string[] mainCategoryLabels, bool unallocatedExists = false)
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
                    SubCategoryName = $"Budgeted {expense.Key}",
                    MainCategoryId = expense.MainCategoryId
                };
                expenseList.Add(data);
            }
            return AssignColorsToChartData(expenseList);
        }
        public static List<ExpenseDataset> GetMonthlySummaryExpenseData(ExpenseEntity[] expenses, bool incomeExists, bool savingsExists, string[] mainCategoryLabels, bool unallocatedExists = false)
        {
            var arraySize = mainCategoryLabels.Length;
            arraySize = incomeExists ? arraySize + 1 : arraySize;
            arraySize = savingsExists ? arraySize + 1 : arraySize;
            arraySize = unallocatedExists ? arraySize + 1 : arraySize;

            var amountsAndLabels = expenses.Where(x => x.Amount > 0).GroupBy(x => x.Category.Name)
                .Select(x => (x.Key,
                Amount: x.Sum(x => x.Amount),
                MainCategory: x.Select(x =>
                    x.Category.MainCategory.Name).FirstOrDefault(),
                MainCategoryId: x.Select(x => x.Category.MainCategoryId).FirstOrDefault()
                )).OrderBy(x => x.Key).ToList();
            var expenseList = new List<ExpenseDataset>();
            foreach (var expense in amountsAndLabels)
            {
                var adjustIndexForIncome = incomeExists ? 1 : 0;
                var index = Array.IndexOf(mainCategoryLabels, expense.MainCategory);
                var dataSet = new int[arraySize];
                dataSet[index + adjustIndexForIncome] = (int)decimal.Round(expense.Amount, 0);
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
        public static List<DailyExpenseDataset> GetDailyExpenseData(ExpenseEntity[] expenses, bool isAnnual)
        {
            if (!expenses.Any())
                return new List<DailyExpenseDataset>();

            var month = expenses.OrderBy(x => x.Date).Select(x => x.Date.Month).FirstOrDefault();
            var year = expenses.FirstOrDefault().Date.Year;
            var days = isAnnual ? Enumerable.Range(1, DateTime.IsLeapYear(year) ? 366 : 365).ToArray() : Enumerable.Range(1, DateTime.DaysInMonth(year, month)).ToArray();

            var daysAndExpenses = expenses.Where(x => x.Amount > 0 && !x.ExcludeFromStatistics)
                .GroupBy(x => x.Date.Day).Select(x => (Day: x.Key, Amounts: x.OrderBy(x => x.Amount).Select(x => x.Amount).ToArray()));

            var listOfDaysAndAmounts = new List<(int Day, decimal[] Amounts)>();

            Array.ForEach(days, x => listOfDaysAndAmounts.Add(
                (Day: x, daysAndExpenses.FirstOrDefault(y => y.Day == x).Amounts)));

            int maxExpensesInADay = daysAndExpenses.Max(x => x.Amounts.Length);

            var expenseList = new List<DailyExpenseDataset>();
            for (int i = 1; i <= maxExpensesInADay; i++)
            { 
                var dataset = new decimal[days.Length];
                for (int d = 1; d < days.Length; d++)
                { 
                    var daysExpenses = listOfDaysAndAmounts.FirstOrDefault(x => x.Day == d).Amounts;

                        if (daysExpenses is not null && i <= daysExpenses.Length)
                            dataset[d - 1] = daysExpenses[i - 1];
                        else
                            dataset[d - 1] = 0;
                }
                expenseList.Add(new DailyExpenseDataset() 
                {
                    DataSet = dataset,
                    Day = i,
                    Color = GetColorForDailyExpenseDataset(i -1)
                });
            }
            return expenseList;
        }
        public static string GetColorForDailyExpenseDataset(int index)
        {
            var colors = new[]
            {
                Qualitative12Set.Red,
                Qualitative12Set.Orange,
                Qualitative12Set.LightOrange,
                Qualitative12Set.Yellow,
                Qualitative12Set.LightGreen,
                Qualitative12Set.Green,
                Qualitative12Set.Blue,
                Qualitative12Set.LightBlue,
                Qualitative12Set.LightPurple,
                Qualitative12Set.Purple,
                Qualitative12Set.Pink,
                Qualitative12Set.Brown
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
                Qualitative6Set.Red,
                Qualitative6Set.Orange,
                Qualitative6Set.Yellow,
                Qualitative6Set.Green,
                Qualitative6Set.Blue,
                Qualitative6Set.Purple
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
