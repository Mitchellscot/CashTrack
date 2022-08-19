using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using CashTrack.Tests.Common;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CashTrack.Tests.Web;

public class HomePageTests : IClassFixture<CustomWebApplicationFactory<CashTrack.Program>>
{
    private readonly CustomWebApplicationFactory<CashTrack.Program> _factory;
    private ITestOutputHelper _output;

    public HomePageTests(CustomWebApplicationFactory<CashTrack.Program> factory, ITestOutputHelper output)
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
            new KeyValuePair<string, string>("LoginRequest.UserName", "Test"),
            new KeyValuePair<string, string>("LoginRequest.Password", "0f1fe927-6221-4b44-bdb5-233a81748de1")
        });

        var result = await response.Content.ReadAsStringAsync();
        //this might change in the future.
        result.ShouldContain($"Welcome ");

        defaultPage.EnsureSuccessStatusCode();
        response.EnsureSuccessStatusCode();

    }
    private void PrintRequestAndResponse(object request, object response)
    {
        _output.WriteLine(request.ToString());
        _output.WriteLine(response.ToString());
    }
}