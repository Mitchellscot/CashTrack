//using AutoMapper;
//using CashTrack.Data.Entities;
//using CashTrack.Repositories.IncomeRepository;
//using CashTrack.Services.IncomeService;
//using Moq;
//using System;
//using System.Linq;
//using Xunit;
//using Shouldly;
//using CashTrack.Models.IncomeModels;
//using CashTrack.Models.Common;
//using Bogus;
//using System.Threading.Tasks;
//using CashTrack.Services.Common;

//namespace CashTrack.Tests.Services
//{
//    public class IncomeServiceTests
//    {
//        private readonly Mock<IIncomeRepository> _repo;
//        private readonly IMapper _mapper;
//        private IncomeService _sut;
//        private readonly Incomes[] _data;

//        public IncomeServiceTests()
//        {
//            _repo = new Mock<IIncomeRepository>();
//            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new IncomeMapperProfile()));
//            _mapper = mapperConfig.CreateMapper();
//            _sut = new IncomeService(_repo.Object, _mapper);
//            _data = GetData();
//        }
//        [Fact]
//        public async void GetById()
//        {
//            _repo.Setup(r => r.FindById(3)).ReturnsAsync(_data.Last());
//            var result = await _sut.GetIncomeByIdAsync(3);
//            result.Id.ShouldBe(3);
//        }
//        [Fact]
//        public async Task Create()
//        {
//            _repo.Setup(x => x.Create(It.IsAny<Incomes>())).ReturnsAsync(true);
//            var request = new AddEditIncome() { Amount = 1m, Date = DateTime.UtcNow, CategoryId = 1, };
//            var result = await _sut.CreateIncomeAsync(request);
//            result.Amount.ShouldBe(1m);
//        }
//        [Fact]
//        public async Task Update()
//        {
//            _repo.Setup(x => x.Update(It.IsAny<Incomes>())).ReturnsAsync(true);
//            var objectToUpdate = new Incomes { id = 1, amount = 1m, date = DateTime.UtcNow };
//            _repo.Setup(x => x.FindById(1)).ReturnsAsync(objectToUpdate);
//            var request = new AddEditIncome() { Id = 1, Amount = 2m };
//            var result = await _sut.UpdateIncomeAsync(request);
//            result.ShouldBe(true);
//        }
//        [Fact]
//        public async Task Delete()
//        {
//            _repo.Setup(x => x.Delete(It.IsAny<Incomes>())).ReturnsAsync(true);
//            var objectToUpdate = new Incomes() { id = 1, amount = 5m, categoryid = 12, date = DateTime.UtcNow };
//            _repo.Setup(x => x.FindById(1)).ReturnsAsync(objectToUpdate);
//            var result = await _sut.DeleteIncomeAsync(1);
//            result.ShouldBe(true);
//        }
//        [Fact]
//        public async void GetAll()
//        {
//            _repo.Setup(r => r.FindWithPagination(x => true, 1, 25)).ReturnsAsync(_data);
//            _repo.Setup(r => r.GetAmountOfIncome(x => true)).ReturnsAsync(_data.Sum(x => x.amount));
//            _repo.Setup(r => r.GetCount(x => true)).ReturnsAsync(_data.Count());
//            var request = new IncomeRequest()
//            {
//                DateOptions = DateOptions.All
//            };
//            var result = await _sut.GetIncomeAsync(request);
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
//            var request = new IncomeRequest() { DateOptions = option };
//            var result = DateOption<Incomes, IncomeRequest>.Parse(request);
//            result.NodeType.ShouldBe(System.Linq.Expressions.ExpressionType.Lambda);
//            result.ShouldNotBeNull();
//        }
//        private static Incomes[] GetData()
//        {
//            return new Incomes[]
//            {
//                new Incomes() {
//                    id = 1,
//                    date = DateTime.UtcNow.AddDays(-3),
//                    amount = 25.00m,
//                    category = new IncomeCategories() {
//                        id=1,
//                        category="Paycheck" }

//                },
//                new Incomes() {
//                    id = 2,
//                    date = DateTime.UtcNow.AddDays(-3),
//                    amount = 15.00m,
//                    category = new IncomeCategories() {
//                        id=1,
//                        category="Gift" }

//                },
//                new Incomes() {
//                    id = 3,
//                    date = DateTime.UtcNow.AddDays(-3),
//                    amount = 5.00m,
//                    category = new IncomeCategories() {
//                        id=1,
//                        category="Bonus" }

//                },
//            };
//        }
//    }
//}
