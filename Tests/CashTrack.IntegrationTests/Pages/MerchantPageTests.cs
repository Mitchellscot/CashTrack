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
    public class MerchantPageTests : IClassFixture<AuthenticatedWebApplicationFactory<CashTrack.Program>>
    {
        private readonly AuthenticatedWebApplicationFactory<CashTrack.Program> _factory;
        private readonly HttpClient _client;
        private ITestOutputHelper _output;
        private string _endpoint;
        public MerchantPageTests(AuthenticatedWebApplicationFactory<CashTrack.Program> factory, ITestOutputHelper output)
        {
            _output = output;
            _factory = factory;
            _client = factory._client;
            _endpoint = "/Merchants";
        }
        [Fact]
        public async Task Can_View_Merchant_Table()
        {
            var merchantPage = await _client.GetAsync(_endpoint);
            var merchantPageresult = await merchantPage.Content.ReadAsStringAsync();
            var merchantPageContent = await HtmlHelpers.GetDocumentAsync(merchantPage);
            merchantPageContent.QuerySelector<IHtmlSpanElement>("#totalCount")!.TextContent.ShouldContain("20 of 486");
            PrintRequestAndResponse(_endpoint, merchantPageresult);
            merchantPage.EnsureSuccessStatusCode();
        }
        [Theory]
        [InlineData("0")]
        [InlineData("1")]
        [InlineData("2")]
        [InlineData("3")]
        [InlineData("4")]
        [InlineData("5")]
        public async Task Can_Reorder_Merchant_Table(string queryValue)
        {
            var query = $"{_endpoint}?Query={queryValue}&Q2=true";
            var merchantPage = await _client.GetAsync(query);
            var merchantPageresult = await merchantPage.Content.ReadAsStringAsync();
            var merchantPageContent = await HtmlHelpers.GetDocumentAsync(merchantPage);
            merchantPageContent.QuerySelector<IHtmlSpanElement>("#totalCount")!.TextContent.ShouldContain("20 of 486");
            PrintRequestAndResponse(query, merchantPageresult);
            merchantPage.EnsureSuccessStatusCode();
        }
        [Fact]
        public async Task Can_Search_Merchant_and_Redirect_To_Detail()
        {
            var merchantPage = await _client.GetAsync($"{_endpoint}?searchTerm=Costco");
            var merchantPageResult = await merchantPage.Content.ReadAsStringAsync();
            PrintRequestAndResponse($"{_endpoint}?searchTerm=Costco", merchantPageResult);
            merchantPage!.RequestMessage!.RequestUri!.AbsolutePath.ShouldBe("/Merchants/Detail/85");
            merchantPage.EnsureSuccessStatusCode();
        }

        private void PrintRequestAndResponse(object request, object response)
        {
            _output.WriteLine(request.ToString());
            _output.WriteLine(response.ToString());
        }
    }
}
