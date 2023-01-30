using Microsoft.EntityFrameworkCore;
using CashTrack.Data.Entities;
using CashTrack.Data.CsvFiles;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Claims;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using CashTrack.Common;
using CashTrack.Common.Extensions;
using System.Collections.Generic;
using CashTrack.Models.IncomeModels;
using CashTrack.Models.Common;
using CashTrack.Models.MerchantModels;
using static Azure.Core.HttpHeader;
using System.Drawing;

namespace CashTrack.Data
{
    public class AppDbContext : IdentityDbContext<UserEntity, IdentityRole<int>, int>
    {
        public DbSet<BudgetEntity> Budgets { get; set; }
        public DbSet<ExpenseEntity> Expenses { get; set; }
        public DbSet<IncomeEntity> Incomes { get; set; }
        public DbSet<MainCategoryEntity> MainCategories { get; set; }
        public DbSet<SubCategoryEntity> SubCategories { get; set; }
        public DbSet<MerchantEntity> Merchants { get; set; }
        public DbSet<TagEntity> Tags { get; set; }
        public DbSet<IncomeSourceEntity> IncomeSources { get; set; }
        public DbSet<IncomeCategoryEntity> IncomeCategories { get; set; }
        public DbSet<ExpenseReviewEntity> ExpensesToReview { get; set; }
        public DbSet<IncomeReviewEntity> IncomeToReview { get; set; }
        public DbSet<ImportRuleEntity> ImportRules { get; set; }
        private string _args { get; set; }
        private readonly string _env;
        private const string SQLite = "Microsoft.EntityFrameworkCore.Sqlite";
        public AppDbContext(DbContextOptions options, IWebHostEnvironment env, string args = "") : base(options)
        {
            _args = args;
            _env = env.EnvironmentName;
        }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);

            mb.Initialize(_env, _args);

