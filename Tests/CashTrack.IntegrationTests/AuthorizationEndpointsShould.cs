using Xunit;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shouldly;
using Xunit.Abstractions;
using System.Net;
using CashTrack.IntegrationTests.Common;
using CashTrack.Models.AuthenticationModels;
using Microsoft.Extensions.Configuration;

namespace CashTrack.IntegrationTests
{
    public class AuthorizationEndpointsShould : IClassFixture<TestServerFixture>
    {
        private TestServerFixture _fixture;
        private readonly TestOptionsSnapshot<TestSettings> _testSettings;
        private ITestOutputHelper _output;
        private const string ENDPOINT = "/api/authenticate";

        public AuthorizationEndpointsShould(TestServerFixture fixture, ITestOutputHelper output)
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.test.json")
                .Build();
            var settings = new TestSettings();
            config.GetSection("TestSettings").Bind(settings);
            _output = output;
            _fixture = fixture;
            _testSettings = new TestOptionsSnapshot<TestSettings>(settings);
        }

        [Fact]
        public async Task ReturnAnAuthenticatedUser()
        {

            var request = GetAuthenticationRequest();
            var response = await _fixture.SendPostRequestAsync(ENDPOINT, request);

            var responseBody = JsonConvert.DeserializeObject<AuthenticationModels.Response>(await response.Content.ReadAsStringAsync());

            response.EnsureSuccessStatusCode();
            responseBody.Token.ShouldNotBeEmpty();
            responseBody.FirstName.ShouldBe(request.Name);
            PrintRequestAndResponse(request, responseBody);
        }

        [Theory]
        [InlineData("Password")]
        [InlineData("Chewbaca")]
        [InlineData("123456789")]
        public async Task ReturnUnauthorizedWithWrongPassword(string password)
        {
            var request = GetAuthenticationRequest() with { Password = password };
            var response = await _fixture.SendPostRequestAsync(ENDPOINT, request);
            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
            PrintRequestAndResponse(request,
                JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync()));
        }

        [Theory]
        [InlineData("Henry")]
        [InlineData("Lydia")]
        [InlineData("Edward")]
        [InlineData("Arthur")]
        public async Task ReturnUnauthorizedWithWrongUserName(string username)
        {
            var request = GetAuthenticationRequest() with { Name = username };
            var response = await _fixture.SendPostRequestAsync(ENDPOINT, request);
            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
            PrintRequestAndResponse(request,
                JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync()));
        }

        [Theory]
        [EmptyData]
        public async Task ReturnBadRequestWithEmptyPassword(string password)
        {
            var request = GetAuthenticationRequest() with { Password = password };
            var response = await _fixture.SendPostRequestAsync(ENDPOINT, request);
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            PrintRequestAndResponse(request,
                JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync()));
        }

        [Theory]
        [EmptyData]
        public async Task ReturnBadRequestWithEmptyUsername(string username)
        {
            var request = GetAuthenticationRequest() with { Name = username };
            var response = await _fixture.SendPostRequestAsync(ENDPOINT, request);
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            PrintRequestAndResponse(request,
                JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync()));
        }

        private AuthenticationModels.Request GetAuthenticationRequest()
        {
            return new AuthenticationModels.Request(_testSettings.Value.Username, _testSettings.Value.Password);
        }

        private void PrintRequestAndResponse(object request, object response)
        {
            _output.WriteLine(request.ToString());
            _output.WriteLine(response.ToString());
        }
    }
}