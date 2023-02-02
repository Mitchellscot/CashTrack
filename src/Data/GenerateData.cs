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
                            var max = 200;
                            expenses.AddRange(GenerateExpenses(
                                rando,
                                categoryId,
                                new[] { 1, 2, 3 },
                                currentId,
                                min,
                                max,
                                0,
                                3));
                        }
                        break;
                }
            }
            return expenses;

            static List<ExpenseEntity> GenerateExpenses(
                Random rando,
                int category,
                int[] merchantIds,
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
                                    MerchantId = RandomMerchantId(merchantIds, rando),
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
                                    MerchantId = RandomMerchantId(merchantIds, rando),
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
                                    MerchantId = RandomMerchantId(merchantIds, rando),
                                    Notes = notes
                                };
                                listOfExpenses.Add(ex3);
                            }
                        }
                        break;
                    case 2:
                        {
                            for (int i = 1; i < 13; i++)
                            {
                                // start Year
                                id++;
                                var ex1 = new ExpenseEntity()
                                {
                                    Id = id,
                                    Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                    Date = new DateTime(StartYear, i, rando.Next(1, 14)),
                                    CategoryId = category,
                                    MerchantId = RandomMerchantId(merchantIds, rando),
                                    Notes = notes
                                };
                                listOfExpenses.Add(ex1);
                                id++;
                                var ex2 = new ExpenseEntity()
                                {
                                    Id = id,
                                    Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                    Date = new DateTime(StartYear, i, rando.Next(15, 28)),
                                    CategoryId = category,
                                    MerchantId = RandomMerchantId(merchantIds, rando),
                                    Notes = notes
                                };
                                listOfExpenses.Add(ex2);
                                //last year
                                id++;
                                var ex3 = new ExpenseEntity()
                                {
                                    Id = id,
                                    Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                    Date = new DateTime(LastYear, i, rando.Next(1, 14)),
                                    CategoryId = category,
                                    MerchantId = RandomMerchantId(merchantIds, rando),
                                    Notes = notes
                                };
                                listOfExpenses.Add(ex3);
                                id++;
                                var ex4 = new ExpenseEntity()
                                {
                                    Id = id,
                                    Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                    Date = new DateTime(LastYear, i, rando.Next(15, 28)),
                                    CategoryId = category,
                                    MerchantId = RandomMerchantId(merchantIds, rando),
                                    Notes = notes
                                };
                                listOfExpenses.Add(ex4);
                            }
                            for (int i = 1; i <= CurrentMonth; i++)
                            {
                                if (i < CurrentMonth)
                                {
                                    id++;
                                    var ex5 = new ExpenseEntity()
                                    {
                                        Id = id,
                                        Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                        Date = new DateTime(CurrentYear, i, rando.Next(1, 14)),
                                        CategoryId = category,
                                        MerchantId = RandomMerchantId(merchantIds, rando),
                                        Notes = notes
                                    };
                                    listOfExpenses.Add(ex5);
                                    id++;
                                    var ex6 = new ExpenseEntity()
                                    {
                                        Id = id,
                                        Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                        Date = new DateTime(CurrentYear, i, rando.Next(15, 28)),
                                        CategoryId = category,
                                        MerchantId = RandomMerchantId(merchantIds, rando),
                                        Notes = notes
                                    };
                                    listOfExpenses.Add(ex6);
                                }
                                if (i == CurrentMonth && CurrentDay > 15)
                                {
                                    id++;
                                    var ex7 = new ExpenseEntity()
                                    {
                                        Id = id,
                                        Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                        Date = new DateTime(CurrentYear, i, rando.Next(1, 14)),
                                        CategoryId = category,
                                        MerchantId = RandomMerchantId(merchantIds, rando),
                                        Notes = notes
                                    };
                                    listOfExpenses.Add(ex7);
                                }
                                if (i == CurrentMonth && CurrentDay > 28)
                                {
                                    id++;
                                    var ex8 = new ExpenseEntity()
                                    {
                                        Id = id,
                                        Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                        Date = new DateTime(CurrentYear, i, rando.Next(16, 28)),
                                        CategoryId = category,
                                        MerchantId = RandomMerchantId(merchantIds, rando),
                                        Notes = notes
                                    };
                                    listOfExpenses.Add(ex8);
                                }
                            }
                        }
                        break;
                    case 3:
                        {
                            for (int i = 1; i < 13; i++)
                            {
                                // start Year
                                id++;
                                var ex1 = new ExpenseEntity()
                                {
                                    Id = id,
                                    Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                    Date = new DateTime(StartYear, i, rando.Next(1, 9)),
                                    CategoryId = category,
                                    MerchantId = RandomMerchantId(merchantIds, rando),
                                    Notes = notes
                                };
                                listOfExpenses.Add(ex1);
                                id++;
                                var ex2 = new ExpenseEntity()
                                {
                                    Id = id,
                                    Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                    Date = new DateTime(StartYear, i, rando.Next(10, 19)),
                                    CategoryId = category,
                                    MerchantId = RandomMerchantId(merchantIds, rando),
                                    Notes = notes
                                };
                                listOfExpenses.Add(ex2);
                                id++;
                                var ex3 = new ExpenseEntity()
                                {
                                    Id = id,
                                    Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                    Date = new DateTime(StartYear, i, rando.Next(20, 28)),
                                    CategoryId = category,
                                    MerchantId = RandomMerchantId(merchantIds, rando),
                                    Notes = notes
                                };
                                listOfExpenses.Add(ex3);
                                //last year
                                id++;
                                var ex4 = new ExpenseEntity()
                                {
                                    Id = id,
                                    Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                    Date = new DateTime(LastYear, i, rando.Next(1, 9)),
                                    CategoryId = category,
                                    MerchantId = RandomMerchantId(merchantIds, rando),
                                    Notes = notes
                                };
                                listOfExpenses.Add(ex4);
                                id++;
                                var ex5 = new ExpenseEntity()
                                {
                                    Id = id,
                                    Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                    Date = new DateTime(LastYear, i, rando.Next(10, 19)),
                                    CategoryId = category,
                                    MerchantId = RandomMerchantId(merchantIds, rando),
                                    Notes = notes
                                };
                                listOfExpenses.Add(ex5);
                                id++;
                                var ex6 = new ExpenseEntity()
                                {
                                    Id = id,
                                    Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                    Date = new DateTime(LastYear, i, rando.Next(20, 28)),
                                    CategoryId = category,
                                    MerchantId = RandomMerchantId(merchantIds, rando),
                                    Notes = notes
                                };
                                listOfExpenses.Add(ex6);
                            }
                            for (int i = 1; i <= CurrentMonth; i++)
                            {
                                if (i < CurrentMonth)
                                {
                                    id++;
                                    var ex7 = new ExpenseEntity()
                                    {
                                        Id = id,
                                        Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                        Date = new DateTime(CurrentYear, i, rando.Next(1, 9)),
                                        CategoryId = category,
                                        MerchantId = RandomMerchantId(merchantIds, rando),
                                        Notes = notes
                                    };
                                    listOfExpenses.Add(ex7);
                                    id++;
                                    var ex8 = new ExpenseEntity()
                                    {
                                        Id = id,
                                        Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                        Date = new DateTime(CurrentYear, i, rando.Next(10, 19)),
                                        CategoryId = category,
                                        MerchantId = RandomMerchantId(merchantIds, rando),
                                        Notes = notes
                                    };
                                    listOfExpenses.Add(ex8);
                                    id++;
                                    var ex9 = new ExpenseEntity()
                                    {
                                        Id = id,
                                        Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                        Date = new DateTime(CurrentYear, i, rando.Next(20, 28)),
                                        CategoryId = category,
                                        MerchantId = RandomMerchantId(merchantIds, rando),
                                        Notes = notes
                                    };
                                    listOfExpenses.Add(ex9);
                                }
                                if (i == CurrentMonth && CurrentDay > 10)
                                {
                                    id++;
                                    var ex10 = new ExpenseEntity()
                                    {
                                        Id = id,
                                        Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                        Date = new DateTime(CurrentYear, i, rando.Next(1, 9)),
                                        CategoryId = category,
                                        MerchantId = RandomMerchantId(merchantIds, rando),
                                        Notes = notes
                                    };
                                    listOfExpenses.Add(ex10);
                                }
                                if (i == CurrentMonth && CurrentDay > 20)
                                {
                                    id++;
                                    var ex11 = new ExpenseEntity()
                                    {
                                        Id = id,
                                        Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                        Date = new DateTime(CurrentYear, i, rando.Next(10, 19)),
                                        CategoryId = category,
                                        MerchantId = RandomMerchantId(merchantIds, rando),
                                        Notes = notes
                                    };
                                    listOfExpenses.Add(ex11);
                                }
                                if (i == CurrentMonth && CurrentDay > 28)
                                {
                                    id++;
                                    var ex12 = new ExpenseEntity()
                                    {
                                        Id = id,
                                        Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                        Date = new DateTime(CurrentYear, i, rando.Next(20, 28)),
                                        CategoryId = category,
                                        MerchantId = RandomMerchantId(merchantIds, rando),
                                        Notes = notes
                                    };
                                    listOfExpenses.Add(ex12);
                                }

                            }
                        }
                        break;
                    case 4:
                        for (int i = 1; i < 13; i++)
                        {
                            // start Year
                            id++;
                            var ex1 = new ExpenseEntity()
                            {
                                Id = id,
                                Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                Date = new DateTime(StartYear, i, rando.Next(1, 7)),
                                CategoryId = category,
                                MerchantId = RandomMerchantId(merchantIds, rando),
                                Notes = notes
                            };
                            listOfExpenses.Add(ex1);
                            id++;
                            var ex2 = new ExpenseEntity()
                            {
                                Id = id,
                                Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                Date = new DateTime(StartYear, i, rando.Next(8, 15)),
                                CategoryId = category,
                                MerchantId = RandomMerchantId(merchantIds, rando),
                                Notes = notes
                            };
                            listOfExpenses.Add(ex2);
                            id++;
                            var ex3 = new ExpenseEntity()
                            {
                                Id = id,
                                Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                Date = new DateTime(StartYear, i, rando.Next(16, 23)),
                                CategoryId = category,
                                MerchantId = RandomMerchantId(merchantIds, rando),
                                Notes = notes
                            };
                            listOfExpenses.Add(ex3);
                            id++;
                            var ex4 = new ExpenseEntity()
                            {
                                Id = id,
                                Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                Date = new DateTime(StartYear, i, rando.Next(24, 28)),
                                CategoryId = category,
                                MerchantId = RandomMerchantId(merchantIds, rando),
                                Notes = notes
                            };
                            listOfExpenses.Add(ex4);
                            //last year
                            id++;
                            var ex5 = new ExpenseEntity()
                            {
                                Id = id,
                                Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                Date = new DateTime(LastYear, i, rando.Next(1, 7)),
                                CategoryId = category,
                                MerchantId = RandomMerchantId(merchantIds, rando),
                                Notes = notes
                            };
                            listOfExpenses.Add(ex5);
                            id++;
                            var ex6 = new ExpenseEntity()
                            {
                                Id = id,
                                Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                Date = new DateTime(LastYear, i, rando.Next(8, 15)),
                                CategoryId = category,
                                MerchantId = RandomMerchantId(merchantIds, rando),
                                Notes = notes
                            };
                            listOfExpenses.Add(ex6);
                            id++;
                            var ex7 = new ExpenseEntity()
                            {
                                Id = id,
                                Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                Date = new DateTime(LastYear, i, rando.Next(16, 23)),
                                CategoryId = category,
                                MerchantId = RandomMerchantId(merchantIds, rando),
                                Notes = notes
                            };
                            listOfExpenses.Add(ex7);
                            id++;
                            var ex8 = new ExpenseEntity()
                            {
                                Id = id,
                                Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                Date = new DateTime(LastYear, i, rando.Next(17, 28)),
                                CategoryId = category,
                                MerchantId = RandomMerchantId(merchantIds, rando),
                                Notes = notes
                            };
                            listOfExpenses.Add(ex8);
                        }
                        for (int i = 1; i <= CurrentMonth; i++)
                        {
                            if (i < CurrentMonth)
                            {
                                id++;
                                var ex9 = new ExpenseEntity()
                                {
                                    Id = id,
                                    Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                    Date = new DateTime(CurrentYear, i, rando.Next(1, 7)),
                                    CategoryId = category,
                                    MerchantId = RandomMerchantId(merchantIds, rando),
                                    Notes = notes
                                };
                                listOfExpenses.Add(ex9);
                                id++;
                                var ex10 = new ExpenseEntity()
                                {
                                    Id = id,
                                    Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                    Date = new DateTime(CurrentYear, i, rando.Next(8, 15)),
                                    CategoryId = category,
                                    MerchantId = RandomMerchantId(merchantIds, rando),
                                    Notes = notes
                                };
                                listOfExpenses.Add(ex10);
                                id++;
                                var ex11 = new ExpenseEntity()
                                {
                                    Id = id,
                                    Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                    Date = new DateTime(CurrentYear, i, rando.Next(16, 23)),
                                    CategoryId = category,
                                    MerchantId = RandomMerchantId(merchantIds, rando),
                                    Notes = notes
                                };
                                listOfExpenses.Add(ex11);
                                id++;
                                var ex12 = new ExpenseEntity()
                                {
                                    Id = id,
                                    Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                    Date = new DateTime(CurrentYear, i, rando.Next(24, 28)),
                                    CategoryId = category,
                                    MerchantId = RandomMerchantId(merchantIds, rando),
                                    Notes = notes
                                };
                                listOfExpenses.Add(ex12);
                            }
                            if (i == CurrentMonth && CurrentDay > 7)
                            {
                                id++;
                                var ex13 = new ExpenseEntity()
                                {
                                    Id = id,
                                    Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                    Date = new DateTime(CurrentYear, i, rando.Next(1, 7)),
                                    CategoryId = category,
                                    MerchantId = RandomMerchantId(merchantIds, rando),
                                    Notes = notes
                                };
                                listOfExpenses.Add(ex13);
                            }
                            if (i == CurrentMonth && CurrentDay > 15)
                            {
                                id++;
                                var ex14 = new ExpenseEntity()
                                {
                                    Id = id,
                                    Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                    Date = new DateTime(CurrentYear, i, rando.Next(8, 15)),
                                    CategoryId = category,
                                    MerchantId = RandomMerchantId(merchantIds, rando),
                                    Notes = notes
                                };
                                listOfExpenses.Add(ex14);
                            }
                            if (i == CurrentMonth && CurrentDay > 23)
                            {
                                id++;
                                var ex15 = new ExpenseEntity()
                                {
                                    Id = id,
                                    Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                    Date = new DateTime(CurrentYear, i, rando.Next(16, 23)),
                                    CategoryId = category,
                                    MerchantId = RandomMerchantId(merchantIds, rando),
                                    Notes = notes
                                };
                                listOfExpenses.Add(ex15);
                            }
                            if (i == CurrentMonth && CurrentDay > 28)
                            {
                                id++;
                                var ex16 = new ExpenseEntity()
                                {
                                    Id = id,
                                    Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) + rando.NextDouble()), 2),
                                    Date = new DateTime(CurrentYear, i, rando.Next(24, 28)),
                                    CategoryId = category,
                                    MerchantId = RandomMerchantId(merchantIds, rando),
                                    Notes = notes
                                };
                                listOfExpenses.Add(ex16);
                            }
                        }
                        break;
                }
                return listOfExpenses;
            }
        }
        private static int RandomMerchantId(int[] numbers, Random rando)
        {
            var index = rando.Next(0, numbers.Length);
            return numbers[index];
        }
    }

}