            if (Database.ProviderName.IsEqualTo(SQLite))
                ConfigureForSqlLite(mb);

        }
        private void ConfigureForSqlLite(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var properties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(decimal));
                foreach (var property in properties)
                {
                    modelBuilder.Entity(entityType.Name).Property(property.Name).HasConversion<double>();
                }
            }
        }
    }
    //model builder extension to seed DB data
    public static class SeedData
    {
        public static void Initialize(this ModelBuilder mb, string env, string args)
        {
            mb.Entity<ExpenseTags>().HasKey(et => new { et.ExpenseId, et.TagId });

            mb.Entity<ExpenseTags>()
                .HasOne(et => et.Expense)
                .WithMany(e => e.ExpenseTags)
                .HasForeignKey(et => et.ExpenseId);
            mb.Entity<ExpenseTags>()
                .HasOne(et => et.Tag)
                .WithMany(e => e.ExpenseTags)
                .HasForeignKey(et => et.TagId);

            mb.Entity<UserEntity>().ToTable("Users");
            mb.Entity<IdentityUserClaim<int>>().ToTable("UsersClaims");
            mb.Entity<IdentityUserToken<int>>().ToTable("UsersTokens");
            mb.Entity<IdentityUserLogin<int>>().ToTable("UsersLogins");
            mb.Ignore<IdentityRole<int>>();
            mb.Ignore<IdentityRoleClaim<int>>();
            mb.Ignore<IdentityUserRole<int>>();

            if (args.IsEqualTo("new"))
            {
                //seed basic data
                var rando = new Random();
                var userId = rando.Next(1, int.MaxValue);
                var passwordHasher = new PasswordHasher<UserEntity>();
                var seededUser = new UserEntity()
                {
                    Id = userId,
                    UserName = "cash",
                    FirstName = "New User",
                    LastName = "",
                    Email = "",
                    NormalizedEmail = "",
                    NormalizedUserName = "CASH",
                    SecurityStamp = Guid.NewGuid().ToString("D"),
                    EmailConfirmed = true
                };
                var hashed = passwordHasher.HashPassword(seededUser, "track");
                seededUser.PasswordHash = hashed;
                mb.Entity<UserEntity>().HasData(seededUser);
                var claim = new IdentityUserClaim<int>()
                {
                    Id = 1,
                    UserId = userId,
                    ClaimType = ClaimTypes.NameIdentifier,
                    ClaimValue = "cash",
                };
                mb.Entity<IdentityUserClaim<int>>().HasData(claim);
                var uncategorizedExpenses = new SubCategoryEntity()
                {
                    Id = 1,
                    Name = "Uncategorized",
                    Notes = "Default category for any expense that does not have a category associated with it.",
                    MainCategoryId = 1,
                    InUse = false
                };
                var groceries = new SubCategoryEntity()
                {
                    Id = 2,
                    Name = "Groceries",
                    Notes = "Food purchased to be eaten at home.",
                    MainCategoryId = 2
                };
                var diningOut = new SubCategoryEntity()
                {
                    Id = 3,
                    Name = "Dining Out",
                    Notes = "Any food purchased at a restaurant.",
                    MainCategoryId = 2
                };
                var rent = new SubCategoryEntity()
                {
                    Id = 4,
                    Name = "Rent",
                    Notes = "Monthly rent payment",
                    MainCategoryId = 3
                };
                var gas = new SubCategoryEntity()
                {
                    Id = 5,
                    Name = "Gas",
                    Notes = "Gas purchased for a vehicle",
                    MainCategoryId = 4
                };

                mb.Entity<SubCategoryEntity>().HasData(uncategorizedExpenses);
                mb.Entity<SubCategoryEntity>().HasData(groceries);
                mb.Entity<SubCategoryEntity>().HasData(diningOut);
                mb.Entity<SubCategoryEntity>().HasData(rent);
                mb.Entity<SubCategoryEntity>().HasData(gas);
                var otherMainCategory = new MainCategoryEntity()
                {
                    Id = 1,
                    Name = "Other"
                };
                var food = new MainCategoryEntity()
                {
                    Id = 2,
                    Name = "Food"
                };
                var housing = new MainCategoryEntity()
                {
                    Id = 3,
                    Name = "Housing"
                };
                var transportation = new MainCategoryEntity()
                {
                    Id = 4,
                    Name = "Transportation"
                };
                mb.Entity<MainCategoryEntity>().HasData(otherMainCategory);
                mb.Entity<MainCategoryEntity>().HasData(food);
                mb.Entity<MainCategoryEntity>().HasData(housing);
                mb.Entity<MainCategoryEntity>().HasData(transportation);
                var uncategorizedIncome = new IncomeCategoryEntity()
                {
                    Id = 1,
                    Name = "Uncategorized",
                    Notes = "Default category for any income that does not have a category associated with it.",
                    InUse = false
                };
                var refundCategory = new IncomeCategoryEntity()
                {
                    Id = 2,
                    Name = "Refund",
                    Notes = "Any Expenses that can be categorized as a refund."
                };
                var paycheck = new IncomeCategoryEntity()
                {
                    Id = 3,
                    Name = "Paycheck"
                };
                mb.Entity<IncomeCategoryEntity>().HasData(refundCategory);
                mb.Entity<IncomeCategoryEntity>().HasData(uncategorizedIncome);
                mb.Entity<IncomeCategoryEntity>().HasData(paycheck);
                return;
            }

            string csvFileDirectory = env == CashTrackEnv.Test ?
                Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName, "ct-data", "TestData") :
                env == CashTrackEnv.Development ? //TODO: CHANGE TO PRODUCTION
                Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "ct-data", "DemoData")
                : Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "ct-data");

            if (!Directory.Exists(csvFileDirectory) && env == CashTrackEnv.Test)
            {
                //test data is empty, so new test data is generated here
                throw new Exception("You need to add ct-data to the project or write some code that generates test data");
            }

            if (args.IsEqualTo("seed")) //TODO: ADD PRODUCTION
            {
                var users = CsvParser.ProcessUserFile(Path.Combine(csvFileDirectory, "Users.csv"));
                foreach (var user in users)
                {
                    var passwordHasher = new PasswordHasher<UserEntity>();
                    var seededUser = new UserEntity()
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        NormalizedEmail = user.NormalizedEmail,
                        NormalizedUserName = user.NormalizedUserName,
                        SecurityStamp = Guid.NewGuid().ToString("D"),
                        EmailConfirmed = true
                    };
                    var hashed = passwordHasher.HashPassword(seededUser, user.PasswordHash);
                    seededUser.PasswordHash = hashed;
                    mb.Entity<UserEntity>().HasData(seededUser);
                    var claim = new IdentityUserClaim<int>()
                    {
                        Id = user.Id,
                        UserId = user.Id,
                        ClaimType = ClaimTypes.NameIdentifier,
                        ClaimValue = user.UserName,
                    };
                    mb.Entity<IdentityUserClaim<int>>().HasData(claim);
                }
                mb.Entity<MainCategoryEntity>().HasData(CsvParser.ProcessMainCategoryFile(Path.Combine(csvFileDirectory, "MainCategories.csv")));
                mb.Entity<SubCategoryEntity>().HasData(CsvParser.ProcessSubCategoryFile(Path.Combine(csvFileDirectory, "SubCategories.csv")));
                mb.Entity<IncomeSourceEntity>().HasData(CsvParser.ProcessIncomeSourceFile(Path.Combine(csvFileDirectory, "IncomeSources.csv")));
                mb.Entity<ImportRuleEntity>().HasData(CsvParser.ProcessImportRuleFile(Path.Combine(csvFileDirectory, "ImportRules.csv")));
                var incomeCategories = CsvParser.ProcessIncomeCategoryFile(Path.Combine(csvFileDirectory, "IncomeCategories.csv"));
                mb.Entity<IncomeCategoryEntity>().HasData(incomeCategories);

                //var incomes = GenerateData.Income(incomeCategories);

                //mb.Entity<IncomeEntity>().HasData(incomes);
            }
        }

        //if (args.IsEqualTo("seed") || env == CashTrackEnv.Test)
        //{
        //    var users = CsvParser.ProcessUserFile(Path.Combine(csvFileDirectory, "Users.csv"));
        //    foreach (var user in users)
        //    {
        //        var passwordHasher = new PasswordHasher<UserEntity>();
        //        var seededUser = new UserEntity()
        //        {
        //            Id = user.Id,
        //            UserName = user.UserName,
        //            FirstName = user.FirstName,
        //            LastName = user.LastName,
        //            Email = user.Email,
        //            NormalizedEmail = user.NormalizedEmail,
        //            NormalizedUserName = user.NormalizedUserName,
        //            SecurityStamp = Guid.NewGuid().ToString("D"),
        //            EmailConfirmed = true
        //        };
        //        var hashed = passwordHasher.HashPassword(seededUser, user.PasswordHash);
        //        seededUser.PasswordHash = hashed;
        //        mb.Entity<UserEntity>().HasData(seededUser);
        //        var claim = new IdentityUserClaim<int>()
        //        {
        //            Id = user.Id,
        //            UserId = user.Id,
        //            ClaimType = ClaimTypes.NameIdentifier,
        //            ClaimValue = user.UserName,
        //        };
        //        mb.Entity<IdentityUserClaim<int>>().HasData(claim);
        //    }
        //    mb.Entity<MainCategoryEntity>().HasData(CsvParser.ProcessMainCategoryFile(Path.Combine(csvFileDirectory, "MainCategories.csv")));
        //    mb.Entity<SubCategoryEntity>().HasData(CsvParser.ProcessSubCategoryFile(Path.Combine(csvFileDirectory, "SubCategories.csv")));
        //    mb.Entity<MerchantEntity>().HasData(CsvParser.ProcessMerchantFile(Path.Combine(csvFileDirectory, "Merchants.csv")));
        //    mb.Entity<ExpenseEntity>().HasData(CsvParser.ProcessExpenseFile(Path.Combine(csvFileDirectory, "Expenses.csv")));
        //    mb.Entity<IncomeCategoryEntity>().HasData(CsvParser.ProcessIncomeCategoryFile(Path.Combine(csvFileDirectory, "IncomeCategories.csv")));
        //    mb.Entity<IncomeSourceEntity>().HasData(CsvParser.ProcessIncomeSourceFile(Path.Combine(csvFileDirectory, "IncomeSources.csv")));
        //    mb.Entity<IncomeEntity>().HasData(CsvParser.ProcessIncomeFile(Path.Combine(csvFileDirectory, "Income.csv")));
        //    mb.Entity<ImportRuleEntity>().HasData(CsvParser.ProcessImportRuleFile(Path.Combine(csvFileDirectory, "ImportRules.csv")));
        //    mb.Entity<BudgetEntity>().HasData(CsvParser.ProcessBudgetFile(Path.Combine(csvFileDirectory, "Budgets.csv")));
        //}
    }
}
//public static class GenerateData
//{
//    private static int StartYear => DateTime.Now.AddYears(-2).Year;
//    private static int LastYear => DateTime.Now.AddYears(-1).Year;
//    private static int CurrentYear => DateTime.Now.Year;
//    private static int CurrentMonth => DateTime.Now.Month;
//    private static int CurrentDay => DateTime.Now.Day;
//    public static List<IncomeEntity> Income(List<CsvModels.CsvIncomeCategory> incomeCategories)
//    {
//        var incomes = new List<IncomeEntity>();
//        int currentIncomeId = 0;
//        foreach (var category in incomeCategories)
//        {
//            switch (category.Name)
//            {
//                case "Paycheck":
//                    {
//                        var basePay = 1529M;
//                        for (int i = 1; i < 13; i++)
//                        {
//                            currentIncomeId++;
//                            var payment = new IncomeEntity()
//                            {
//                                Id = currentIncomeId,
//                                Amount = basePay,
//                                Date = new DateTime(StartYear, i, 1),
//                                CategoryId = 3,
//                                SourceId = 1,
//                                IsRefund = false
//                            };
//                            incomes.Add(payment);
//                            currentIncomeId++;
//                            var secondPayment = new IncomeEntity()
//                            {
//                                Id = currentIncomeId,
//                                Amount = basePay,
//                                Date = new DateTime(StartYear, i, 15),
//                                CategoryId = 3,
//                                SourceId = 1,
//                                IsRefund = false
//                            };
//                            incomes.Add(secondPayment);
//                        }

