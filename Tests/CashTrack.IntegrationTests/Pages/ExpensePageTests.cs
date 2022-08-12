using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using CashTrack.IntegrationTests.Common;
using CashTrack.IntegrationTests.Pages.Common;
using CsvHelper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CashTrack.IntegrationTests.Pages
{
    public class ExpensePageTests : IClassFixture<AuthenticatedWebApplicationFactory<CashTrack.Program>>
    {
        private readonly AuthenticatedWebApplicationFactory<CashTrack.Program> _factory;
        private readonly HttpClient _client;
        private readonly TestSettings _settings;
        private ITestOutputHelper _output;
        public ExpensePageTests(AuthenticatedWebApplicationFactory<CashTrack.Program> factory, ITestOutputHelper output)
        {
            _settings = factory._settings;
            _output = output;
            _factory = factory;
            _client = GetAuthenticated(_factory).Result;
        }
        [Fact]
        public async Task Can_View_Expense_Table()
        {
            var expensePage = await _client.GetAsync("/Expenses");
            var expensePageresult = await expensePage.Content.ReadAsStringAsync();
            var expensePageContent = await HtmlHelpers.GetDocumentAsync(expensePage);
            expensePageContent.QuerySelector<IHtmlSpanElement>("#totalPages")!.TextContent.ShouldContain("20 of 7277");
            expensePageContent.QuerySelector("#totalAmount")!.InnerHtml.ShouldContain("474241.90");
            PrintRequestAndResponse("/Expenses", expensePageresult);
            expensePage.EnsureSuccessStatusCode();
        }
        [Fact]
        public async Task Can_Query_Expenses()
        { 
            
            for (int i = 0; i < 15; i++)
            {
                var expensePage = await _client.GetAsync($"/Expenses?query={i}");
                expensePage.EnsureSuccessStatusCode();
            }
            
        }
        private async Task<HttpClient> GetAuthenticated(AuthenticatedWebApplicationFactory<Program> app)
        {
            var client = app.CreateClient();
            var defaultPage = await client.GetAsync("/");
            var content = await HtmlHelpers.GetDocumentAsync(defaultPage);

            var passWord = _settings.Password;
            var name = _settings.Username;
            //grab the form and the submit button with anglesharps JS looking syntax
            var form = content.QuerySelector<IHtmlFormElement>("#loginForm");
            var button = content.QuerySelector<IHtmlButtonElement>("#loginButton");

            var response = await client.SendAsync((IHtmlFormElement)form!, (IHtmlButtonElement)button!, new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("UserName", name),
            new KeyValuePair<string, string>("Password", passWord)
        });

            var result = await response.Content.ReadAsStringAsync();
            result.ShouldContain($"Welcome {name}");

            defaultPage.EnsureSuccessStatusCode();
            response.EnsureSuccessStatusCode();
            return client;
        }
        private void PrintRequestAndResponse(object request, object response)
        {
            _output.WriteLine(request.ToString());
            _output.WriteLine(response.ToString());
        }
    }
}
