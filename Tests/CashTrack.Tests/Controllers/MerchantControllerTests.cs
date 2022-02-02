using CashTrack.Controllers;
using CashTrack.Models.MerchantModels;
using CashTrack.Services.MerchantService;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CashTrack.Tests.Controllers
{
    public class MerchantControllerTests
    {
        private readonly Mock<IMerchantService> _service;
        private readonly MerchantsController _sut;

        public MerchantControllerTests()
        {
            _service = new Mock<IMerchantService>();
            _sut = new MerchantsController(_service.Object);
        }
        [Fact]
        public async void GetAll()
        {
            var request = new MerchantRequest();
            var result = await _sut.GetAllMerchants(request);
            var viewResult = Assert.IsType<ActionResult<MerchantResponse>>(result);
            _service.Verify(s => s.GetMerchantsAsync(It.IsAny<MerchantRequest>()), Times.AtLeastOnce());
        }
        [Fact]
        public async void GetById()
        {
            var result = await _sut.GetMerchantDetail(1);
            var viewResult = Assert.IsType<ActionResult<MerchantDetail>>(result);
            _service.Verify(s => s.GetMerchantDetailAsync(It.IsAny<int>()), Times.AtLeastOnce());
        }
        [Fact]
        public async void Create()
        {
            var request = new AddEditMerchant();
            var result = await _sut.CreateMerchant(request);
            var viewResult = Assert.IsType<ActionResult<AddEditMerchant>>(result);
            _service.Verify(s => s.CreateMerchantAsync(It.IsAny<AddEditMerchant>()), Times.AtLeastOnce());
        }
        [Fact]
        public async void Update()
        {
            var request = new AddEditMerchant() with { Id = int.MaxValue };
            var result = await _sut.UpdateMerchant(request);
            _service.Verify(s => s.UpdateMerchantAsync(It.IsAny<AddEditMerchant>()), Times.AtLeastOnce());
        }
        [Fact]
        public async void Delete()
        {
            var result = await _sut.DeleteMerchant(int.MaxValue);
            _service.Verify(s => s.DeleteMerchantAsync(It.IsAny<int>()), Times.AtLeastOnce());
        }
    }
}
