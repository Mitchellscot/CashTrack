using System;
using System.IO;
using System.Net.Http;
using CashTrack.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Bogus;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using CashTrack.Models.AuthenticationModels;
using System.Net.Http.Headers;
using CashTrack.IntegrationTests.Common;

namespace CashTrack.IntegrationTests
{
    public class TestServerFixture : IDisposable
    {
        private readonly TestServer _testServer;
        private readonly TestSettings _settings;
        public HttpClient Client { get; }
        public Faker Faker;
        public string Token;

        public TestServerFixture()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Test.json")
                .Build();
            var builder = new WebHostBuilder()
                .UseContentRoot(GetContentRootPath())
                .UseEnvironment("Test")
                .UseConfiguration(configuration)
                .UseStartup<CashTrack.Startup>();
            _testServer = new TestServer(builder);
            Client = _testServer.CreateClient();
            Faker = new Faker();
            _settings = configuration.GetSection("TestSettings").Get<TestSettings>();
            Token = GetTokenForAuthenticatedRoutes(_settings.Username, _settings.Password).Result;
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

        }

        private async Task<string> GetTokenForAuthenticatedRoutes(string user, string password)
        {
            var request = new AuthenticationModels.Request(user, password);
            var response = await SendPostRequestAsync("/api/authenticate", request);
            #if DEBUG
            var READTHISIFYOUAREGETTINGBUGS = await response.Content.ReadAsStringAsync();
            #endif
            return JsonConvert.DeserializeObject<AuthenticationModels.Response>(await response.Content.ReadAsStringAsync()).Token;

        }

        private string GetContentRootPath()
        {
            string testProjectPath = PlatformServices.Default.Application.ApplicationBasePath;
            var relativePathToWebProject = @"..\..\..\..\..\src";
            return Path.Combine(testProjectPath, relativePathToWebProject);
        }

        public async Task<HttpResponseMessage> SendPostRequestAsync(string path, object request)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = new StringContent(
                    JsonConvert.SerializeObject(request),
                    Encoding.UTF8,
                    "application/json")
            };
            return await Client.SendAsync(message);
        }
        public async Task<HttpResponseMessage> SendPutRequestAsync(string path, object request)
        {
            var message = new HttpRequestMessage(HttpMethod.Put, path)
            {
                Content = new StringContent(
                    JsonConvert.SerializeObject(request),
                    Encoding.UTF8,
                    "application/json")
            };
            return await Client.SendAsync(message);
        }

        public void Dispose()
        {
            Client.Dispose();
            _testServer.Dispose();
        }
    }
}
