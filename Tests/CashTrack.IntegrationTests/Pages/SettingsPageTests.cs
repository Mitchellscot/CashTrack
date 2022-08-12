using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using CashTrack.IntegrationTests.Pages.Common;

namespace CashTrack.IntegrationTests.Pages;

public class SettingsTests
    : IClassFixture<AuthenticatedWebApplicationFactory<CashTrack.Program>>
{
    private readonly AuthenticatedWebApplicationFactory<CashTrack.Program> _factory;
    private readonly HttpClient _client;
    private ITestOutputHelper _output;
    private string _endpoint;

    public SettingsTests(AuthenticatedWebApplicationFactory<CashTrack.Program> factory, ITestOutputHelper output)
    {
        _output = output;
        _factory = factory;
        _endpoint = "/Settings";
        _client = factory._client;
    }
    [Fact]
    public async Task Can_View_Settings()
    {
        var sourcesPage = await _client.GetAsync(_endpoint);
        var sourcesPageresult = await sourcesPage.Content.ReadAsStringAsync();
        PrintRequestAndResponse(_endpoint, sourcesPageresult);
        sourcesPage.EnsureSuccessStatusCode();
    }
    private void PrintRequestAndResponse(object request, object response)
    {
        _output.WriteLine(request.ToString());
        _output.WriteLine(response.ToString());
    }
}