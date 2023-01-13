using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.IO;
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using CashTrack.Common;

namespace CashTrack.Data
{
    /// <summary>
    /// This purpose of this clas is to generate a db context from the EF CLI and accept arguments from the command line. See New-Lite.ps1 for an example.
    /// </summary>
    public class AppContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            var settings = new AppSettingsOptions();
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
            config.GetSection(AppSettingsOptions.AppSettings).Bind(settings);

            if (env == CashTrackEnv.Development)
            {
                var connectionString = $"Data Source={Path.Join(Directory.GetCurrentDirectory(),
                    "Data",
                    settings.ConnectionStrings[env]
                    )}";
                optionsBuilder.UseSqlite(connectionString);
            }
            else
            {
                optionsBuilder.UseSqlServer(config.GetConnectionString(CashTrackEnv.Production), builder =>
                {
                    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(5), null);
                });

            }
            bool seed = false;
            if (args.Length > 0 && args[0] == "seed")
                seed = true;

            return new AppDbContext(optionsBuilder.Options, new FakeWebHostEnvironment(env), seed);
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
        public FakeWebHostEnvironment(string env)
        {
            EnvironmentName = env;
            ContentRootPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()!)!.Parent!.Parent!.FullName);
        }
    }
}
