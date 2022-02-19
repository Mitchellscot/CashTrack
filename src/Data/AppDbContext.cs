using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using CashTrack.Data.Entities;
using CashTrack.Data.CsvFiles;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace CashTrack.Data
{
    public class AppDbContext : IdentityDbContext<Users, IdentityRole<int>, int>
    {

        //public DbSet<Users> Users { get; set; }
        public DbSet<Expenses> Expenses { get; set; }
        public DbSet<Incomes> Incomes { get; set; }
        public DbSet<MainCategories> MainCategories { get; set; }
        public DbSet<SubCategories> SubCategories { get; set; }
        public DbSet<Merchants> Merchants { get; set; }
        public DbSet<Tags> Tags { get; set; }
        public DbSet<IncomeSources> IncomeSources { get; set; }
        public DbSet<IncomeCategories> IncomeCategories { get; set; }
        public DbSet<ExpenseReview> ExpensesToReview { get; set; }
        public DbSet<IncomeReview> IncomeToReview { get; set; }

        private IConfiguration _config;

        public AppDbContext(DbContextOptions options, IConfiguration config) : base(options)
        {
            this._config = config;
        }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);
            mb.Initialize(_config.GetSection("Users").Get<Users[]>(), _config["CsvFileDirectory"]);
        }
    }
    //model builder extension to seed DB data
    public static class SeedData
    {
        public static void Initialize(this ModelBuilder mb, Users[] users, string csvFileDirectory)
        {
            foreach (var user in users)
            {
                var password = new PasswordHasher<Users>();
                var seededUser = new Users()
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
                mb.Entity<Users>().HasData(seededUser);
                var claim = new IdentityUserClaim<int>()
                {
                    Id = user.Id,
                    UserId = user.Id,
                    ClaimType = ClaimTypes.NameIdentifier,
                    ClaimValue = user.UserName,
                };
                mb.Entity<IdentityUserClaim<int>>().HasData(claim);
            }

            mb.Entity<ExpenseTags>().HasKey(et => new { et.expense_id, et.tag_id });

            mb.Entity<ExpenseTags>()
                .HasOne(et => et.expense)
                .WithMany(e => e.expense_tags)
                .HasForeignKey(et => et.expense_id);
            mb.Entity<ExpenseTags>()
                .HasOne(et => et.tag)
                .WithMany(e => e.expense_tags)
                .HasForeignKey(et => et.tag_id);

            mb.Entity<Users>().ToTable("users");
            mb.Entity<IdentityUserClaim<int>>().ToTable("users_claims");
            mb.Entity<IdentityUserToken<int>>().ToTable("users_tokens");
            mb.Entity<IdentityUserLogin<int>>().ToTable("users_logins");
            mb.Ignore<IdentityRole<int>>();
            mb.Ignore<IdentityRoleClaim<int>>();
            mb.Ignore<IdentityUserRole<int>>();

            mb.Entity<MainCategories>().HasData(CsvParser.ProcessMainCategoryFile(csvFileDirectory + "main-categories.csv"));
            mb.Entity<SubCategories>().HasData(CsvParser.ProcessSubCategoryFile(csvFileDirectory + "sub-categories.csv"));
            mb.Entity<Merchants>().HasData(CsvParser.ProcessMerchantFile(csvFileDirectory + "merchants.csv"));
            mb.Entity<Expenses>().HasData(CsvParser.ProcessExpenseFile(csvFileDirectory + "expenses.csv"));
            mb.Entity<IncomeCategories>().HasData(CsvParser.ProcessIncomeCategoryFile(csvFileDirectory + "income-categories.csv"));
            mb.Entity<IncomeSources>().HasData(CsvParser.ProcessIncomeSourceFile(csvFileDirectory + "income-sources.csv"));
            mb.Entity<Incomes>().HasData(CsvParser.ProcessIncomeFile(csvFileDirectory + "incomes.csv"));


        }
    }
}