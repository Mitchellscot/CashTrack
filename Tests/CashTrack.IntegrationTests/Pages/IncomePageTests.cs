using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using CashTrack.IntegrationTests.Common;
using CashTrack.IntegrationTests.Pages.Common;
using Shouldly;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CashTrack.IntegrationTests.Pages
{
    public class IncomePageTests : IClassFixture<AuthenticatedWebApplicationFactory<CashTrack.Program>>
    {
        private readonly AuthenticatedWebApplicationFactory<CashTrack.Program> _factory;
        private readonly HttpClient _client;
        private ITestOutputHelper _output;
        private string _endpoint;
        public IncomePageTests(AuthenticatedWebApplicationFactory<CashTrack.Program> factory, ITestOutputHelper output)
        {
            _output = output;
            _factory = factory;
            _client = factory._client;
            _endpoint = "/Income";
        }
        [Fact]
        public async Task Can_View_Income_Table()
        {
            var incomePage = await _client.GetAsync(_endpoint);
            var incomePageresult = await incomePage.Content.ReadAsStringAsync();
            var incomePageContent = await HtmlHelpers.GetDocumentAsync(incomePage);
            incomePageContent.QuerySelector<IHtmlSpanElement>("#totalPages")!.TextContent.ShouldContain("20 of 745");
            incomePageContent.QuerySelector("#totalAmount")!.InnerHtml.ShouldContain("625348.17");
            PrintRequestAndResponse(_endpoint, incomePageresult);
            incomePage.EnsureSuccessStatusCode();
        }
        [Theory]
        [InlineData("0")]
        [InlineData("2")]
        [InlineData("3")]
        [InlineData("4")]
        [InlineData("5")]
        [InlineData("6")]
        [InlineData("7")]
        [InlineData("8")]
        [InlineData("9")]
        [InlineData("10")]
        [InlineData("11")]
        [InlineData("12")]
        [InlineData("13")]
        [InlineData("14")]
        public async Task Can_Query_Income(string queryValue)
        {
            var query = _endpoint + $"?query={queryValue}";
            var incomePage = await _client.GetAsync(query);
            incomePage.EnsureSuccessStatusCode();
        }
        [Fact]
        public async Task Can_Query_Income_Between_Two_Dates()
        {
            var endpoint = _endpoint + $"?query=1&q=2020-01-01&q2=2021-12-31&pageNumber=1";
            var incomePage = await _client.GetAsync(endpoint);
            incomePage.EnsureSuccessStatusCode();
        }
        private void PrintRequestAndResponse(object request, object response)
        {
            _output.WriteLine(request.ToString());
            _output.WriteLine(response.ToString());
        }
    }
}
