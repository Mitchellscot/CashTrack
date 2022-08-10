//using AutoMapper;
//using CashTrack.Data.Entities;
//using CashTrack.Repositories.ExpenseRepository;
//using CashTrack.Services.ExpenseService;
//using Moq;
//using System;
//using System.Linq;
//using Xunit;
//using Shouldly;
//using CashTrack.Models.ExpenseModels;
//using CashTrack.Models.Common;
//using Bogus;
//using System.Threading.Tasks;
//using CashTrack.Services.Common;

//namespace CashTrack.Tests.Services
//{
//    public class ExpenseServiceTests
//    {
//        private readonly Mock<IExpenseRepository> _repo;
//        private readonly IMapper _mapper;
//        private ExpenseService _sut;
//        private readonly ExpenseEntity[] _data;

//        public ExpenseServiceTests()
//        {
//            _repo = new Mock<IExpenseRepository>();
//            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new ExpenseMapperProfile()));
//            _mapper = mapperConfig.CreateMapper();
//            _sut = new ExpenseService(_repo.Object, _mapper);
//            _data = GetData();
//        }
//        [Fact]
//        public async Task Create()
//        {
//            _repo.Setup(x => x.Create(It.IsAny<ExpenseEntity>())).ReturnsAsync(true);
//            var request = new AddEditExpense() { Amount = 1m, Date = DateTimeOffset.UtcNow, SubCategoryId = 12 };
//            var result = await _sut.CreateExpenseAsync(request);
//            result.Amount.ShouldBe(1m);
//        }
//        [Fact]
//        public async Task Update()
//        {
//            _repo.Setup(x => x.Update(It.IsAny<Expenses>())).ReturnsAsync(true);
//            var objectToUpdate = new Expenses { id = 1, amount = 1m, date = DateTimeOffset.UtcNow, categoryid = 12 };
//            _repo.Setup(x => x.FindById(1)).ReturnsAsync(objectToUpdate);
//            var request = new AddEditExpense() { Id = 1, Amount = 2m };
//            var result = await _sut.UpdateExpenseAsync(request);
//            result.ShouldBe(true);
//        }
//        [Fact]
//        public async Task Delete()
//        {
//            _repo.Setup(x => x.Delete(It.IsAny<ExpenseEntiy>())).ReturnsAsync(true);
//            var objectToUpdate = new Expenses() { id = 1, amount = 5m, categoryid = 12, date = DateTimeOffset.UtcNow };
//            _repo.Setup(x => x.FindById(1)).ReturnsAsync(objectToUpdate);
//            var result = await _sut.DeleteExpenseAsync(1);
//            result.ShouldBe(true);
//        }
//        [Fact]
//        public async Task GetById()
//        {
//            _repo.Setup(r => r.FindById(3)).ReturnsAsync(_data.Last());
//            var result = await _sut.GetExpenseByIdAsync(3);
//            result.Id.ShouldBe(3);
//        }
//        [Fact]
//        public async Task GetAll()
//        {
//            _repo.Setup(r => r.FindWithPagination(x => true, 1, 25)).ReturnsAsync(_data);
//            _repo.Setup(r => r.GetAmountOfExpenses(x => true)).ReturnsAsync(_data.Sum(x => x.Amount));
//            _repo.Setup(r => r.GetCount(x => true)).ReturnsAsync(_data.Count());
//            var request = new ExpenseRequest()
//            {
//                DateOptions = DateOptions.All
//            };
//            var result = await _sut.GetExpensesAsync(request);
//            result.ListItems.Count().ShouldBe(3);
//            result.TotalPages.ShouldBe(1);
//            result.TotalAmount.ShouldBe(45.00m);
//        }

//        [Theory]
//        [InlineData(DateOptions.All)]
//        [InlineData(DateOptions.SpecificDate)]
//        [InlineData(DateOptions.SpecificMonthAndYear)]
//        [InlineData(DateOptions.SpecificQuarter)]
//        [InlineData(DateOptions.SpecificYear)]
//        [InlineData(DateOptions.DateRange)]
//        [InlineData(DateOptions.Last30Days)]
//        [InlineData(DateOptions.CurrentMonth)]
//        [InlineData(DateOptions.CurrentQuarter)]
//        [InlineData(DateOptions.CurrentYear)]
//        public void GetPredicateWorks(DateOptions option)
//        {
//            var request = new ExpenseRequest() { DateOptions = option };
//            var result = DateOption<Expenses, ExpenseRequest>.Parse(request);
//            result.NodeType.ShouldBe(System.Linq.Expressions.ExpressionType.Lambda);
//            result.ShouldNotBeNull();
//        }
//    }
//}
