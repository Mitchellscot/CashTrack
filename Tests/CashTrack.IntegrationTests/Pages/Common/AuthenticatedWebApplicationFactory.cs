using Bogus;
using CashTrack.Data;
using CashTrack.IntegrationTests.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System.IO;
using System.Linq;

namespace CashTrack.IntegrationTests.Pages.Common
{
    public class AuthenticatedWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        public readonly TestSettings _settings;
        public Faker _faker;
        public AuthenticatedWebApplicationFactory()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Test.json")
                .Build();
            _settings = configuration.GetSection("TestSettings").Get<TestSettings>();
            _faker = new Faker();
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
    }
}