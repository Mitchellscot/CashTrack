using CashTrack.Models.UserModels;
using Newtonsoft.Json;
using Shouldly;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CashTrack.IntegrationTests
{
    public class UserEndpointsShould : IClassFixture<TestServerFixture>
    {
        private readonly TestServerFixture _fixture;
        private readonly ITestOutputHelper _output;
        const string ENDPOINT = "/api/user";

        public UserEndpointsShould(TestServerFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task ReturnASingleUser(int id)
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + $"?id={id}");
            response.EnsureSuccessStatusCode();
            var responseObject = JsonConvert.DeserializeObject<UserModels.Response>(await response.Content.ReadAsStringAsync());
            _output.WriteLine(responseObject.ToString());
            responseObject.id.ShouldBeEquivalentTo(id);
        }
        [Fact]
        public async Task ReturnAllUsers()
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + "/all");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            var userArray = JsonConvert.DeserializeObject<UserModels.Response[]>(responseString);
            foreach (var user in userArray)
            {
                _output.WriteLine(user.ToString());
                user.FirstName.ShouldNotBeEmpty();
            }
        }
        [Theory]
        [InlineData(0)]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        public async Task ErrorWithInvalidId(int invalidId)
        {
            var response = await _fixture.Client.GetAsync(ENDPOINT + $"?={invalidId}");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }
    }
}
