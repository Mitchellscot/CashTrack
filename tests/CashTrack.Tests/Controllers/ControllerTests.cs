using CashTrack.Models.IncomeCategoryModels;
using CashTrack.Models.IncomeSourceModels;
using CashTrack.Models.MainCategoryModels;
using CashTrack.Models.MerchantModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Tests.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Shouldly;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CashTrack.Tests.Controllers;

public class ControllerTests : IClassFixture<CustomWebApplicationFactory<CashTrack.Program>>
{
    private readonly CustomWebApplicationFactory<CashTrack.Program> _factory;
    private readonly HttpClient _client;
    private ITestOutputHelper _output;

    public ControllerTests(CustomWebApplicationFactory<CashTrack.Program> factory, ITestOutputHelper output)
    {
        _output = output;
        _factory = factory;
        _client = GetAuthenticatedClient();
    }
    [Fact]
    public async Task Income_Category_Controller_Returns_Categories()
    {
        var response = await _client.GetAsync("/api/IncomeCategory");

        var result = await response.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeObject<IncomeCategoryDropdownSelection[]>(result);
        responseObject.Length.ShouldBe(10);
        responseObject.Last().Category.ShouldBe("Bonus");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        PrintRequestAndResponse("/api/IncomeCategory", result);
    }
    [Fact]
    public async Task Sub_Category_Controller_Returns_Categories()
    {
        var response = await _client.GetAsync("/api/SubCategory");

        var result = await response.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeObject<SubCategoryDropdownSelection[]>(result);
        responseObject.Length.ShouldBe(31);
        responseObject.Last().Category.ShouldBe("Travel Misc");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        PrintRequestAndResponse("/api/SubCategory", result);
    }
    [Fact]
    public async Task Merchant_Controller_Returns_Merchants_For_Dropdown()
    {
        var response = await _client.GetAsync("/api/merchants/dropdown");

        var result = await response.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeObject<MerchantDropdownSelection[]>(result);
        responseObject.Length.ShouldBe(16);
        responseObject.Last().Name.ShouldBe("Torch of India");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        PrintRequestAndResponse("/api/Merchants/dropdown", result);
    }
    [Fact]
    public async Task Source_Controller_Returns_Sources_For_Dropdown()
    {
        var response = await _client.GetAsync("/api/incomesource/dropdown");

        var result = await response.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeObject<SourceDropdownSelection[]>(result);
        responseObject.Length.ShouldBe(14);
        responseObject.Last().Name.ShouldBe("Hessel Church");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        PrintRequestAndResponse("/api/incomesource/dropdown", result);
    }
    [Fact]
    public async Task Main_Category_Controller_Returns_Categories()
    {
        var response = await _client.GetAsync("/api/MainCategory");

        var result = await response.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeObject<MainCategoryDropdownSelection[]>(result);
        responseObject.Length.ShouldBe(16);
        responseObject.Last().Category.ShouldBe("Vacation");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        PrintRequestAndResponse("/api/MainCategory", result);
    }
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]

    public async Task Main_Category_Controller_Returns_By_Subcategory_Id(int id)
    {
        var response = await _client.GetAsync($"/api/maincategory/sub-category/{id}");

        var result = await response.Content.ReadAsStringAsync();
        result.ShouldNotBeEmpty();
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        PrintRequestAndResponse("/api/maincategory/sub-category/", result);
    }
    [Theory]
    [InlineData("BZT Tips")]
    [InlineData("Alliance Redwoods")]
    [InlineData("SCT Tips")]

    public async Task Income_Source_Controller_Returns_Matching_Sources(string name)
    {
        var response = await _client.GetAsync($"/api/incomesource?sourceName={name}");

        var result = await response.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeObject<string[]>(result);
        responseObject[0].ShouldBe(name);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        PrintRequestAndResponse("/api/IncomeSource?sourceName=", result);
    }
    [Theory]
    [InlineData("Groceries")]
    [InlineData("Hobbies")]
    [InlineData("Rent")]

    public async Task Subcategory_Controller_Returns_Matching_Categories(string name)
    {
        var response = await _client.GetAsync($"/api/subcategory/autocomplete?categoryName={name}");

        var result = await response.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeObject<string[]>(result);
        responseObject[0].ShouldBe(name);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        PrintRequestAndResponse("/api/subcategory/autocomplete?categoryName=", result);
    }
    [Theory]
    [InlineData("Paycheck")]
    [InlineData("Tip")]
    [InlineData("Gift")]
    public async Task Incomecategory_Controller_Returns_Matching_Categories(string name)
    {
        var response = await _client.GetAsync($"/api/incomecategory/autocomplete?categoryName={name}");

        var result = await response.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeObject<string[]>(result);
        responseObject[0].ShouldBe(name);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        PrintRequestAndResponse("/api/incomecategory/autocomplete?categoryName=", result);
    }
    [Theory]
    [InlineData("Costco")]
    [InlineData("In N Out")]
    [InlineData("Target")]

    public async Task Merchants_Controller_Returns_Matching_Merchants(string name)
    {
        var response = await _client.GetAsync($"/api/merchants?merchantName={name}");

        var result = await response.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeObject<string[]>(result);
        responseObject.Length.ShouldBe(1);
        responseObject[0].ShouldContain(name);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        PrintRequestAndResponse("/api/merchants?merchantName=", result);
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