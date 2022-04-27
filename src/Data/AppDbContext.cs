using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using CashTrack.Data.Entities;
using CashTrack.Data.CsvFiles;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Claims;

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

        private IConfiguration _config;

        public AppDbContext(DbContextOptions options, IConfiguration config) : base(options)
        {
            this._config = config;
        }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);
            mb.Initialize(_config.GetSection("Users").Get<UserEntity[]>(), _config["CsvFileDirectory"]);
        }
    }
    //model builder extension to seed DB data
    public static class SeedData
    {
        public static void Initialize(this ModelBuilder mb, UserEntity[] users, string csvFileDirectory)
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

            mb.Entity<MainCategoryEntity>().HasData(CsvParser.ProcessMainCategoryFile(csvFileDirectory + "MainCategories.csv"));
            mb.Entity<SubCategoryEntity>().HasData(CsvParser.ProcessSubCategoryFile(csvFileDirectory + "SubCategories.csv"));
            mb.Entity<MerchantEntity>().HasData(CsvParser.ProcessMerchantFile(csvFileDirectory + "Merchants.csv"));
            mb.Entity<ExpenseEntity>().HasData(CsvParser.ProcessExpenseFile(csvFileDirectory + "Expenses.csv"));
            mb.Entity<IncomeCategoryEntity>().HasData(CsvParser.ProcessIncomeCategoryFile(csvFileDirectory + "IncomeCategories.csv"));
            mb.Entity<IncomeSourceEntity>().HasData(CsvParser.ProcessIncomeSourceFile(csvFileDirectory + "IncomeSources.csv"));
            mb.Entity<IncomeEntity>().HasData(CsvParser.ProcessIncomeFile(csvFileDirectory + "Income.csv"));
            mb.Entity<ImportRuleEntity>().HasData(CsvParser.ProcessImportRuleFile(csvFileDirectory + "ImportRules.csv"));
        }
    }
}