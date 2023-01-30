using CashTrack.Data.CsvFiles;
using CashTrack.Data.Entities;
using System.Collections.Generic;
using System;

namespace CashTrack.Data
{
    public static class GenerateData
    {
        private static int StartYear => DateTime.Now.AddYears(-2).Year;
        private static int LastYear => DateTime.Now.AddYears(-1).Year;
        private static int CurrentYear => DateTime.Now.Year;
        private static int CurrentMonth => DateTime.Now.Month;
        private static int CurrentDay => DateTime.Now.Day;
        public static List<IncomeEntity> Income(List<CsvModels.CsvIncomeCategory> incomeCategories)
        {
            var incomes = new List<IncomeEntity>();
            int currentIncomeId = 0;
            foreach (var category in incomeCategories)
            {
                switch (category.Name)
                {
                    case "Paycheck":
                        {
                            var basePay = 1529M;
                            for (int i = 1; i < 13; i++)
                            {
                                currentIncomeId++;
                                var payment = new IncomeEntity()
                                {
                                    Id = currentIncomeId,
                                    Amount = basePay,
                                    Date = new DateTime(StartYear, i, 1),
                                    CategoryId = 3,
                                    SourceId = 1,
                                    IsRefund = false
                                };
                                incomes.Add(payment);
                                currentIncomeId++;
                                var secondPayment = new IncomeEntity()
                                {
                                    Id = currentIncomeId,
                                    Amount = basePay,
                                    Date = new DateTime(StartYear, i, 15),
                                    CategoryId = 3,
                                    SourceId = 1,
                                    IsRefund = false
                                };
                                incomes.Add(secondPayment);
                            }

                            for (int i = 1; i < 13; i++)
                            {
                                currentIncomeId++;
                                var payment = new IncomeEntity()
                                {
                                    Id = currentIncomeId,
                                    Amount = basePay + 100m,
                                    Date = new DateTime(LastYear, i, 1),
                                    CategoryId = 3,
                                    SourceId = 1,
                                    IsRefund = false
                                };
                                incomes.Add(payment);
                                currentIncomeId++;
                                var secondPayment = new IncomeEntity()
                                {
                                    Id = currentIncomeId,
                                    Amount = basePay + 100m,
                                    Date = new DateTime(LastYear, i, 15),
                                    CategoryId = 3,
                                    SourceId = 1,
                                    IsRefund = false
                                };
                                incomes.Add(secondPayment);
                            }

                            for (int i = 1; i <= CurrentMonth; i++)
                            {
                                currentIncomeId++;
                                var payment = new IncomeEntity()
                                {
                                    Id = currentIncomeId,
                                    Amount = basePay + 150m,
                                    Date = new DateTime(CurrentYear, i, 1),
                                    CategoryId = 3,
                                    SourceId = 1,
                                    IsRefund = false
                                };
                                incomes.Add(payment);
                                currentIncomeId++;
                                if (CurrentDay > 15)
                                {
                                    var secondPayment = new IncomeEntity()
                                    {
                                        Id = currentIncomeId,
                                        Amount = basePay + 150m,
                                        Date = new DateTime(CurrentYear, i, 15),
                                        CategoryId = 3,
                                        SourceId = 1,
                                        IsRefund = false
                                    };
                                    incomes.Add(secondPayment);
                                }
                            }
                        }
                        break;
                    case "Gift":
                        {
                            currentIncomeId++;
                            var firstpayment = new IncomeEntity()
                            {
                                Id = currentIncomeId,
                                Amount = 100M,
                                Date = new DateTime(StartYear, 1, 11),
                                CategoryId = 4,
                                SourceId = 3,
                                IsRefund = false,
                                Notes = "Birthday Money from Parents"
                            };
                            incomes.Add(firstpayment);
                            currentIncomeId++;
                            var secondPayment = new IncomeEntity()
                            {
                                Id = currentIncomeId,
                                Amount = 200M,
                                Date = new DateTime(StartYear, 12, 25),
                                CategoryId = 4,
                                SourceId = 3,
                                IsRefund = false,
                                Notes = "Christmas money from Parents"
                            };
                            incomes.Add(secondPayment);
                            currentIncomeId++;
                            var thirdPayment = new IncomeEntity()
                            {
                                Id = currentIncomeId,
                                Amount = 100M,
                                Date = new DateTime(LastYear, 1, 11),
                                CategoryId = 4,
                                SourceId = 3,
                                IsRefund = false,
                                Notes = "Birthday Money from Parents"
                            };
                            incomes.Add(thirdPayment);
                            currentIncomeId++;
                            var fourthPayment = new IncomeEntity()
                            {
                                Id = currentIncomeId,
                                Amount = 200M,
                                Date = new DateTime(LastYear, 12, 25),
                                CategoryId = 4,
                                SourceId = 3,
                                IsRefund = false,
                                Notes = "Christmas money from Parents"
                            };
                            incomes.Add(fourthPayment);
                            if (CurrentDay > 11)
                            {
                                currentIncomeId++;
                                var fifthpayment = new IncomeEntity()
                                {
                                    Id = currentIncomeId,
                                    Amount = 100M,
                                    Date = new DateTime(CurrentYear, 1, 11),
                                    CategoryId = 4,
                                    SourceId = 3,
                                    IsRefund = false,
                                    Notes = "Birthday Money from Parents, again"
                                };
                                incomes.Add(fifthpayment);
                            }
                        }
                        break;
                    case "Tax Refund":
                        {
                            currentIncomeId++;
                            var payment = new IncomeEntity()
                            {
                                Id = currentIncomeId,
                                Amount = 2123M,
                                Date = new DateTime(StartYear, 4, 14),
                                CategoryId = 5,
                                SourceId = 2,
                                IsRefund = false,
                                Notes = "Federal Tax Refund"
                            };
                            incomes.Add(payment);

                            currentIncomeId++;
                            var secondpayment = new IncomeEntity()
                            {
                                Id = currentIncomeId,
                                Amount = 2243M,
                                Date = new DateTime(LastYear, 4, 9),
                                CategoryId = 5,
                                SourceId = 2,
                                IsRefund = false,
                                Notes = "Federal Tax Refund"
                            };
                            incomes.Add(secondpayment);

                            if (CurrentMonth >= 4)
                            {
                                currentIncomeId++;
                                var thirdpayment = new IncomeEntity()
                                {
                                    Id = currentIncomeId,
                                    Amount = 2156M,
                                    Date = new DateTime(CurrentYear, 4, 1),
                                    CategoryId = 5,
                                    SourceId = 2,
                                    IsRefund = false,
                                    Notes = "Federal Tax Refund"
                                };
                                incomes.Add(thirdpayment);
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
            int currentId = 1;
            var rando = new Random();
            foreach (var category in categories)
            {
                switch (category.Name)
                {
                    case "Groceries":
                        {
                            var categoryId = category.Id;
                            //3 times a month, 90-200
                            var min = 90;
                            var max = 210;
                            expenses.AddRange(GenerateExpenses(rando, categoryId, 1, currentId, min, max));
                        }
                    break;
                }
            }
            return expenses;

            static List<ExpenseEntity> GenerateExpenses(
                Random rando, 
                int category, 
                int merchant, 
                int id, 
                int min, 
                int max, 
                int day = 0, 
                int numberToGenerateInAMonth = 1, 
                string notes = "")
            {
                var listOfExpenses = new List<ExpenseEntity>();
                
                switch (numberToGenerateInAMonth)
                {
                    case 1:
                        {
                            for (int i = 1; i < 13; i++)
                            {
                                // start Year
                                id++;
                                var ex1 = new ExpenseEntity()
                                {
                                    Id = id,
                                    Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                    Date = new DateTime(StartYear, i, rando.Next(1, 28)),
                                    CategoryId = category,
                                    MerchantId = merchant,
                                    Notes = notes
                                };
                                listOfExpenses.Add(ex1);
                                //last year
                                id++;
                                var ex2 = new ExpenseEntity()
                                {
                                    Id = id,
                                    Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                    Date = new DateTime(LastYear, i, rando.Next(1, 28)),
                                    CategoryId = category,
                                    MerchantId = merchant,
                                    Notes = notes
                                };
                                listOfExpenses.Add(ex2);
                            }
                            for (int i = 1; i <= CurrentMonth; i++)
                            {
                                id++;
                                var ex3 = new ExpenseEntity()
                                {
                                    Id = id,
                                    Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                    Date = new DateTime(CurrentYear, i, rando.Next(1, 28)),
                                    CategoryId = category,
                                    MerchantId = merchant,
                                    Notes = notes
                                };
                                listOfExpenses.Add(ex3);
                            }
                        }
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                }
                return listOfExpenses;
            }
        }
    }
}