//using System;
//using System.IO;
//using System.Net.Http;
//using CashTrack.Data;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.TestHost;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Bogus;
//using System.Text;
//using System.Threading.Tasks;
//using Xunit.Abstractions;
//using CashTrack.Models.AuthenticationModels;
//using System.Net.Http.Headers;
//using CashTrack.IntegrationTests.Common;
//using Microsoft.AspNetCore.Mvc.Testing;

//namespace CashTrack.IntegrationTests
//{
//    public class TestServerFixture : IDisposable
//    {
//        private readonly TestServer _testServer;
//        private readonly TestSettings _settings;
//        public HttpClient Client { get; }
//        public Faker Faker;
//        public string Token;

//        public TestServerFixture()
//        {
//            var application = new WebApplicationFactory<Startup>()
//                .WithWebHostBuilder(builder => { });
//            var client = application.CreateClient();
//            //var configuration = new ConfigurationBuilder()
//            //    .SetBasePath(Directory.GetCurrentDirectory())
//            //    .AddJsonFile("appsettings.Test.json")
//            //    .Build();
//            //var builder = new WebHostBuilder()
//            //    .UseContentRoot(GetContentRootPath())
//            //    .UseEnvironment("Test")
//            //    .UseConfiguration(configuration)
//            //    .UseStartup<CashTrack.Startup>();
//            //_testServer = new TestServer(builder);
//            //Client = _testServer.CreateClient();
//            //Faker = new Faker();
//            //_settings = configuration.GetSection("TestSettings").Get<TestSettings>();
//            //Token = GetTokenForAuthenticatedRoutes(_settings.Username, _settings.Password).Result;
//            //Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

//        }

//        public void Dispose()
//        {
//            Client.Dispose();
//            _testServer.Dispose();
//        }
//    }
//}