//                        for (int i = 1; i < 13; i++)
//                        {
//                            currentIncomeId++;
//                            var payment = new IncomeEntity()
//                            {
//                                Id = currentIncomeId,
//                                Amount = basePay + 100m,
//                                Date = new DateTime(LastYear, i, 1),
//                                CategoryId = 3,
//                                SourceId = 1,
//                                IsRefund = false
//                            };
//                            incomes.Add(payment);
//                            currentIncomeId++;
//                            var secondPayment = new IncomeEntity()
//                            {
//                                Id = currentIncomeId,
//                                Amount = basePay + 100m,
//                                Date = new DateTime(LastYear, i, 15),
//                                CategoryId = 3,
//                                SourceId = 1,
//                                IsRefund = false
//                            };
//                            incomes.Add(secondPayment);
//                        }

//                        for (int i = 1; i <= CurrentMonth; i++)
//                        {
//                            currentIncomeId++;
//                            var payment = new IncomeEntity()
//                            {
//                                Id = currentIncomeId,
//                                Amount = basePay + 150m,
//                                Date = new DateTime(CurrentYear, i, 1),
//                                CategoryId = 3,
//                                SourceId = 1,
//                                IsRefund = false
//                            };
//                            incomes.Add(payment);
//                            currentIncomeId++;
//                            if (CurrentDay > 15)
//                            {
//                                var secondPayment = new IncomeEntity()
//                                {
//                                    Id = currentIncomeId,
//                                    Amount = basePay + 150m,
//                                    Date = new DateTime(CurrentYear, i, 15),
//                                    CategoryId = 3,
//                                    SourceId = 1,
//                                    IsRefund = false
//                                };
//                                incomes.Add(secondPayment);
//                            }
//                        }
//                    }
//                    break;
//                case "Gift":
//                    {
//                        currentIncomeId++;
//                        var firstpayment = new IncomeEntity()
//                        {
//                            Id = currentIncomeId,
//                            Amount = 100M,
//                            Date = new DateTime(StartYear, 1, 11),
//                            CategoryId = 4,
//                            SourceId = 3,
//                            IsRefund = false,
//                            Notes = "Birthday Money from Parents"
//                        };
//                        incomes.Add(firstpayment);
//                        currentIncomeId++;
//                        var secondPayment = new IncomeEntity()
//                        {
//                            Id = currentIncomeId,
//                            Amount = 200M,
//                            Date = new DateTime(StartYear, 12, 25),
//                            CategoryId = 4,
//                            SourceId = 3,
//                            IsRefund = false,
//                            Notes = "Christmas money from Parents"
//                        };
//                        incomes.Add(secondPayment);
//                        currentIncomeId++;
//                        var thirdPayment = new IncomeEntity()
//                        {
//                            Id = currentIncomeId,
//                            Amount = 100M,
//                            Date = new DateTime(LastYear, 1, 11),
//                            CategoryId = 4,
//                            SourceId = 3,
//                            IsRefund = false,
//                            Notes = "Birthday Money from Parents"
//                        };
//                        incomes.Add(thirdPayment);
//                        currentIncomeId++;
//                        var fourthPayment = new IncomeEntity()
//                        {
//                            Id = currentIncomeId,
//                            Amount = 200M,
//                            Date = new DateTime(LastYear, 12, 25),
//                            CategoryId = 4,
//                            SourceId = 3,
//                            IsRefund = false,
//                            Notes = "Christmas money from Parents"
//                        };
//                        incomes.Add(fourthPayment);
//                        if (CurrentDay > 11)
//                        {
//                            currentIncomeId++;
//                            var fifthpayment = new IncomeEntity()
//                            {
//                                Id = currentIncomeId,
//                                Amount = 100M,
//                                Date = new DateTime(CurrentYear, 1, 11),
//                                CategoryId = 4,
//                                SourceId = 3,
//                                IsRefund = false,
//                                Notes = "Birthday Money from Parents, again"
//                            };
//                            incomes.Add(fifthpayment);
//                        }
//                    }
//                    break;
//                case "Tax Refund":
//                    {
//                        currentIncomeId++;
//                        var payment = new IncomeEntity()
//                        {
//                            Id = currentIncomeId,
//                            Amount = 2123M,
//                            Date = new DateTime(StartYear, 4, 14),
//                            CategoryId = 5,
//                            SourceId = 2,
//                            IsRefund = false,
//                            Notes = "Federal Tax Refund"
//                        };
//                        incomes.Add(payment);

