using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using CashTrack.IntegrationTests.Common;
using CashTrack.IntegrationTests.Pages.Common;
using Shouldly;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CashTrack.IntegrationTests.Pages
{
    public class SplitPageTests : IClassFixture<AuthenticatedWebApplicationFactory<CashTrack.Program>>
    {
        private readonly AuthenticatedWebApplicationFactory<CashTrack.Program> _factory;
        private readonly HttpClient _client;
        private readonly TestSettings _settings;
        private ITestOutputHelper _output;
        private string _endpoint;
        public SplitPageTests(AuthenticatedWebApplicationFactory<CashTrack.Program> factory, ITestOutputHelper output)
        {
            _settings = factory._settings;
            _output = output;
            _factory = factory;
            _client = factory._client;
            _endpoint = "/Expenses/Split/";
        }
        [Fact]
        public async Task Can_View_Split_Table()
        {
            Random rnd = new Random();
            var randomNumber = rnd.Next(1, 7000);
            var splitPage = await _client.GetAsync(_endpoint + $"{randomNumber}");
            var splitPageresult = await splitPage.Content.ReadAsStringAsync();
            var splitPageContent = await HtmlHelpers.GetDocumentAsync(splitPage);
            var totalAmount = double.Parse(splitPageContent.QuerySelector<IHtmlSpanElement>("#total")!.TextContent);
            totalAmount.ShouldBeGreaterThan(0.01);
            PrintRequestAndResponse(_endpoint + $"/{randomNumber}", splitPageresult);
            splitPage.EnsureSuccessStatusCode();
        }
        [Theory]
        [InlineData("2")]
        [InlineData("3")]
        [InlineData("4")]
        [InlineData("5")]
        [InlineData("6")]
        [InlineData("7")]
        [InlineData("8")]
        public async Task Can_Add_Splits_And_Change_Taxes(string splitNumber)
        {
            Random rnd = new Random();
            var randomNumber = rnd.Next(1, 7000);
            var randomTax = rnd.Next(1, 10);
            var query = _endpoint + $"{randomNumber}?Split={splitNumber}&Tax={randomTax}";
            var splitPage = await _client.GetAsync(query);
            var splitPageresult = await splitPage.Content.ReadAsStringAsync();
            var splitPageContent = await HtmlHelpers.GetDocumentAsync(splitPage);
            var taxAmount = double.Parse(splitPageContent.QuerySelector<IHtmlInputElement>("#Tax")!.Value);
            var totalAmount = double.Parse(splitPageContent.QuerySelector<IHtmlSpanElement>("#total")!.TextContent);
            totalAmount.ShouldBeGreaterThan(0.01);
            taxAmount.ShouldBe(randomTax);
            PrintRequestAndResponse(query, splitPageresult);
            splitPage.EnsureSuccessStatusCode();
        }
        private void PrintRequestAndResponse(object request, object response)
        {
            _output.WriteLine(request.ToString());
            _output.WriteLine(response.ToString());
        }
    }
}
