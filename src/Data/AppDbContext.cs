using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using CashTrack.Data.Entities;
using CashTrack.Data.CsvFiles;

namespace CashTrack.Data
{
    public class AppDbContext : DbContext
    {

        public DbSet<Users> Users { get; set; }
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
            var csvFileDirectory = _config["CsvFileDirectory"];

            mb.Entity<ExpenseTags>().HasKey(et => new { et.expense_id, et.tag_id });

            mb.Entity<ExpenseTags>()
                .HasOne<Expenses>(et => et.expense)
                .WithMany(e => e.expense_tags)
                .HasForeignKey(et => et.expense_id);
            mb.Entity<ExpenseTags>()
                .HasOne<Tags>(et => et.tag)
                .WithMany(e => e.expense_tags)
                .HasForeignKey(et => et.tag_id);

            //mb.Entity<MainCategories>().HasData(CsvParser.ProcessMainCategoryFile(csvFileDirectory + "main-categories.csv"));
            //mb.Entity<SubCategories>().HasData(CsvParser.ProcessSubCategoryFile(csvFileDirectory + "sub-categories.csv"));
            //mb.Entity<Merchants>().HasData(CsvParser.ProcessMerchantFile(csvFileDirectory + "merchants.csv"));
            //mb.Entity<Expenses>().HasData(CsvParser.ProcessExpenseFile(csvFileDirectory + "expenses.csv"));
            //mb.Entity<IncomeCategories>().HasData(CsvParser.ProcessIncomeCategoryFile(csvFileDirectory + "income-categories.csv"));
            //mb.Entity<IncomeSources>().HasData(CsvParser.ProcessIncomeSourceFile(csvFileDirectory + "income-sources.csv"));
            //mb.Entity<Incomes>().HasData(CsvParser.ProcessIncomeFile(csvFileDirectory + "incomes.csv"));
            //mb.Entity<Users>().HasData(CsvParser.ProcessUserFile(csvFileDirectory + "users.csv"));

        }
    }
}