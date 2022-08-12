using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Io;
using CashTrack.Data;
using CashTrack.IntegrationTests.Common;
using CsvHelper;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using System.Linq;

namespace CashTrack.IntegrationTests.Pages.IndexTests;

public class IndexTests
    : IClassFixture<CustomWebApplicationFactory<CashTrack.Program>>
{
    private readonly CustomWebApplicationFactory<CashTrack.Program> _factory;
    private ITestOutputHelper _output;

    public IndexTests(CustomWebApplicationFactory<CashTrack.Program> factory, ITestOutputHelper output)
    {
        _output = output;
        _factory = factory;
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/Expenses")]
    [InlineData("/Income")]
    [InlineData("/Merchants")]
    [InlineData("/Sources")]

    public async Task Redirect_All_Endpoints_When_Not_Authenticated(string url)
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync(url);

        response.StatusCode.ShouldBe(HttpStatusCode.Found);
        var result = await response.Content.ReadAsStringAsync();
        Assert.Equal("/login", response!.Headers!.Location!.AbsolutePath);
        PrintRequestAndResponse(url, result);
    }
    [Fact]
    public async Task Can_Log_In()
    {
        var client = _factory.CreateClient();
        // get with HttpClient
        var defaultPage = await client.GetAsync("/");
        //parse into a response AngleSharp understands
        var content = await HtmlHelpers.GetDocumentAsync(defaultPage);

        var passWord = "8043";
        var name = "Mitch";
        //grab the form and the submit button with anglesharps JS looking syntax
        var form = content.QuerySelector<IHtmlFormElement>("#loginForm");
        var button = content.QuerySelector<IHtmlButtonElement>("#loginButton");

        //You can tackle this two ways:
        //1. Get the inputs and assign them values (commented out code below)
        //2. Add a KeyValuePair List with the form values
        //See HttpClientExtensions for the overloads

        //var nameInput = content.QuerySelector<IHtmlInputElement>("input[name='UserName']");
        //var passwordInput = content.QuerySelector<IHtmlInputElement>("input[name='Password']");
        //nameInput!.Value = name;
        //passwordInput!.Value = passWord;

        var response = await client.SendAsync((IHtmlFormElement)form!, (IHtmlButtonElement)button!, new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("UserName", name),
            new KeyValuePair<string, string>("Password", passWord)
        });

        var result = await response.Content.ReadAsStringAsync();
        //this might change in the future.
        result.ShouldContain($"Welcome {name}");

        defaultPage.EnsureSuccessStatusCode();
        response.EnsureSuccessStatusCode();

    }
    [Fact]
    public async Task Can_Log_In_To_Secure_Page()
    {
        var client = _factory.CreateClient();

        var defaultPage = await client.GetAsync("/");
        var content = await HtmlHelpers.GetDocumentAsync(defaultPage);

        var passWord = "8043";
        var name = "Mitch";
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

        var expensePage = await client.GetAsync("/Expenses");
        var expensePageresult = await expensePage.Content.ReadAsStringAsync();
        var expensePageContent = await HtmlHelpers.GetDocumentAsync(expensePage);

        expensePage.EnsureSuccessStatusCode();

        // newClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("set-cookie", response.Headers);

    }
    private void PrintRequestAndResponse(object request, object response)
    {
        _output.WriteLine(request.ToString());
        _output.WriteLine(response.ToString());
    }

}