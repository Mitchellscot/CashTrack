using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

public class BasicTests
    : IClassFixture<CustomWebApplicationFactory<CashTrack.Program>>
{
    private readonly CustomWebApplicationFactory<CashTrack.Program> _factory;
    private ITestOutputHelper _output;

    public BasicTests(CustomWebApplicationFactory<CashTrack.Program> factory, ITestOutputHelper output)
    {
        _output = output;
        _factory = factory;
    }

    [Theory]
    [InlineData("/")]

    public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        var result = await response.Content.ReadAsStringAsync();
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
        PrintRequestAndResponse("/", result);
    }
    private void PrintRequestAndResponse(object request, object response)
    {
        _output.WriteLine(request.ToString());
        _output.WriteLine(response.ToString());
    }
}