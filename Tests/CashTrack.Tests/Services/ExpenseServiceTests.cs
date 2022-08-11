using AutoMapper;
using CashTrack.Data.Entities;
using CashTrack.Repositories.ExpenseRepository;
using CashTrack.Services.ExpenseService;
using Moq;
using System;
using System.Linq;
using Xunit;
using Shouldly;
using CashTrack.Models.ExpenseModels;
using CashTrack.Models.Common;
using Bogus;
using System.Threading.Tasks;
using CashTrack.Services.Common;
using CashTrack.Repositories.IncomeRepository;
using CashTrack.Repositories.MerchantRepository;
using CashTrack.Repositories.SubCategoriesRepository;
using System.Linq.Expressions;

namespace CashTrack.Tests.Services
{
    public class ExpenseServiceTests
    {
        private readonly Mock<IExpenseRepository> _expenseRepo;
        private readonly Mock<IIncomeRepository> _incomeRepo;
        private readonly Mock<IMerchantRepository> _merchantRepo;
        private readonly Mock<ISubCategoryRepository> _subcategoryRepo;
        private readonly IMapper _mapper;
        private ExpenseService _sut;
        private readonly ExpenseEntity[] _data;

        public ExpenseServiceTests()
        {
            _expenseRepo = new Mock<IExpenseRepository>();
            _incomeRepo = new Mock<IIncomeRepository>();
            _merchantRepo = new Mock<IMerchantRepository>();
            _subcategoryRepo = new Mock<ISubCategoryRepository>();
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new ExpenseMapperProfile()));
            _mapper = mapperConfig.CreateMapper();

