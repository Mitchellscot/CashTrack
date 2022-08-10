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
        public async void Should_GetMatchingMerchants()
        {
            var result = await _sut.GetMatchingMerchants("Costco");
            var viewResult = Assert.IsType<ActionResult<MerchantResponse>>(result);
            _service.Verify(s => s.GetMerchantsAsync(It.IsAny<MerchantRequest>()), Times.AtLeastOnce());
        }
    }
}
