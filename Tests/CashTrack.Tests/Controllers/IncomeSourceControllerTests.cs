//using CashTrack.Controllers;
//using CashTrack.Models.IncomeSourceModels;
//using CashTrack.Services.IncomeSourceService;
//using Microsoft.AspNetCore.Mvc;
//using Moq;
//using Xunit;

//namespace CashTrack.Tests.Controllers
//{
//    public class IncomeSourceControllerTests
//    {
//        private readonly Mock<IIncomeSourceService> _service;
//        private readonly IncomeSourceController _sut;

//        public IncomeSourceControllerTests()
//        {
//            _service = new Mock<IIncomeSourceService>();
//            _sut = new IncomeSourceController(_service.Object);
//        }
//        [Fact]
//        public async void GetAll()
//        {
//            var result = await _sut.GetIncomeSources();
//            var viewResult = Assert.IsType<ActionResult<IncomeSourceResponse>>(result);
//            _service.Verify(s => s.GetIncomeSourcesAsync(It.IsAny<IncomeSourceRequest>()), Times.AtLeastOnce());
//        }
//    }
//}
