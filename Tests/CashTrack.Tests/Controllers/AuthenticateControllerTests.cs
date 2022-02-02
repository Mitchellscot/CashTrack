using CashTrack.Controllers;
using CashTrack.Models.AuthenticationModels;
using CashTrack.Services.AuthenticationService;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace CashTrack.Tests.Controllers
{
    public class AuthenticateControllerTests
    {
        private readonly CashTrack.Controllers.AuthenticateController _sut;
        private readonly Mock<IAuthenticationService> _service;
        public AuthenticateControllerTests()
        {
            _service = new Mock<IAuthenticationService>();
            _sut = new CashTrack.Controllers.AuthenticateController(_service.Object);
        }
        [Fact]
        public async Task GetAuthenticated()
        {
            var request = new AuthenticationModels.Request("mitch", "password");
            var result = await _sut.Authenticate(request);
            var viewResult = Assert.IsType<ActionResult<AuthenticationModels.Response>>(result);
            _service.Verify(s => s.AuthenticateAsync(It.IsAny<AuthenticationModels.Request>()), Times.AtLeastOnce());
        }
    }
}
