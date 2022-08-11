using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;


namespace CashTrack.IntegrationTests.Pages
{
    public class BasicTests : IClassFixture<WebApplicationFactory<CashTrack.Program>>
    {
        private readonly WebApplicationFactory<CashTrack.Program> _factory;
        private ITestOutputHelper _output;
        public BasicTests(WebApplicationFactory<CashTrack.Program> factory, ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
        }
        [Fact]
        public async Task Index_Returns_OK()
        {

            var client = _factory.CreateClient();
            var response = await client.GetAsync("/");
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains("CashTrack", stringResponse);
            PrintRequestAndResponse("/", stringResponse);
        }
        private void PrintRequestAndResponse(object request, object response)
        {
            _output.WriteLine(request.ToString());
            _output.WriteLine(response.ToString());
        }
    }
}
