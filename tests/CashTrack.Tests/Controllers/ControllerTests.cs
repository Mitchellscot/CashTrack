using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using CashTrack.Tests.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CashTrack.Tests.Web;

public class ControllerTests : IClassFixture<CustomWebApplicationFactory<CashTrack.Program>>
{
    private readonly CustomWebApplicationFactory<CashTrack.Program> _factory;
    private ITestOutputHelper _output;

    public ControllerTests(CustomWebApplicationFactory<CashTrack.Program> factory, ITestOutputHelper output)
    {
        _output = output;
        _factory = factory;
    }
    [Theory]
    [InlineData("/Expenses")]
    [InlineData("/Expenses/Categories")]
    [InlineData("/Merchants")]
    [InlineData("/Merchants/Detail/1")]
    [InlineData("/Expenses/Categories/Detail/1")]
    [InlineData("/Expenses/Categories/Main")]
    [InlineData("/Expenses/Split/1")]
    [InlineData("/Income")]
    [InlineData("/Sources/Detail/1")]
    [InlineData("/Income/Refund/1")]
    [InlineData("/Settings")]
    public async Task Can_Return_Secure_Pages(string path)
    {
        var client = GetAuthenticatedClient();

        var response = await client.GetAsync(path);

        var html = await response.Content.ReadAsStringAsync();
        response.Content.Headers.ContentType!.ToString().ShouldBe("text/html; charset=utf-8");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        PrintRequestAndResponse(path, html);

    }
    private HttpClient GetAuthenticatedClient()
    {
        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                        "Test", options => { });
            });
        }).CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
        return client;
    }
    private void PrintRequestAndResponse(object request, object response)
    {
        _output.WriteLine(request.ToString());
        _output.WriteLine(response.ToString());
    }
}