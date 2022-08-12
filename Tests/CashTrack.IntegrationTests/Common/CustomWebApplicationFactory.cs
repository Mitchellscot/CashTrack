using CashTrack.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;

namespace CashTrack.IntegrationTests.Common
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
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
                    //options.UseInMemoryDatabase(Guid.NewGuid().ToString());
                    //options.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=CashTrackTest;Trusted_Connection=True;");
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
                        .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                    db.Database.EnsureCreated();

                    //I think the database would already be seeded, though if not you can do it here.
                    //try
                    //{
                    //    Utilities.InitializeDbForTests(db);
                    //}
                    //catch (Exception ex)
                    //{
                    //    logger.LogError(ex, "An error occurred seeding the " +
                    //        "database with test messages. Error: {Message}", ex.Message);
                    //}
                }

            });
        }
    }
}