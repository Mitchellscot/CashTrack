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
    public class SourcesPageTests : IClassFixture<AuthenticatedWebApplicationFactory<CashTrack.Program>>
    {
        private readonly AuthenticatedWebApplicationFactory<CashTrack.Program> _factory;
        private readonly HttpClient _client;
        private ITestOutputHelper _output;
        private string _endpoint;
        public SourcesPageTests(AuthenticatedWebApplicationFactory<CashTrack.Program> factory, ITestOutputHelper output)
        {
            _output = output;
            _factory = factory;
            _client = factory._client;
            _endpoint = "/Sources";
        }
        [Fact]
        public async Task Can_View_Sources_Table()
        {
            var sourcesPage = await _client.GetAsync(_endpoint);
            var sourcesPageresult = await sourcesPage.Content.ReadAsStringAsync();
            var sourcesPageContent = await HtmlHelpers.GetDocumentAsync(sourcesPage);
            sourcesPageContent.QuerySelector<IHtmlSpanElement>("#totalCount")!.TextContent.ShouldContain("20 of 32");
            PrintRequestAndResponse(_endpoint, sourcesPageresult);
            sourcesPage.EnsureSuccessStatusCode();
        }
        [Fact]
        public async Task Can_Search_Sources_and_Redirect_To_Detail()
        {
            var sourcesPage = await _client.GetAsync($"{_endpoint}?searchTerm=God");
            var sourcesPageResult = await sourcesPage.Content.ReadAsStringAsync();
            PrintRequestAndResponse($"{_endpoint}?searchTerm=God", sourcesPageResult);
            sourcesPage!.RequestMessage!.RequestUri!.AbsolutePath.ShouldBe("/Sources/Detail/32");
            sourcesPage.EnsureSuccessStatusCode();
        }

        private void PrintRequestAndResponse(object request, object response)
        {
            _output.WriteLine(request.ToString());
            _output.WriteLine(response.ToString());
        }
    }
}
