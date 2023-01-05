using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.IO;
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace CashTrack.Data
{
    public class AppContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

            if (env == "Production")
            {
                optionsBuilder.UseSqlServer(config.GetConnectionString("Production"), builder =>
                {
                    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(5), null);
                });
            }
            else
            {
                optionsBuilder.UseSqlite("Data Source=cashtrack.db");
            }
            bool seed = false;
            if (args.Length > 0 && args[0] == "seed")
                seed = true;

            return new AppDbContext(optionsBuilder.Options, new FakeWebHostEnvironment(), seed);
        }
    }
    public class FakeWebHostEnvironment : IWebHostEnvironment
    {
        public string WebRootPath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IFileProvider WebRootFileProvider { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IFileProvider ContentRootFileProvider { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ContentRootPath { get; set; }
        public string EnvironmentName { get; set; }
        public FakeWebHostEnvironment()
        {
            EnvironmentName = "Development";
            ContentRootPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()!)!.Parent!.Parent!.FullName);
        }
    }
}