//                        currentIncomeId++;
//                        var secondpayment = new IncomeEntity()
//                        {
//                            Id = currentIncomeId,
//                            Amount = 2243M,
//                            Date = new DateTime(LastYear, 4, 9),
//                            CategoryId = 5,
//                            SourceId = 2,
//                            IsRefund = false,
//                            Notes = "Federal Tax Refund"
//                        };
//                        incomes.Add(secondpayment);

//                        if (CurrentMonth >= 4)
//                        {
//                            currentIncomeId++;
//                            var thirdpayment = new IncomeEntity()
//                            {
//                                Id = currentIncomeId,
//                                Amount = 2156M,
//                                Date = new DateTime(CurrentYear, 4, 1),
//                                CategoryId = 5,
//                                SourceId = 2,
//                                IsRefund = false,
//                                Notes = "Federal Tax Refund"
//                            };
//                            incomes.Add(thirdpayment);
//                        }
//                    }
//                    break;
//            }
//        }
//        return incomes;
//    }
//        public static List<ExpenseEntity> Expenses(List<CsvModels.CsvExpenseSubCategory> categories)
//        {
//            var expenses = new List<ExpenseEntity>();
//            int currentId = 0;
//            var rando = new Random();
//            foreach (var category in categories)
//            {
//                switch (category.Name)
//                {
//                    case "Groceries":
//                        {
//                            var categoryId = category.Id;
//                            //3 times a month, 90-200
//                            for (int i = 1; i < 13; i++)
//                            {
//                                currentId++;
//                                var min = 90;
//                                var max = 210;
//                                var payment = new ExpenseEntity()
//                                {
//                                    Id = currentId,
//                                    Amount =
//                                    Date = new DateTime(StartYear, i, 1),
//                                    CategoryId = 3,
//                                };
//                                expenses.Add(GenerateExpense(rando,
//                                    currentId,
//                                    min, max,
//                                    StartYear, i, rando.Next.Next(1, 10),
//                                    categoryId,

