using AutoMapper;
using CashTrack.Models.IncomeModels;
using CashTrack.Services.IncomeService;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using CashTrack.Controllers;

namespace CashTrack.Tests.Controllers
{
    public class IncomeControllerTests
    {
        private readonly IncomeController _sut;
        public readonly IMapper _mapper;
        public readonly Mock<IIncomeService> _service;

        public IncomeControllerTests()
        {
            _mapper = Mock.Of<IMapper>();
            _service = new Mock<IIncomeService>();
            _sut = new IncomeController(_service.Object);
        }
        //[Fact]
        //public async void GetById()
        //{
        //    var result = await _sut.GetAnIncomeById(1);
        //    var viewResult = Assert.IsType<ActionResult<IncomeListItem>>(result);
        //    _service.Verify(s => s.GetIncomeByIdAsync(It.IsAny<int>()), Times.AtLeastOnce());
        //}
        [Fact]
        public async void GetAll()
        {
            var request = new IncomeRequest();
            var result = await _sut.GetIncomes(request);
            var viewResult = Assert.IsType<ActionResult<IncomeResponse>>(result);
            _service.Verify(s => s.GetIncomeAsync(It.IsAny<IncomeRequest>()), Times.AtLeastOnce());
        }
        //[Fact]
        //public async void GetByNotes()
        //{
        //    var request = new IncomeRequest() { Query = "test" };
        //    var result = await _sut.GetIncomesByNotes(request);
        //    var viewResult = Assert.IsType<ActionResult<IncomeResponse>>(result);
        //    _service.Verify(s => s.GetIncomesByNotesAsync(It.IsAny<IncomeRequest>()), Times.AtLeastOnce());
        //}
        //[Fact]
        //public async void GetByAmount()
        //{
        //    var request = new AmountSearchRequest() { Query = 1.00m };
        //    var result = await _sut.GetIncomesByAmount(request);
        //    var viewResult = Assert.IsType<ActionResult<IncomeResponse>>(result);
        //    _service.Verify(s => s.GetIncomesByAmountAsync(It.IsAny<AmountSearchRequest>()), Times.AtLeastOnce());
        //}
        //[Fact]
        //public async void Create()
        //{
        //    var request = new AddEditIncome();
        //    var result = await _sut.CreateIncome(request);
        //    var viewResult = Assert.IsType<ActionResult<AddEditIncome>>(result);
        //    _service.Verify(s => s.CreateIncomeAsync(It.IsAny<AddEditIncome>()), Times.AtLeastOnce());
        //}
        //[Fact]
        //public async void Update()
        //{
        //    var request = new AddEditIncome() { Id = 99999 };
        //    var result = await _sut.UpdateIncome(request);
        //    var viewResult = Assert.IsType<ActionResult<AddEditIncome>>(result);
        //    _service.Verify(s => s.UpdateIncomeAsync(It.IsAny<AddEditIncome>()), Times.AtLeastOnce());
        //}
        //[Fact]
        //public async void Delete()
        //{
        //    var result = await _sut.DeleteIncome(99999);
        //    _service.Verify(s => s.DeleteIncomeAsync(It.IsAny<int>()), Times.AtLeastOnce());
        //}
    }
}
