using CashTrack.IntegrationTests.Common;
using CashTrack.IntegrationTests.Pages.Common;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CashTrack.IntegrationTests.Pages
{
    public class ImportPageTests : IClassFixture<AuthenticatedWebApplicationFactory<CashTrack.Program>>
    {
        private readonly AuthenticatedWebApplicationFactory<CashTrack.Program> _factory;
        private readonly HttpClient _client;
        private ITestOutputHelper _output;
        private string _expenseEndpoint;
        private string _incomeEndpoint;
        public ImportPageTests(AuthenticatedWebApplicationFactory<CashTrack.Program> factory, ITestOutputHelper output)
        {
            _output = output;
            _factory = factory;
            _client = factory._client;
            _expenseEndpoint = "/Import/Expenses";
            _incomeEndpoint = "/Import/Income";
        }
        [Fact]
        public async Task Can_View_Import_Expense_Table()
        {
            var importPage = await _client.GetAsync(_expenseEndpoint);
            var importPageresult = await importPage.Content.ReadAsStringAsync();
            var importPageContent = await HtmlHelpers.GetDocumentAsync(importPage);
            PrintRequestAndResponse(_expenseEndpoint, importPageresult);
            importPage.EnsureSuccessStatusCode();
        }
        [Fact]
        public async Task Can_View_Import_Income_Table()
        {
            var importPage = await _client.GetAsync(_incomeEndpoint);
            var importPageresult = await importPage.Content.ReadAsStringAsync();
            var importPageContent = await HtmlHelpers.GetDocumentAsync(importPage);
            PrintRequestAndResponse(_incomeEndpoint, importPageresult);
            importPage.EnsureSuccessStatusCode();
        }

        private void PrintRequestAndResponse(object request, object response)
        {
            _output.WriteLine(request.ToString());
            _output.WriteLine(response.ToString());
        }
    }
}
