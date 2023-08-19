using CashTrack.Data.CsvFiles;
using CashTrack.Data.Entities;
using System.Collections.Generic;
using System;
using System.Linq;
using CashTrack.Models.BudgetModels;

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
        private static int CurrentBudgetId { get; set; }
        private static decimal BasePay { get; set; } = 1529M;
        public static List<BudgetEntity> IncomeBudgets()
        {
            var incomeBudgets = new List<BudgetEntity>();
            for (int i = 1; i <= 6; i++)
            {
                var incomeAmount = Convert.ToInt32(BasePay);
                var year = 0;
                var rent = 1000;
                var groc = 525;
                var dinOut = 125;
                var gas = 125;
                var healthIns = 284;
                var elec = 121;
                var internet = 25;
                var phone = 35;
                var natGas = 48;
                var water = 55;
                var carIns = 450;
                switch (i)
                {
                    case 1:
                        year = FiveYearsAgo;
                        break;
                    case 2:
                        year = FourYearsAgo;
                        incomeAmount = incomeAmount + 100;
                        rent = Convert.ToInt32(((decimal)rent * 0.1M)) + rent;
                        groc = Convert.ToInt32(((decimal)groc * 0.1m)) + groc;
                        dinOut = Convert.ToInt32(((decimal)dinOut * 0.1m)) + dinOut;
                        gas = Convert.ToInt32(((decimal)gas * 0.1m)) + gas;
                        healthIns = Convert.ToInt32(((decimal)healthIns * 0.1m)) + healthIns;
                        elec = Convert.ToInt32(((decimal)elec * 0.1m)) + elec;
                        phone = Convert.ToInt32(((decimal)phone * 0.1m)) + phone;
                        natGas = Convert.ToInt32(((decimal)natGas * 0.1m)) + natGas;
                        water = Convert.ToInt32(((decimal)water * 0.1m)) + water;
                        carIns = Convert.ToInt32(((decimal)carIns * 0.1m)) + carIns;
                        break;
                    case 3:
                        year = ThreeYearsAgo;
                        rent = Convert.ToInt32(((decimal)rent * 0.1M)) + rent;
                        groc = Convert.ToInt32(((decimal)groc * 0.1m)) + groc;
                        dinOut = Convert.ToInt32(((decimal)dinOut * 0.1m)) + dinOut;
                        gas = Convert.ToInt32(((decimal)gas * 0.1m)) + gas;
                        healthIns = Convert.ToInt32(((decimal)healthIns * 0.1m)) + healthIns;
                        elec = Convert.ToInt32(((decimal)elec * 0.1m)) + elec;
                        phone = Convert.ToInt32(((decimal)phone * 0.1m)) + phone;
                        natGas = Convert.ToInt32(((decimal)natGas * 0.1m)) + natGas;
                        water = Convert.ToInt32(((decimal)water * 0.1m)) + water;
                        carIns = Convert.ToInt32(((decimal)carIns * 0.1m)) + carIns;
                        incomeAmount = incomeAmount + 150;
                        break;
                    case 4:
                        year = TwoYearsAgo;
                        rent = Convert.ToInt32(((decimal)rent * 0.2M)) + rent;
                        groc = Convert.ToInt32(((decimal)groc * 0.2m)) + groc;
                        dinOut = Convert.ToInt32(((decimal)dinOut * 0.2m)) + dinOut;
                        gas = Convert.ToInt32(((decimal)gas * 0.2m)) + gas;
                        healthIns = Convert.ToInt32(((decimal)healthIns * 0.2m)) + healthIns;
                        elec = Convert.ToInt32(((decimal)elec * 0.2m)) + elec;
                        phone = Convert.ToInt32(((decimal)phone * 0.2m)) + phone;
                        natGas = Convert.ToInt32(((decimal)natGas * 0.2m)) + natGas;
                        water = Convert.ToInt32(((decimal)water * 0.2m)) + water;
                        carIns = Convert.ToInt32(((decimal)carIns * 0.2m)) + carIns;
                        incomeAmount = incomeAmount + 250;
                        break;
                    case 5:
                        year = LastYear;
                        rent = Convert.ToInt32(((decimal)rent * 0.2M)) + rent;
                        groc = Convert.ToInt32(((decimal)groc * 0.2m)) + groc;
                        dinOut = Convert.ToInt32(((decimal)dinOut * 0.2m)) + dinOut;
                        gas = Convert.ToInt32(((decimal)gas * 0.2m)) + gas;
                        healthIns = Convert.ToInt32(((decimal)healthIns * 0.2m)) + healthIns;
                        elec = Convert.ToInt32(((decimal)elec * 0.2m)) + elec;
                        phone = Convert.ToInt32(((decimal)phone * 0.2m)) + phone;
                        natGas = Convert.ToInt32(((decimal)natGas * 0.2m)) + natGas;
                        water = Convert.ToInt32(((decimal)water * 0.2m)) + water;
                        carIns = Convert.ToInt32(((decimal)carIns * 0.2m)) + carIns;
                        incomeAmount = incomeAmount + 350;
                        break;
                    case 6:
                        year = CurrentYear;
                        rent = Convert.ToInt32(((decimal)rent * 0.3M)) + rent;
                        groc = Convert.ToInt32(((decimal)groc * 0.3m)) + groc;
                        dinOut = Convert.ToInt32(((decimal)dinOut * 0.3m)) + dinOut;
                        gas = Convert.ToInt32(((decimal)gas * 0.3m)) + gas;
                        healthIns = Convert.ToInt32(((decimal)healthIns * 0.3m)) + healthIns;
                        elec = Convert.ToInt32(((decimal)elec * 0.3m)) + elec;
                        phone = Convert.ToInt32(((decimal)phone * 0.3m)) + phone;
                        natGas = Convert.ToInt32(((decimal)natGas * 0.3m)) + natGas;
                        water = Convert.ToInt32(((decimal)water * 0.3m)) + water;
                        carIns = Convert.ToInt32(((decimal)carIns * 0.3m)) + carIns;
                        incomeAmount = incomeAmount + 400;
                        break;

                }
                for (int j = 1; j < 13; j++)
                {
                    incomeBudgets.Add(GetBudget(j, year, (incomeAmount * 2), null, BudgetType.Income));
                    incomeBudgets.Add(GetBudget(j, year, Convert.ToInt32((incomeAmount * 2) * .1M), 3, BudgetType.Savings));
                    incomeBudgets.Add(GetBudget(j, year, rent, 4, BudgetType.Need));
                    incomeBudgets.Add(GetBudget(j, year, groc, 2, BudgetType.Need));
                    incomeBudgets.Add(GetBudget(j, year, dinOut, 3, BudgetType.Want));
                    incomeBudgets.Add(GetBudget(j, year, gas, 5, BudgetType.Need));
                    incomeBudgets.Add(GetBudget(j, year, healthIns, 12, BudgetType.Need));
                    incomeBudgets.Add(GetBudget(j, year, elec, 18, BudgetType.Need));
                    incomeBudgets.Add(GetBudget(j, year, internet, 19, BudgetType.Need));
                    incomeBudgets.Add(GetBudget(j, year, phone, 20, BudgetType.Need));
                    incomeBudgets.Add(GetBudget(j, year, natGas, 21, BudgetType.Need));
                    incomeBudgets.Add(GetBudget(j, year, water, 22, BudgetType.Need));
                    incomeBudgets.Add(GetBudget(j, year, 11, 8, BudgetType.Want));
                    incomeBudgets.Add(GetBudget(j, year, 5, 9, BudgetType.Want));
                    incomeBudgets.Add(GetBudget(j, year, 5, 7, BudgetType.Want));
                    incomeBudgets.Add(GetBudget(j, year, 80, 15, BudgetType.Want));
                    incomeBudgets.Add(GetBudget(j, year, 20, 16, BudgetType.Want));
                    incomeBudgets.Add(GetBudget(j, year, 10, 25, BudgetType.Want));
                    if (i == 3 || i == 9)
                    {
                        incomeBudgets.Add(GetBudget(j, year, carIns, 11, BudgetType.Need));
                    }
                }
            }

            return incomeBudgets;
        }
        private static BudgetEntity GetBudget(int month, int year, int Amount, int? categoryId, BudgetType type)
        {
            CurrentBudgetId++;
            return new BudgetEntity()
            {
                Id = CurrentBudgetId,
                Month = month,
                Year = year,
                Amount = Amount,
                SubCategoryId = categoryId,
                BudgetType = type
            };
        }
        public static List<IncomeEntity> Income(List<CsvModels.CsvIncomeCategory> incomeCategories)
        {
            var incomes = new List<IncomeEntity>();
            foreach (var category in incomeCategories)
            {
                switch (category.Name)
                {
                    case "Paycheck":
                        {
                            for (int i = 1; i < 13; i++)
                            {
                                incomes.Add(GetIncome(BasePay, new DateTime(FiveYearsAgo, i, 1), 3, 1, string.Empty));
                                incomes.Add(GetIncome(BasePay, new DateTime(FiveYearsAgo, i, 15), 3, 1, string.Empty));
                                incomes.Add(GetIncome(BasePay + 100, new DateTime(FourYearsAgo, i, 1), 3, 1, string.Empty));
                                incomes.Add(GetIncome(BasePay + 100, new DateTime(FourYearsAgo, i, 15), 3, 1, string.Empty));
                                incomes.Add(GetIncome(BasePay + 150, new DateTime(ThreeYearsAgo, i, 1), 3, 1, string.Empty));
                                incomes.Add(GetIncome(BasePay + 150, new DateTime(ThreeYearsAgo, i, 15), 3, 1, string.Empty));
                                incomes.Add(GetIncome(BasePay + 250, new DateTime(TwoYearsAgo, i, 1), 3, 1, string.Empty));
                                incomes.Add(GetIncome(BasePay + 250, new DateTime(TwoYearsAgo, i, 15), 3, 1, string.Empty));
                                incomes.Add(GetIncome(BasePay + 350, new DateTime(LastYear, i, 1), 3, 1, string.Empty));
                                incomes.Add(GetIncome(BasePay + 350, new DateTime(LastYear, i, 15), 3, 1, string.Empty));

                            }
                            for (int i = 1; i < CurrentMonth; i++)
                            {
                                incomes.Add(GetIncome(BasePay + 400, new DateTime(CurrentYear, i, 1), 3, 1, string.Empty));
                                incomes.Add(GetIncome(BasePay + 400, new DateTime(CurrentYear, i, 15), 3, 1, string.Empty));
                            }
                            if (CurrentDay >= 1)
                            {
                                incomes.Add(GetIncome(BasePay + 400, new DateTime(CurrentYear, CurrentMonth, 1), 3, 1, string.Empty));
                            }
                            if (CurrentDay >= 15)
                            {
                                incomes.Add(GetIncome(BasePay + 400, new DateTime(CurrentYear, CurrentMonth, 15), 3, 1, string.Empty));
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

                            if (CurrentMonth > 1)
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
                            var max = 300;
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
                            expenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 7, new int[] { 62, 63, 64, 65 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 7, new int[] { 62, 63, 64, 65 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 7, new int[] { 62, 63, 64, 65 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(FourYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 7, new int[] { 62, 63, 64, 65 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(FourYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 7, new int[] { 62, 63, 64, 65 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 7, new int[] { 62, 63, 64, 65 }, " to read", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 7, new int[] { 62, 63, 64, 65 }, " to read", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 7, new int[] { 62, 63, 64, 65 }, " to read", 0, rando));
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
                                min, max, 11, timesAMonth, false, false));
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
                    case "Health Insurance":
                        {
                            var min = 280;
                            var max = 280;
                            var timesAMonth = 1;
                            expenses.AddRange(GenerateExpensesAcrossYears(rando, category.Id,
                                new[] { 70 },
                                min, max, 3, timesAMonth, true, false));
                        }
                        break;
                    case "Doctor":
                        {
                            var min = 25;
                            var max = 75;
                            expenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 13, new int[] { 71 }, "Dr Visit for headaches", 0, rando, false));
                            expenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 13, new int[] { 71 }, "exam", 0, rando, false));
                            expenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 13, new int[] { 71 }, "Visit for sinus infection", 0, rando, false));
                            expenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 13, new int[] { 71 }, "X ray scan", 0, rando, false));
                            expenses.Add(GetExpense(min, max, new DateTime(LastYear, rando.Next(1, 12), rando.Next(1, 28)), 13, new int[] { 71 }, "dr Visit", 0, rando, false));
                        }
                        break;
                    case "Drugs":
                        {
                            var min = 5;
                            var max = 15;
                            expenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 14, new int[] { 1, 2, 3 }, "", 0, rando, false));
                            expenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 14, new int[] { 1, 2, 3 }, "", 0, rando, false));
                            expenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 14, new int[] { 1, 2, 3 }, "", 0, rando, false));
                            expenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 14, new int[] { 1, 2, 3 }, "", 0, rando, false));
                            expenses.Add(GetExpense(min, max, new DateTime(LastYear, rando.Next(1, 12), rando.Next(1, 28)), 14, new int[] { 1, 2, 3 }, "", 0, rando, false));
                            expenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 14, new int[] { 1, 2, 3 }, "", 0, rando, false));
                            expenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 14, new int[] { 1, 2, 3 }, "", 0, rando, false));
                            expenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 14, new int[] { 1, 2, 3 }, "", 0, rando, false));
                            expenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 14, new int[] { 1, 2, 3 }, "", 0, rando, false));
                            expenses.Add(GetExpense(min, max, new DateTime(LastYear, rando.Next(1, 12), rando.Next(1, 28)), 14, new int[] { 1, 2, 3 }, "", 0, rando, false));
                        }
                        break;
                    case "Church":
                        {
                            var min = 20;
                            var max = 20;
                            var timesAMonth = 4;
                            expenses.AddRange(GenerateExpensesAcrossYears(rando, category.Id,
                                merchantIds: new[] { 72 },
                                min, max, day: 0, timesAMonth, incrementPrice: false, randomizeCents: false));
                        }
                        break;
                    case "Personal Clothing":
                        {
                            var min = 25;
                            var max = 150;
                            expenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 16, new int[] { 1, 2, 73, 74 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 16, new int[] { 1, 2, 73, 74 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 16, new int[] { 1, 2, 73, 74 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(FourYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 16, new int[] { 1, 2, 73, 74 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(FourYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 16, new int[] { 1, 2, 73, 74 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 16, new int[] { 1, 2, 73, 74 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 16, new int[] { 1, 2, 73, 74 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 16, new int[] { 1, 2, 73, 74 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 16, new int[] { 1, 2, 73, 74 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 16, new int[] { 1, 2, 73, 74 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(LastYear, rando.Next(1, 12), rando.Next(1, 28)), 16, new int[] { 1, 2, 73, 74 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(LastYear, rando.Next(1, 12), rando.Next(1, 28)), 16, new int[] { 1, 2, 73, 74 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(CurrentYear, rando.Next(1, CurrentMonth), rando.Next(1, CurrentDay)), 16, new int[] { 1, 2, 73, 74 }, "", 0, rando));
                        }
                        break;
                    case "Work Clothes":
                        {
                            var min = 85;
                            var max = 159;
                            expenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 17, new int[] { 74 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 17, new int[] { 74 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 17, new int[] { 74 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), 17, new int[] { 74 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(LastYear, rando.Next(1, 12), rando.Next(1, 28)), 17, new int[] { 74 }, "", 0, rando));
                            expenses.Add(GetExpense(min, max, new DateTime(CurrentYear, rando.Next(1, CurrentMonth), rando.Next(1, CurrentDay)), category: 17, merchantIds: new int[] { 74 }, "", 0, rando));
                        }
                        break;
                    case "Electricity":
                        {
                            var min = 95;
                            var max = 140;
                            var timesAMonth = 1;
                            expenses.AddRange(GenerateExpensesAcrossYears(rando, category.Id,
                                merchantIds: new[] { 75 },
                                min, max, day: 7, timesAMonth, incrementPrice: true, randomizeCents: true));
                        }
                        break;
                    case "Internet":
                        {
                            var min = 25;
                            var max = 25;
                            var timesAMonth = 1;
                            expenses.AddRange(GenerateExpensesAcrossYears(rando, category.Id,
                                merchantIds: new[] { 76 },
                                min, max, day: 26, timesAMonth, incrementPrice: false, randomizeCents: false));
                        }
                        break;
                    case "Phone":
                        {
                            var min = 35;
                            var max = 35;
                            var timesAMonth = 1;
                            expenses.AddRange(GenerateExpensesAcrossYears(rando, category.Id,
                                merchantIds: new[] { 77 },
                                min, max, day: 21, timesAMonth, incrementPrice: true, randomizeCents: false));
                        }
                        break;
                    case "Natural Gas":
                        {
                            var min = 48;
                            var max = 48;
                            var timesAMonth = 1;
                            expenses.AddRange(GenerateExpensesAcrossYears(rando, category.Id,
                                merchantIds: new[] { 78 },
                                min, max, day: 9, timesAMonth, incrementPrice: true, randomizeCents: false));
                        }
                        break;
                    case "Water":
                        {
                            var min = 50;
                            var max = 60;
                            var timesAMonth = 1;
                            expenses.AddRange(GenerateExpensesAcrossYears(rando, category.Id,
                                merchantIds: new[] { 79 },
                                min, max, day: 13, timesAMonth, incrementPrice: true, randomizeCents: false));
                        }
                        break;
                    case "Fees":
                        {
                            var min = 2;
                            var max = 25;
                            var categoryId = 23;
                            var merchantIds = new[] { 80, 81, 82 };
                            expenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(LastYear, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(LastYear, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(CurrentYear, rando.Next(1, CurrentMonth), rando.Next(1, CurrentDay)), category: categoryId, merchantIds: merchantIds, "random fee", 0, rando));
                        }
                        break;
                    case "Furniture":
                        {
                            var min = 25;
                            var max = 800;
                            var categoryId = 24;
                            var merchantIds = new[] { 1, 2, 74 };
                            expenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(LastYear, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(LastYear, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(CurrentYear, rando.Next(1, CurrentMonth), rando.Next(1, CurrentDay)), category: categoryId, merchantIds: merchantIds, "a chair", 0, rando));
                        }
                        break;
                    case "Games":
                        {
                            var min = 20;
                            var max = 60;
                            var categoryId = 25;
                            var merchantIds = new[] { 83, 84 };
                            expenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(LastYear, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(LastYear, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(CurrentYear, rando.Next(1, CurrentMonth), rando.Next(1, CurrentDay)), category: categoryId, merchantIds: merchantIds, "board game", 0, rando));
                        }
                        break;
                    case "Gifts":
                        {
                            var min = 25;
                            var max = 50;
                            var categoryId = 26;
                            var merchantIds = new[] { 1, 2 };
                            expenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(LastYear, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(LastYear, rando.Next(1, 12), rando.Next(1, 28)), categoryId, merchantIds, "", 0, rando, true));
                            expenses.Add(GetExpense(min, max, new DateTime(CurrentYear, rando.Next(1, CurrentMonth), rando.Next(1, CurrentDay)), category: categoryId, merchantIds: merchantIds, "flowers for my mom", 0, rando));
                        }
                        break;
                    case "Other":
                        {
                            var min = 5;
                            var max = 10;
                            var categoryId = 27;
                            var merchantIds = new[] { 1 };
                            expenses.Add(GetExpense(min, max, new DateTime(CurrentYear, rando.Next(1, CurrentMonth), rando.Next(1, CurrentDay)), categoryId, merchantIds, "I couldn't figure out what category to put this under.", 0, rando, true));
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
                bool randomizeCents = true,
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
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, i, day), category, merchantIds, notes, 0, rando, randomizeCents));

                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FourYearsAgo, i, day), category, merchantIds, notes, incrementPrice ? incrementPrice ? incrementPrice ? 0.1M : 0 : 0 : 0, rando, randomizeCents));

                                listOfExpenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, i, day), category, merchantIds, notes, incrementPrice ? 0.1M : 0, rando, randomizeCents));

                                listOfExpenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, i, day), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, randomizeCents));

                                listOfExpenses.Add(GetExpense(min, max, new DateTime(LastYear, i, day), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, randomizeCents));
                            }
                            for (int i = 1; i <= CurrentMonth; i++)
                            {
                                if (i < CurrentMonth)
                                {
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, day), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, randomizeCents));
                                }
                                if (i == CurrentMonth && CurrentDay > day)
                                {
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, day), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, randomizeCents));
                                }
                            }
                        }
                        break;
                    case 2:
                        {
                            for (int i = 1; i < 13; i++)
                            {
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, i, rando.Next(1, 14)), category, merchantIds, notes, 0, rando, randomizeCents));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, i, rando.Next(15, 28)), category, merchantIds, notes, 0, rando, randomizeCents));

                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FourYearsAgo, i, rando.Next(1, 14)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, randomizeCents));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FourYearsAgo, i, rando.Next(15, 28)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, randomizeCents));

                                listOfExpenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, i, rando.Next(1, 14)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, randomizeCents));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, i, rando.Next(15, 28)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, randomizeCents));

                                listOfExpenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, i, rando.Next(1, 14)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, randomizeCents));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, i, rando.Next(15, 28)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, randomizeCents));

                                listOfExpenses.Add(GetExpense(min, max, new DateTime(LastYear, i, rando.Next(1, 14)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, randomizeCents));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(LastYear, i, rando.Next(15, 28)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, randomizeCents));

                            }
                            for (int i = 1; i <= CurrentMonth; i++)
                            {
                                if (i < CurrentMonth)
                                {
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(1, 14)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, randomizeCents));
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(15, 28)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, randomizeCents));

                                }
                                if (i == CurrentMonth && CurrentDay > 15)
                                {
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(1, 14)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, randomizeCents));
                                }
                                if (i == CurrentMonth && CurrentDay > 28)
                                {
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(15, 28)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, randomizeCents));
                                }
                            }
                        }
                        break;
                    case 3:
                        {
                            for (int i = 1; i < 13; i++)
                            {
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, i, rando.Next(1, 9)), category, merchantIds, notes, 0, rando, randomizeCents));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, i, rando.Next(10, 19)), category, merchantIds, notes, 0, rando, randomizeCents));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, i, rando.Next(20, 28)), category, merchantIds, notes, 0, rando, randomizeCents));

                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FourYearsAgo, i, rando.Next(1, 9)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, randomizeCents));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FourYearsAgo, i, rando.Next(10, 19)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, randomizeCents));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FourYearsAgo, i, rando.Next(20, 28)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, randomizeCents));

                                listOfExpenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, i, rando.Next(1, 9)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, randomizeCents));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, i, rando.Next(10, 19)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, randomizeCents));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, i, rando.Next(20, 28)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, randomizeCents));

                                listOfExpenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, i, rando.Next(1, 9)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, randomizeCents));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, i, rando.Next(10, 19)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, randomizeCents));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, i, rando.Next(20, 28)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, randomizeCents));

                                listOfExpenses.Add(GetExpense(min, max, new DateTime(LastYear, i, rando.Next(1, 9)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, randomizeCents));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(LastYear, i, rando.Next(10, 19)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, randomizeCents));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(LastYear, i, rando.Next(20, 28)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, randomizeCents));

                            }
                            for (int i = 1; i <= CurrentMonth; i++)
                            {
                                if (i < CurrentMonth)
                                {
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(1, 9)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, randomizeCents));
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(10, 19)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, randomizeCents));
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(20, 28)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, randomizeCents));
                                }
                                if (i == CurrentMonth && CurrentDay > 10)
                                {
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(1, 9)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, randomizeCents));
                                }
                                if (i == CurrentMonth && CurrentDay > 20)
                                {
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(10, 19)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, randomizeCents));
                                }
                                if (i == CurrentMonth && CurrentDay > 28)
                                {
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(20, 28)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, randomizeCents));
                                }

                            }
                        }
                        break;
                    case 4:
                        {
                            for (int i = 1; i < 13; i++)
                            {
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, i, rando.Next(1, 7)), category, merchantIds, notes, 0, rando, randomizeCents));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, i, rando.Next(8, 14)), category, merchantIds, notes, 0, rando, randomizeCents));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, i, rando.Next(15, 21)), category, merchantIds, notes, 0, rando, randomizeCents));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FiveYearsAgo, i, rando.Next(22, 28)), category, merchantIds, notes, 0, rando, randomizeCents));

                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FourYearsAgo, i, rando.Next(1, 7)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, randomizeCents));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FourYearsAgo, i, rando.Next(8, 14)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, randomizeCents));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FourYearsAgo, i, rando.Next(15, 21)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, randomizeCents));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(FourYearsAgo, i, rando.Next(22, 28)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, randomizeCents));

                                listOfExpenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, i, rando.Next(1, 7)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, randomizeCents));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, i, rando.Next(8, 14)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, randomizeCents));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, i, rando.Next(15, 21)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, randomizeCents));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(ThreeYearsAgo, i, rando.Next(22, 28)), category, merchantIds, notes, incrementPrice ? incrementPrice ? 0.1M : 0 : 0, rando, randomizeCents));

                                listOfExpenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, i, rando.Next(1, 7)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, randomizeCents));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, i, rando.Next(8, 14)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, randomizeCents));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, i, rando.Next(15, 21)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, randomizeCents));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(TwoYearsAgo, i, rando.Next(22, 28)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, randomizeCents));

                                listOfExpenses.Add(GetExpense(min, max, new DateTime(LastYear, i, rando.Next(1, 7)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, randomizeCents));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(LastYear, i, rando.Next(8, 14)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, randomizeCents));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(LastYear, i, rando.Next(15, 21)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, randomizeCents));
                                listOfExpenses.Add(GetExpense(min, max, new DateTime(LastYear, i, rando.Next(22, 28)), category, merchantIds, notes, incrementPrice ? 0.2M : 0, rando, randomizeCents));
                            }
                            for (int i = 1; i <= CurrentMonth; i++)
                            {
                                if (i < CurrentMonth)
                                {
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(1, 7)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, randomizeCents));
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(8, 14)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, randomizeCents));
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(15, 21)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, randomizeCents));
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(22, 28)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, randomizeCents));
                                }
                                if (i == CurrentMonth && CurrentDay > 7)
                                {
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(1, 7)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, randomizeCents));
                                }
                                if (i == CurrentMonth && CurrentDay > 15)
                                {
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(8, 14)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, randomizeCents));
                                }
                                if (i == CurrentMonth && CurrentDay > 23)
                                {
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(15, 21)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, randomizeCents));
                                }
                                if (i == CurrentMonth && CurrentDay > 28)
                                {
                                    listOfExpenses.Add(GetExpense(min, max, new DateTime(CurrentYear, i, rando.Next(22, 28)), category, merchantIds, notes, incrementPrice ? 0.3M : 0, rando, randomizeCents));
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