            _sut = new ExpenseService(_expenseRepo.Object, _incomeRepo.Object, _merchantRepo.Object, _mapper, _subcategoryRepo.Object);
            _data = GetData();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task Get_Expense_By_Id(int id)
        {
            _expenseRepo.Setup(r => r.FindById(id)).ReturnsAsync(_data[id-1]);
            var result = await _sut.GetExpenseByIdAsync(id);
            result.Id.ShouldBe(id);
        }
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task Get_Expense_Refund_By_Id(int id)
        {
            _expenseRepo.Setup(r => r.FindById(id)).ReturnsAsync(_data[id - 1]);
            var result = await _sut.GetExpenseRefundByIdAsync(id);
            result.Id.ShouldBe(id);
            result.ShouldBeOfType<ExpenseRefund>();
        }
        [Theory]
        [InlineData(DateOptions.All)]
        [InlineData(DateOptions.SpecificDate)]
        [InlineData(DateOptions.SpecificMonthAndYear)]
        [InlineData(DateOptions.SpecificQuarter)]
        [InlineData(DateOptions.SpecificYear)]
        [InlineData(DateOptions.DateRange)]
        [InlineData(DateOptions.Last30Days)]
        [InlineData(DateOptions.CurrentMonth)]
        [InlineData(DateOptions.CurrentQuarter)]
        [InlineData(DateOptions.CurrentYear)]
        public async Task Get_Expenses(DateOptions option)
        {
            var request = new ExpenseRequest()
            {
                DateOptions = option
            };
            _expenseRepo.Setup(r => r.FindWithPagination(It.IsAny<Expression<Func<ExpenseEntity, bool>>>(), 1, 20)).ReturnsAsync(_data);
            _expenseRepo.Setup(r => r.GetCount(It.IsAny<Expression<Func<ExpenseEntity, bool>>>())).ReturnsAsync(3);
            _expenseRepo.Setup(r => r.GetAmountOfExpenses(It.IsAny<Expression<Func<ExpenseEntity, bool>>>())).ReturnsAsync(45);
            var result = await _sut.GetExpensesAsync(request);
            result.ListItems.Count().ShouldBe(3);
        }
        [Theory]
        [InlineData("test")]
        public async Task Get_Expenses_By_Notes(string notes)
        {
            var request = new ExpenseRequest()
            {
                DateOptions = DateOptions.All,
                Query = notes
            };
            _expenseRepo.Setup(r => r.FindWithPagination(It.IsAny<Expression<Func<ExpenseEntity, bool>>>(), 1, 20)).ReturnsAsync(_data);
            _expenseRepo.Setup(r => r.GetCount(It.IsAny<Expression<Func<ExpenseEntity, bool>>>())).ReturnsAsync(3);
            _expenseRepo.Setup(r => r.GetAmountOfExpenses(It.IsAny<Expression<Func<ExpenseEntity, bool>>>())).ReturnsAsync(45);
            var result = await _sut.GetExpensesByNotesAsync(request);
            result.ListItems.Count().ShouldBe(3);
        }
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task Get_Expenses_By_SubCategory_Id(int id)
        {
            var request = new ExpenseRequest()
            {
                DateOptions = DateOptions.All,
                Query = id.ToString()
            };
            _expenseRepo.Setup(r => r.FindWithPagination(It.IsAny<Expression<Func<ExpenseEntity, bool>>>(), 1, 20)).ReturnsAsync(_data.Where(x => x.Category!.Id == id).ToArray());
            _expenseRepo.Setup(r => r.GetCount(It.IsAny<Expression<Func<ExpenseEntity, bool>>>())).ReturnsAsync(1);
            _expenseRepo.Setup(r => r.GetAmountOfExpenses(It.IsAny<Expression<Func<ExpenseEntity, bool>>>())).ReturnsAsync(45);
            var result = await _sut.GetExpensesBySubCategoryIdAsync(request);
            result.ListItems.Count().ShouldBe(1);
        }
        [Theory]
        [InlineData("Costco")]
        public async Task Get_Expenses_By_Merchant(string merchant)
        {
            var request = new ExpenseRequest()
            {
                DateOptions = DateOptions.All,
                Query = merchant
            };
            _expenseRepo.Setup(r => r.FindWithPagination(It.IsAny<Expression<Func<ExpenseEntity, bool>>>(), 1, 20)).ReturnsAsync(_data.Where(x => x.Merchant!.Name == merchant).ToArray());
            _expenseRepo.Setup(r => r.GetCount(It.IsAny<Expression<Func<ExpenseEntity, bool>>>())).ReturnsAsync(3);
            _expenseRepo.Setup(r => r.GetAmountOfExpenses(It.IsAny<Expression<Func<ExpenseEntity, bool>>>())).ReturnsAsync(45);
            var result = await _sut.GetExpensesByMerchantAsync(request);
            result.ListItems.Count().ShouldBe(1);
        }
        [Theory]
        [InlineData(5)]
        [InlineData(15)]
        [InlineData(25)]
        public async Task Get_Expenses_By_Amount(decimal amount)
        {
            var request = new AmountSearchRequest()
            {
                Query = amount
            };
            _expenseRepo.Setup(r => r.FindWithPagination(It.IsAny<Expression<Func<ExpenseEntity, bool>>>(), 1, 20)).ReturnsAsync(_data.Where(x => x.Amount == amount).ToArray());
            _expenseRepo.Setup(r => r.GetCount(It.IsAny<Expression<Func<ExpenseEntity, bool>>>())).ReturnsAsync(3);
            _expenseRepo.Setup(r => r.GetAmountOfExpenses(It.IsAny<Expression<Func<ExpenseEntity, bool>>>())).ReturnsAsync(45);
            var result = await _sut.GetExpensesByAmountAsync(request);
            result.ListItems.Count().ShouldBe(1);
        }
        [Fact]
        public async Task Create_Expense()
        {
            _expenseRepo.Setup(r => r.Create(It.IsAny<ExpenseEntity>())).ReturnsAsync(1);
            var expense = new Expense()
            {
                Amount = 5,
                Date = DateTime.Now,
                SubCategoryId = 1,
                ExcludeFromStatistics = true,
                Notes = "notes",
            };
            var result = await _sut.CreateExpenseAsync(expense);
            result.ShouldBe(1);
        }
        [Fact]
        public async Task Create_Expense_From_Split()
        {
            var split = new ExpenseSplit()
            {
                Amount = 2,
                Date = DateTime.UtcNow,
                Taxed = false,
                SubCategoryId=1
            };
            _expenseRepo.Setup(r => r.Create(It.IsAny<ExpenseEntity>())).ReturnsAsync(1);
            var expense = _data[0];
            var result = await _sut.CreateExpenseFromSplitAsync(split);
            result.ShouldBe(1);
        }
        [Fact]
        public async Task Update_Expense()
        {
            var editedExpense = new Expense()
            {
                Id=1,
                Amount = 5,
                Date = DateTime.Now,
                SubCategoryId = 2,
                ExcludeFromStatistics = true,
                Notes = "notes",
            };
            var currentExpense = new ExpenseEntity()
            {
                Id = 1,
                Amount = 5,
                Date = DateTime.Now,
                CategoryId = 1,
                ExcludeFromStatistics = true,
                Notes = "notes",
            };
            _expenseRepo.Setup(r => r.Update(It.IsAny<ExpenseEntity>())).ReturnsAsync(1);
            _expenseRepo.Setup(r => r.FindById(It.IsAny<int>())).ReturnsAsync(currentExpense);
            _subcategoryRepo.Setup(r => r.FindById(It.IsAny<int>())).ReturnsAsync(new SubCategoryEntity() {Id = 1, Name="test" });
            var expense = _data[0];
            var result = await _sut.UpdateExpenseAsync(editedExpense);
            result.ShouldBe(1);
        }
        [Fact]
        public async Task Delete_Expense()
        {
            var currentExpense = new ExpenseEntity()
            {
                Id = 1,
                Amount = 5,
                Date = DateTime.Now,
                CategoryId = 1,
                ExcludeFromStatistics = true,
                Notes = "notes",
            };
            _expenseRepo.Setup(r => r.FindById(It.IsAny<int>())).ReturnsAsync(currentExpense);
            _expenseRepo.Setup(r => r.Delete(It.IsAny<ExpenseEntity>())).ReturnsAsync(true);
            var result = await _sut.DeleteExpenseAsync(1);
            result.ShouldBe(true);
        }
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task Get_Expenses_By_MainCategory_Id(int id)
        {
            var request = new ExpenseRequest()
            {
                DateOptions = DateOptions.All,
                Query = id.ToString()
            };
            _expenseRepo.Setup(r => r.FindWithPagination(It.IsAny<Expression<Func<ExpenseEntity, bool>>>(), 1, 20)).ReturnsAsync(_data.Where(x => x.Category!.MainCategory!.Id == id).ToArray());
            _expenseRepo.Setup(r => r.GetCount(It.IsAny<Expression<Func<ExpenseEntity, bool>>>())).ReturnsAsync(1);
            _expenseRepo.Setup(r => r.GetAmountOfExpenses(It.IsAny<Expression<Func<ExpenseEntity, bool>>>())).ReturnsAsync(45);
            var result = await _sut.GetExpensesByMainCategoryAsync(request);
            result.ListItems.Count().ShouldBe(1);
        }
        [Fact]
        public async Task Get_Expenses_By_Date_No_Pagination() 
        {
            _expenseRepo.Setup(r => r.Find(It.IsAny<Expression<Func<ExpenseEntity, bool>>>())).ReturnsAsync(_data);
            var result = await _sut.GetExpensesByDateWithoutPaginationAsync(DateTime.Now);
        }

