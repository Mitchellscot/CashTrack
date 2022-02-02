using CashTrack.Controllers;
using CashTrack.Models.IncomeSourceModels;
using CashTrack.Services.IncomeSourceService;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CashTrack.Tests.Controllers
{
    public class IncomeSourceControllerTests
    {
        private readonly Mock<IIncomeSourceService> _service;
        private readonly IncomeSourceController _sut;

        public IncomeSourceControllerTests()
        {
            _service = new Mock<IIncomeSourceService>();
            _sut = new IncomeSourceController(_service.Object);
        }
        [Fact]
        public async void GetAll()
        {
            var request = new IncomeSourceRequest();
            var result = await _sut.GetIncomeSources(request);
            var viewResult = Assert.IsType<ActionResult<IncomeSourceResponse>>(result);
            _service.Verify(s => s.GetIncomeSourcesAsync(It.IsAny<IncomeSourceRequest>()), Times.AtLeastOnce());
        }
        [Fact]
        public async void Create()
        {
            var request = new AddEditIncomeSource();
            var result = await _sut.CreateIncomeSource(request);
            var viewResult = Assert.IsType<ActionResult<AddEditIncomeSource>>(result);
            _service.Verify(s => s.CreateIncomeSourceAsync(It.IsAny<AddEditIncomeSource>()), Times.AtLeastOnce());
        }
        [Fact]
        public async void Update()
        {
            var request = new AddEditIncomeSource() with { Id = int.MaxValue };
            var result = await _sut.UpdateIncomeSource(request);
            _service.Verify(s => s.UpdateIncomeSourceAsync(It.IsAny<AddEditIncomeSource>()), Times.AtLeastOnce());
        }
        [Fact]
        public async void Delete()
        {
            var result = await _sut.DeleteIncomeSource(int.MaxValue);
            _service.Verify(s => s.DeleteIncomeSourceAsync(It.IsAny<int>()), Times.AtLeastOnce());
        }
    }
}
