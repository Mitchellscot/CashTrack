using CashTrack.Data.CsvFiles;
using CashTrack.Data.Entities;
using System.Collections.Generic;
using System;
using static Azure.Core.HttpHeader;
using System.Linq;

namespace CashTrack.Data
{
    public static class GenerateData
    {
        private static int FiveYearsAgo => DateTime.Now.AddYears(-5).Year;
        private static int FourYearsAgo => DateTime.Now.AddYears(-4).Year;
        private static int ThreeYearsAgo => DateTime.Now.AddYears(-3).Year;
        private static int TwoYearsAgo => DateTime.Now.AddYears(-2).Year;
        private static int LastYear => DateTime.Now.AddYears(-1).Year;
        private static int CurrentYear => DateTime.Now.Year;
        private static int CurrentMonth => DateTime.Now.Month;
        private static int CurrentDay => DateTime.Now.Day;
        private static int CurrentExpenseId { get; set; }
        private static int CurrentIncomeId { get; set; }
        public static List<IncomeEntity> Income(List<CsvModels.CsvIncomeCategory> incomeCategories)
        {
            var incomes = new List<IncomeEntity>();
            foreach (var category in incomeCategories)
            {
                switch (category.Name)
                {
                    case "Paycheck":
                        {
                            var basePay = 1529M;
                            for (int i = 1; i < 13; i++)
                            {
                                incomes.Add(GetIncome(basePay, new DateTime(FiveYearsAgo, i, 1), 3, 1, string.Empty));
                                incomes.Add(GetIncome(basePay, new DateTime(FiveYearsAgo, i, 15), 3, 1, string.Empty));
                                incomes.Add(GetIncome(basePay + 100, new DateTime(FourYearsAgo, i, 1), 3, 1, string.Empty));
                                incomes.Add(GetIncome(basePay + 100, new DateTime(FourYearsAgo, i, 15), 3, 1, string.Empty));
                                incomes.Add(GetIncome(basePay + 150, new DateTime(ThreeYearsAgo, i, 1), 3, 1, string.Empty));
                                incomes.Add(GetIncome(basePay + 150, new DateTime(ThreeYearsAgo, i, 15), 3, 1, string.Empty));
                                incomes.Add(GetIncome(basePay + 250, new DateTime(TwoYearsAgo, i, 1), 3, 1, string.Empty));
                                incomes.Add(GetIncome(basePay + 250, new DateTime(TwoYearsAgo, i, 15), 3, 1, string.Empty));
                                incomes.Add(GetIncome(basePay + 350, new DateTime(LastYear, i, 1), 3, 1, string.Empty));
                                incomes.Add(GetIncome(basePay + 350, new DateTime(LastYear, i, 15), 3, 1, string.Empty));

                            }
                            for (int i = 1; i < CurrentMonth; i++)
                            {
                                incomes.Add(GetIncome(basePay + 400, new DateTime(CurrentYear, i, 1), 3, 1, string.Empty));
                                incomes.Add(GetIncome(basePay + 400, new DateTime(CurrentYear, i, 15), 3, 1, string.Empty));
                                if (i == CurrentMonth && CurrentDay < 15)
                                {
                                    incomes.Add(GetIncome(basePay + 400, new DateTime(CurrentYear, i, 1), 3, 1, string.Empty));
                                }
                                if (i == CurrentMonth && CurrentDay > 15)
                                {
                                    incomes.Add(GetIncome(basePay + 400, new DateTime(CurrentYear, i, 15), 3, 1, string.Empty));
                                }
                            }

                        }
                        break;
                    case "Gift":
                        {
                            incomes.Add(GetIncome(100M, new DateTime(FiveYearsAgo, 1, 11), 4, 3, "Birthday Money from Parents"));
                            incomes.Add(GetIncome(200M, new DateTime(FiveYearsAgo, 12, 25), 4, 3, "Christmas money from Parents"));
                            incomes.Add(GetIncome(100M, new DateTime(FourYearsAgo, 1, 11), 4, 3, "Birthday Money from Parents"));
                            incomes.Add(GetIncome(200M, new DateTime(FourYearsAgo, 12, 25), 4, 3, "Christmas money from Parents"));
                            incomes.Add(GetIncome(100M, new DateTime(ThreeYearsAgo, 1, 11), 4, 3, "Birthday Money from Parents"));
                            incomes.Add(GetIncome(200M, new DateTime(ThreeYearsAgo, 12, 25), 4, 3, "Christmas money from Parents"));
                            incomes.Add(GetIncome(100M, new DateTime(TwoYearsAgo, 1, 11), 4, 3, "Birthday Money from Parents"));
                            incomes.Add(GetIncome(200M, new DateTime(TwoYearsAgo, 12, 25), 4, 3, "Christmas money from Parents"));
                            incomes.Add(GetIncome(100M, new DateTime(LastYear, 1, 11), 4, 3, "Birthday Money from Parents"));
                            incomes.Add(GetIncome(200M, new DateTime(LastYear, 12, 25), 4, 3, "Christmas money from Parents"));

                            if (CurrentMonth >= 1 && CurrentDay >= 11)
                            {
                                incomes.Add(GetIncome(100M, new DateTime(CurrentYear, 1, 11), 4, 3, "Birthday Money from Parents, again"));
                            }
                            if (CurrentMonth >= 12 && CurrentDay > 24)
                            {
                                incomes.Add(GetIncome(200M, new DateTime(CurrentYear, 12, 25), 4, 3, "Christmas money from Parents"));
                            }


                        }
                        break;
                    case "Tax Refund":
                        {
                            incomes.Add(GetIncome(2123M, new DateTime(FiveYearsAgo, 4, 14), 5, 2, "Federal Tax Refund"));
                            incomes.Add(GetIncome(2246M, new DateTime(FourYearsAgo, 4, 10), 5, 2, "Federal Tax Refund"));
                            incomes.Add(GetIncome(2389M, new DateTime(ThreeYearsAgo, 4, 1), 5, 2, "Federal Tax Refund"));
                            incomes.Add(GetIncome(2002M, new DateTime(TwoYearsAgo, 4, 5), 5, 2, "Federal Tax Refund"));
                            incomes.Add(GetIncome(2443M, new DateTime(LastYear, 4, 8), 5, 2, "Federal Tax Refund"));


                            if (CurrentMonth >= 4 && CurrentDay > 8)
                            {
                                incomes.Add(GetIncome(1918M, new DateTime(CurrentYear, 4, 7), 5, 2, "Federal Tax Refund"));
                            }
                        }
                        break;
                }
            }
            return incomes;
        }
        public static List<ExpenseEntity> Expenses(List<CsvModels.CsvExpenseSubCategory> categories)
        {
            var expenses = new List<ExpenseEntity>();
            CurrentExpenseId = 1;
            var rando = new Random();
            foreach (var category in categories)
            {
                switch (category.Name)
                {
                    case "Groceries":
                        {
                            var min = 40;
                            var max = 200;
                            var timesAMonth = 3;
                            expenses.AddRange(GenerateExpensesAcrossYears(rando, category.Id, new[] { 1, 2, 3, 1, 1, 2 }, min, max, 0, timesAMonth));
                        }
                        break;
                    case "Gas":
                        {

                            var min = 10;
                            var max = 75;
                            var timesAMonth = 3;
                            expenses.AddRange(GenerateExpensesAcrossYears(rando, category.Id, new[] { 4, 5, 6, 7, 4, 4, 5, 6, 6, 7, 8 }, min, max, 0, timesAMonth));
                        }
                        break;
                    case "Dining Out":
                        {
                            var min = 10;
                            var max = 50;
                            var timesAMonth = 4;
                            expenses.AddRange(GenerateExpensesAcrossYears(rando, category.Id,
                                Enumerable.Range(9, 50).ToArray(),
                                min, max, 0, timesAMonth));
                        }
                        break;
                    case "Rent":
                        {
                            var min = 1000;
                            var max = 1000;
                            var timesAMonth = 1;
                            expenses.AddRange(GenerateExpensesAcrossYears(rando, category.Id,
                                new[] { 59 },
                                min, max, 1, timesAMonth));
                        }
                        break;
                    case "Car Repairs":
                        {
                            var min = 100;
                            var max = 1200;
                            expenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 6, new int[] { 60, 61 }, "Repair for my car", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(FourYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 6, new int[] { 60, 61 }, "Repair for my vehicle", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 6, new int[] { 60, 61 }, "car repairs", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 6, new int[] { 60, 61 }, "vehicle repairs", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(LastYear, rando.Next(1, 12), rando.Next(1, 28)), 6, new int[] { 60, 61 }, "Repair for car", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(CurrentYear, rando.Next(1, CurrentMonth), rando.Next(1, CurrentDay)), 6, new int[] { 60, 61 }, "Repair for my vehicle", 0, rando));
                        }
                        break;
                    case "Books":
                        {
                            var min = 10;
                            var max = 35;
                            expenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 7, new int[] { 62, 63, 64, 65 }, "My Favorite book", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 7, new int[] { 62, 63, 64, 65 }, "My Favorite book", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 7, new int[] { 62, 63, 64, 65 }, "My Favorite book", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(FourYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 7, new int[] { 62, 63, 64, 65 }, "books to read", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(FourYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 7, new int[] { 62, 63, 64, 65 }, "books to read", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 7, new int[] { 62, 63, 64, 65 }, "My Favorite book to read", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 7, new int[] { 62, 63, 64, 65 }, "My Favorite book to read", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 7, new int[] { 62, 63, 64, 65 }, "My Favorite book to read", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 7, new int[] { 62, 63, 64, 65 }, "some books", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 7, new int[] { 62, 63, 64, 65 }, "some books", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(LastYear, rando.Next(1, 12), rando.Next(1, 28)), 7, new int[] { 62, 63, 64, 65 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(LastYear, rando.Next(1, 12), rando.Next(1, 28)), 7, new int[] { 62, 63, 64, 65 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(CurrentYear, rando.Next(1, CurrentMonth), rando.Next(1, CurrentDay)), 7, new int[] { 62, 63, 64, 65 }, "book stuff", 0, rando));
                        }
                        break;
                    case "Music":
                        {
                            var min = 11;
                            var max = 11;
                            var timesAMonth = 1;
                            expenses.AddRange(GenerateExpensesAcrossYears(rando, category.Id,
                                new[] { 66 },
                                min, max, 11, timesAMonth, false));
                        }
                        break;
                    case "Movies":
                        {
                            var min = 10;
                            var max = 35;
                            expenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 9, new int[] { 67 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 9, new int[] { 67 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 9, new int[] { 67 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(FourYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 9, new int[] { 67 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(FourYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 9, new int[] { 67 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 9, new int[] { 67 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 9, new int[] { 67 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 9, new int[] { 67 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 9, new int[] { 67 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 9, new int[] { 67 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(LastYear, rando.Next(1, 12), rando.Next(1, 28)), 9, new int[] { 67 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(LastYear, rando.Next(1, 12), rando.Next(1, 28)), 9, new int[] { 67 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(CurrentYear, rando.Next(1, CurrentMonth), rando.Next(1, CurrentDay)), 9, new int[] { 67 }, "", 0, rando));
                        }
                        break;
                    case "Airfare":
                        {
                            var min = 250;
                            var max = 750;
                            expenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 10, new int[] { 68 }, "Airplane trip", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(FourYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 10, new int[] { 68 }, "Trip across the country", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 10, new int[] { 68 }, "Airplane tickets", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 10, new int[] { 68 }, "vacation", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(LastYear, rando.Next(1, 12), rando.Next(1, 28)), 10, new int[] { 68 }, "Trip To Minnesota", 0, rando));
                        }
                        break;
                    case "Car Insurance":
                        {
                            var min = 200;
                            var max = 250;
                            expenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, 3, rando.Next(1, 28)), 11, new int[] { 69 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, 9, rando.Next(1, 28)), 11, new int[] { 69 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(FourYearsAgo, 3, rando.Next(1, 28)), 11, new int[] { 69 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(FourYearsAgo, 9, rando.Next(1, 28)), 11, new int[] { 69 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, 3, rando.Next(1, 28)), 11, new int[] { 69 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, 9, rando.Next(1, 28)), 11, new int[] { 69 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, 3, rando.Next(1, 28)), 11, new int[] { 69 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, 9, rando.Next(1, 28)), 11, new int[] { 69 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(LastYear, 3, rando.Next(1, 28)), 11, new int[] { 69 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(LastYear, 9, rando.Next(1, 28)), 11, new int[] { 69 }, "", 0, rando));
                            if (CurrentMonth > 3)
                            {
                                expenses.Add(GetExpense(min, max, new DateTime(CurrentYear, 3, rando.Next(1, 28)), 11, new int[] { 69 }, "", 0, rando));
                            }
                            if (CurrentMonth > 9)
                            {
                                expenses.Add(GetExpense(min, max, new DateTime(CurrentYear, 9, rando.Next(1, 28)), 11, new int[] { 69 }, "", 0, rando));
                            }
                        }
                        break;
                }
            }
            return expenses;

            static List<ExpenseEntity> GenerateExpensesAcrossYears(
                Random rando,
                int category,
                int[] merchantIds,
                int min,
                int max,
                int day = 0,
                int numberToGenerateInAMonth = 1,
                bool incrementPrice = true,
                string notes = "")
            {
                var listOfExpenses = new List<ExpenseEntity>();

                switch (numberToGenerateInAMonth)
                {
                    case 1:
                        {
                            for (int i = 1; i < 13; i++)
                            {
                                day = day > 0 ? day : rando.Next(1, 28);
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, i, day), category, merchantIds, notes, 0, rando, incrementPrice));

                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FourYearsAgo, i, day), category, merchantIds, notes, incrementPrice ? incrementPrice ? incrementPrice ? 0.1M : 0 : 0 : 0, rando, incrementPrice));

                                listOfExpenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, i, day), category, merchantIds, notes, incrementPrice ? 0.1M : 0, rando, incrementPrice));

                                listOfExpenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, i, day), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, incrementPrice));

                                listOfExpenses.Add(GetExpense(min, max, new DateTime(LastYear, i, day), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, incrementPrice));
                            }
                            for (int i = 1; i <= CurrentMonth; i++)
                            {
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, day), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, incrementPrice));
                                if (i == CurrentMonth && CurrentDay > day)
                                {
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, day), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, incrementPrice));
                                }
                            }
                        }
                        break;
                    case 2:
                        {
                            for (int i = 1; i < 13; i++)
                            {
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, i, rando.Next(1, 14)), category, merchantIds, notes, 0, rando, incrementPrice));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, i, rando.Next(15, 28)), category, merchantIds, notes, 0, rando, incrementPrice));

                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FourYearsAgo, i, rando.Next(1, 14)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, incrementPrice));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FourYearsAgo, i, rando.Next(15, 28)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, incrementPrice));

                                listOfExpenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, i, rando.Next(1, 14)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, incrementPrice));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, i, rando.Next(15, 28)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, incrementPrice));

                                listOfExpenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, i, rando.Next(1, 14)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, incrementPrice));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, i, rando.Next(15, 28)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, incrementPrice));

                                listOfExpenses.Add(GetExpense(min, max, new DateTime(LastYear, i, rando.Next(1, 14)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, incrementPrice));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(LastYear, i, rando.Next(15, 28)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, incrementPrice));

                            }
                            for (int i = 1; i <= CurrentMonth; i++)
                            {
                                if (i < CurrentMonth)
                                {
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(1, 14)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, incrementPrice));
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(15, 28)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, incrementPrice));

                                }
                                if (i == CurrentMonth && CurrentDay > 15)
                                {
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(1, 14)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, incrementPrice));
                                }
                                if (i == CurrentMonth && CurrentDay > 28)
                                {
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(15, 28)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, incrementPrice));
                                }
                            }
                        }
                        break;
                    case 3:
                        {
                            for (int i = 1; i < 13; i++)
                            {
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, i, rando.Next(1, 9)), category, merchantIds, notes, 0, rando, incrementPrice));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, i, rando.Next(10, 19)), category, merchantIds, notes, 0, rando, incrementPrice));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, i, rando.Next(20, 28)), category, merchantIds, notes, 0, rando, incrementPrice));

                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FourYearsAgo, i, rando.Next(1, 9)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, incrementPrice));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FourYearsAgo, i, rando.Next(10, 19)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, incrementPrice));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FourYearsAgo, i, rando.Next(20, 28)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, incrementPrice));

                                listOfExpenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, i, rando.Next(1, 9)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, incrementPrice));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, i, rando.Next(10, 19)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, incrementPrice));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, i, rando.Next(20, 28)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, incrementPrice));

                                listOfExpenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, i, rando.Next(1, 9)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, incrementPrice));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, i, rando.Next(10, 19)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, incrementPrice));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, i, rando.Next(20, 28)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, incrementPrice));

                                listOfExpenses.Add(GetExpense(min, max, new DateTime(LastYear, i, rando.Next(1, 9)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, incrementPrice));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(LastYear, i, rando.Next(10, 19)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, incrementPrice));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(LastYear, i, rando.Next(20, 28)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, incrementPrice));

                            }
                            for (int i = 1; i <= CurrentMonth; i++)
                            {
                                if (i < CurrentMonth)
                                {
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(1, 9)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, incrementPrice));
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(10, 19)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, incrementPrice));
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(20, 28)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, incrementPrice));
                                }
                                if (i == CurrentMonth && CurrentDay > 10)
                                {
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(1, 9)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, incrementPrice));
                                }
                                if (i == CurrentMonth && CurrentDay > 20)
                                {
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(10, 19)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, incrementPrice));
                                }
                                if (i == CurrentMonth && CurrentDay > 28)
                                {
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(20, 28)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, incrementPrice));
                                }

                            }
                        }
                        break;
                    case 4:
                        {
                            for (int i = 1; i < 13; i++)
                            {
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, i, rando.Next(1, 7)), category, merchantIds, notes, 0, rando, incrementPrice));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, i, rando.Next(8, 14)), category, merchantIds, notes, 0, rando, incrementPrice));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, i, rando.Next(15, 21)), category, merchantIds, notes, 0, rando, incrementPrice));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, i, rando.Next(22, 28)), category, merchantIds, notes, 0, rando, incrementPrice));

                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FourYearsAgo, i, rando.Next(1, 7)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, incrementPrice));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FourYearsAgo, i, rando.Next(8, 14)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, incrementPrice));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FourYearsAgo, i, rando.Next(15, 21)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, incrementPrice));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FourYearsAgo, i, rando.Next(22, 28)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, incrementPrice));

                                listOfExpenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, i, rando.Next(1, 7)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, incrementPrice));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, i, rando.Next(8, 14)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, incrementPrice));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, i, rando.Next(15, 21)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, incrementPrice));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, i, rando.Next(22, 28)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, incrementPrice));

                                listOfExpenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, i, rando.Next(1, 7)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, incrementPrice));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, i, rando.Next(8, 14)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, incrementPrice));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, i, rando.Next(15, 21)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, incrementPrice));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, i, rando.Next(22, 28)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, incrementPrice));

                                listOfExpenses.Add(GetExpense(min, max, new DateTime(LastYear, i, rando.Next(1, 7)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, incrementPrice));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(LastYear, i, rando.Next(8, 14)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, incrementPrice));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(LastYear, i, rando.Next(15, 21)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, incrementPrice));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(LastYear, i, rando.Next(22, 28)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, incrementPrice));
                            }
                            for (int i = 1; i <= CurrentMonth; i++)
                            {
                                if (i < CurrentMonth)
                                {
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(1, 7)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, incrementPrice));
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(8, 14)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, incrementPrice));
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(15, 21)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, incrementPrice));
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(22, 28)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, incrementPrice));
                                }
                                if (i == CurrentMonth && CurrentDay > 7)
                                {
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(1, 7)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, incrementPrice));
                                }
                                if (i == CurrentMonth && CurrentDay > 15)
                                {
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(8, 14)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, incrementPrice));
                                }
                                if (i == CurrentMonth && CurrentDay > 23)
                                {
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(15, 21)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, incrementPrice));
                                }
                                if (i == CurrentMonth && CurrentDay > 28)
                                {
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(22, 28)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, incrementPrice));
                                }
                            }
                        }
                        break;
                }
                return listOfExpenses;
            }
        }

        private static ExpenseEntity GetExpense(int min, int max, DateTime dateTime, int category, int[] merchantIds, string notes, decimal percentChange, Random rando, bool randomizeCents = true)
        {
            var amount = randomizeCents ? Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2) : Math.Round(Convert.ToDecimal(rando.Next(min, max)), 2);
            amount = percentChange > 0 ? Math.Round((amount * percentChange) + amount, 2) : amount;
            CurrentExpenseId++;
            var expense = new ExpenseEntity()
            {
                Id = CurrentExpenseId,
                Amount = amount,
                Date = dateTime,
                CategoryId = category,
                MerchantId = RandomMerchantId(merchantIds, rando),
                Notes = notes
            };
            return expense;
        }
        private static IncomeEntity GetIncome(decimal amount, DateTime dateTime, int category, int sourceId, string notes)
        {
            CurrentIncomeId++;
            var income = new IncomeEntity()
            {
                Id = CurrentIncomeId,
                Amount = amount,
                Date = dateTime,
                CategoryId = category,
                SourceId = sourceId,
                Notes = notes,
                IsRefund = false
            };
            return income;
        }

        private static int RandomMerchantId(int[] numbers, Random rando)
        {
            var index = rando.Next(0, numbers.Length);
            return numbers[index];
        }
    }

}