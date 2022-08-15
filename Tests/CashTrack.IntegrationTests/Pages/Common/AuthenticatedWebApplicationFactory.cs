using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Bogus;
using CashTrack.Data;
using CashTrack.Data.Entities;
using CashTrack.IntegrationTests.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace CashTrack.IntegrationTests.Pages.Common
{
    public class AuthenticatedWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        public Faker _faker;
        public readonly HttpClient _client;

        public AuthenticatedWebApplicationFactory()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Test.json")
                .Build();
            _faker = new Faker();
            _client = GetAuthenticated(this.CreateClient()).Result;
        }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                services.Remove(descriptor!);
                var connection = new SqliteConnection("Filename=:memory:");
                connection.Open();
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseSqlite(connection);
                });
                services.AddHeaderPropagation(options =>
                {
                    options.Headers.Add("Cookie", context =>
                    {
                        var accessToken = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "access-token")?.Value;
                        return accessToken != null ? new StringValues($"token={accessToken}") : new StringValues();
                    });
                });

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<AppDbContext>();
                    var logger = scopedServices
                        .GetRequiredService<ILogger<AuthenticatedWebApplicationFactory<TStartup>>>();

                    db.Database.EnsureCreated();
                }
            });
        }
        private async Task<HttpClient> GetAuthenticated(HttpClient client)
        {
            var defaultPage = await client.GetAsync("/");
            var content = await HtmlHelpers.GetDocumentAsync(defaultPage);

            //grab the form and the submit button with anglesharps JS looking syntax
            var form = content.QuerySelector<IHtmlFormElement>("#loginForm");
            var button = content.QuerySelector<IHtmlButtonElement>("#loginButton");

            var response = await client.SendAsync((IHtmlFormElement)form!, (IHtmlButtonElement)button!, new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("UserName", "Test"),
            new KeyValuePair<string, string>("Password", "0f1fe927-6221-4b44-bdb5-233a81748de1")
        });
            return client;
        }
    }
}