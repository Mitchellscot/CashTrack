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
    public class RefundPageTests : IClassFixture<AuthenticatedWebApplicationFactory<CashTrack.Program>>
    {
        private readonly AuthenticatedWebApplicationFactory<CashTrack.Program> _factory;
        private readonly HttpClient _client;
        private ITestOutputHelper _output;
        private string _endpoint;
        public RefundPageTests(AuthenticatedWebApplicationFactory<CashTrack.Program> factory, ITestOutputHelper output)
        {
            _output = output;
            _factory = factory;
            _client = factory._client;
            _endpoint = "/Income/Refund/";
        }
        [Fact]
        public async Task Can_View_Refund_Table()
        {
            var knownRefund = 740;
            var refundPage = await _client.GetAsync(_endpoint + $"{knownRefund}");
            var refundPageresult = await refundPage.Content.ReadAsStringAsync();
            var refundPageContent = await HtmlHelpers.GetDocumentAsync(refundPage);
            var totalAmount = double.Parse(refundPageContent.QuerySelector<IHtmlSpanElement>("#total")!.TextContent);
            totalAmount.ShouldBe(32.73);
            PrintRequestAndResponse(_endpoint + $"/{knownRefund}", refundPageresult);
            refundPage.EnsureSuccessStatusCode();
        }
        //this page is complicated I would rather test the service layer
        private void PrintRequestAndResponse(object request, object response)
        {
            _output.WriteLine(request.ToString());
            _output.WriteLine(response.ToString());
        }
    }
}