//                                    ));
//                                currentId++;
//                                var secondPayment = new ExpenseEntity()
//                                {
//                                    Id = currentId,
//                                    Amount = basePay,
//                                    Date = new DateTime(StartYear, i, 15),
//                                    CategoryId = 3
//                                };
//                                expenses.Add(secondPayment);
//                            }

//                            for (int i = 1; i < 13; i++)
//                            {
//                                currentId++;
//                                var payment = new ExpenseEntity()
//                                {
//                                    Id = currentId,
//                                    Amount = basePay + 100m,
//                                    Date = new DateTime(LastYear, i, 1),
//                                    CategoryId = 3
//                                };
//                                expenses.Add(payment);
//                                currentId++;
//                                var secondPayment = new ExpenseEntity()
//                                {
//                                    Id = currentId,
//                                    Amount = basePay + 100m,
//                                    Date = new DateTime(LastYear, i, 15),
//                                    CategoryId = 3
//                                };
//                                expenses.Add(secondPayment);
//                            }

//                            for (int i = 1; i <= CurrentMonth; i++)
//                            {
//                                currentId++;
//                                var payment = new ExpenseEntity()
//                                {
//                                    Id = currentId,
//                                    Amount = basePay + 150m,
//                                    Date = new DateTime(CurrentYear, i, 1),
//                                    CategoryId = 3
//                                };
//                                expenses.Add(payment);
//                                currentId++;
//                                if (CurrentDay > 15)
//                                {
//                                    var secondPayment = new ExpenseEntity()
//                                    {
//                                        Id = currentId,
//                                        Amount = basePay + 150m,
//                                        Date = new DateTime(CurrentYear, i, 15),
//                                        CategoryId = 3
//                                    };
//                                    expenses.Add(secondPayment);
//                                }
//                            }
//                        }
//                        break;
//                }
//            }
//            return expenses;

