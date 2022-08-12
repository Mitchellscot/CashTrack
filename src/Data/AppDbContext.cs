using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using CashTrack.Data.Entities;
using CashTrack.Data.CsvFiles;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Claims;
using System.IO;
using Microsoft.Extensions.Options;
using CashTrack.Common;
using Microsoft.AspNetCore.Hosting;

namespace CashTrack.Data
{
    public class AppDbContext : IdentityDbContext<UserEntity, IdentityRole<int>, int>
    {
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

        private IOptions<AppSettingsOptions> _appSettings;
        private readonly IWebHostEnvironment _env;

        public AppDbContext(DbContextOptions options, IOptions<AppSettingsOptions> appSettings, IWebHostEnvironment env, IConfiguration args) : base(options)
        {

            _appSettings = appSettings;
            _env = env;
        }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);
            var csvFileDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Code", "CashTrack", "ct-data");
            mb.Initialize(_appSettings.Value.Users, csvFileDirectory, _env.EnvironmentName, _appSettings.Value.CreateDb);
        }
    }
    //model builder extension to seed DB data
    public static class SeedData
    {
        public static void Initialize(this ModelBuilder mb, UserEntity[] users, string csvFileDirectory, string env, bool emptyDatabase)
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

            if (env == "Test" || emptyDatabase)
            {
                foreach (var user in users)
                {
                    var password = new PasswordHasher<UserEntity>();
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
                    var hashed = password.HashPassword(seededUser, user.PasswordHash);
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
                mb.Entity<MerchantEntity>().HasData(CsvParser.ProcessMerchantFile(Path.Combine(csvFileDirectory, "Merchants.csv")));
                mb.Entity<ExpenseEntity>().HasData(CsvParser.ProcessExpenseFile(Path.Combine(csvFileDirectory, "Expenses.csv")));
                mb.Entity<IncomeCategoryEntity>().HasData(CsvParser.ProcessIncomeCategoryFile(Path.Combine(csvFileDirectory, "IncomeCategories.csv")));
                mb.Entity<IncomeSourceEntity>().HasData(CsvParser.ProcessIncomeSourceFile(Path.Combine(csvFileDirectory, "IncomeSources.csv")));
                mb.Entity<IncomeEntity>().HasData(CsvParser.ProcessIncomeFile(Path.Combine(csvFileDirectory, "Income.csv")));
                mb.Entity<ImportRuleEntity>().HasData(CsvParser.ProcessImportRuleFile(Path.Combine(csvFileDirectory, "ImportRules.csv")));
            }

        }
    }
}