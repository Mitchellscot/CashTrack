using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using CashTrack.Data.Entities;
using CashTrack.Data.CsvFiles;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

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
            var csvFileDirectory = _config["CsvFileDirectory"];

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
            mb.Ignore<IdentityRole<int>>();
            mb.Ignore<IdentityRoleClaim<int>>();
            mb.Ignore<IdentityUserRole<int>>();
            mb.Ignore<IdentityUserLogin<int>>();

            mb.Entity<MainCategories>().HasData(CsvParser.ProcessMainCategoryFile(csvFileDirectory + "main-categories.csv"));
            mb.Entity<SubCategories>().HasData(CsvParser.ProcessSubCategoryFile(csvFileDirectory + "sub-categories.csv"));
            mb.Entity<Merchants>().HasData(CsvParser.ProcessMerchantFile(csvFileDirectory + "merchants.csv"));
            mb.Entity<Expenses>().HasData(CsvParser.ProcessExpenseFile(csvFileDirectory + "expenses.csv"));
            mb.Entity<IncomeCategories>().HasData(CsvParser.ProcessIncomeCategoryFile(csvFileDirectory + "income-categories.csv"));
            mb.Entity<IncomeSources>().HasData(CsvParser.ProcessIncomeSourceFile(csvFileDirectory + "income-sources.csv"));
            mb.Entity<Incomes>().HasData(CsvParser.ProcessIncomeFile(csvFileDirectory + "incomes.csv"));
            //mb.Entity<Users>().HasData(CsvParser.ProcessUserFile(csvFileDirectory + "users.csv"));

        }
    }
}