using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AutoMapper.Configuration.Conventions;
using Bogus.DataSets;
using CashTrack.IntegrationTests.Common;
using CashTrack.IntegrationTests.Pages.Common;
using CsvHelper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Shouldly;
using System;
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
            _client = factory._client;
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
        public async Task Can_Query_Expenses(string query)
        {
            var expensePage = await _client.GetAsync($"/Expenses?query={query}");
            expensePage.EnsureSuccessStatusCode();
        }
        [Fact]
        public async Task Can_Query_Expenses_Between_Two_Dates()
        {
            var expensePage = await _client.GetAsync($"/Expenses?query=1&q=2020-01-01&q2=2021-12-31&pageNumber=1");
            expensePage.EnsureSuccessStatusCode();
        }
        //Anglesharp doesn't like select lists, so I can't run tests on select list form values...
        //[Fact]
        //public async Task Can_Add_New_Expense()
        //{
        //    var expensePage = await _client.GetAsync($"/Expenses");
        //    var content = await HtmlHelpers.GetDocumentAsync(expensePage);
        //    var form = content.QuerySelector<IHtmlFormElement>("#addExpenseForm");
        //    var button = content.QuerySelector<IHtmlButtonElement>("#addExpenseButton");
        //    var response = await _client.SendAsync((IHtmlFormElement)form!, (IHtmlButtonElement)button!, new List<KeyValuePair<string, string>>
        //    {
        //    new KeyValuePair<string, string>("Date", DateTime.Now.ToShortDateString()),
        //    new KeyValuePair<string, string>("Amount", "10"),
        //    new KeyValuePair<string, string>("SubCategoryId", "1"),
        //    new KeyValuePair<string, string>("Merchant", "Costco"),
        //});
        //    response.EnsureSuccessStatusCode();
        //}

        private void PrintRequestAndResponse(object request, object response)
        {
            _output.WriteLine(request.ToString());
            _output.WriteLine(response.ToString());
        }
    }
}