//            static List<ExpenseEntity> GenerateExpenses(Random rando, int category, int merchant, int id, int min, int max, int year, int month, int day, int numberToGenerateInAMonth = 1, string notes = "")
//            {
//                var listOfExpenses = new List<ExpenseEntity>();
//                switch (numberToGenerateInAMonth)
//                {
//                    case 1:
//                        {
//                            for (int i = 1; i < 13; i++)
//                            {
//                                id++;
//                                // first Year
//                                var ex = new ExpenseEntity()
//                                { 
//                                    Id = id,
//                                    Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) * rando.NextDouble()), 2),
//                                    Date = new DateTime(year, month, day),
//                                    CategoryId = category,
//                                    MerchantId = merchant,
//                                    Notes = notes
//                                };
//                                listOfExpenses.Add(ex);
//                            }
//                        }
//                        break;
//                    case 2:
//                        break;
//                    case 3:
//                        break;
//                    case 4:
//                        break;
//                }
//                return listOfExpenses;
//            }
//        }
//    }
//}
//new ExpenseEntity()
//{
//    Id = id,
//    Amount = Math.Round(Convert.ToDecimal(rando.Next(min, max) * rando.NextDouble()), 2),
//    Date = new DateTime(year, month, day),
//    CategoryId = category,
//    MerchantId = merchant,
//    Notes = notes
//};