        private static ExpenseEntity[] GetData()
        {
            return new ExpenseEntity[]
            {
                new ExpenseEntity() {
                    Id = 1,
                    Date = DateTimeOffset.UtcNow.AddDays(-3),
                    Amount = 25.00m,
                    Category = new SubCategoryEntity() {
                        Id=1,
                        Name="Groceries",
                        MainCategory = new MainCategoryEntity() {
                            Id=1,
                            Name= "Food"
                        }},
                    ExcludeFromStatistics=false,
                    Notes="Test",
                    Merchant = new MerchantEntity(){
                        Id = 1,
                        Name = "Costco"
                    }
                },
                new ExpenseEntity() {
                    Id = 2,
                    Date = DateTimeOffset.UtcNow.AddDays(-2),
                    Amount = 15.00m,
                    Category = new SubCategoryEntity() {
                        Id=2,
                        Name="Car",
                        MainCategory = new MainCategoryEntity() {
                            Id=2,
                            Name= "Transportation"
                        }},
                    ExcludeFromStatistics=false,
                    Notes="Test1",
                    Merchant = new MerchantEntity(){
                        Id = 1,
                        Name = "Costco Tire Center"
                    }
                },
                new ExpenseEntity() {
                    Id = 3,
                    Date = DateTimeOffset.UtcNow.AddDays(-1),
                    Amount = 5.00m,
                    Category = new SubCategoryEntity() {
                        Id=3,
                        Name="House",
                        MainCategory = new MainCategoryEntity() {
                            Id=3,
                            Name= "Household"
                        }},
                    ExcludeFromStatistics=false,
                    Notes="Test2",
                    Merchant = new MerchantEntity(){
                        Id = 1,
                        Name = "Costco Food Court"
                    }
                },
            };
        }
    }
}
