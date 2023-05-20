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
    /// This purpose of this class is to generate a db context from the EF CLI and accept arguments from the command line. See New-Db.ps1 for an example.
    /// </summary>
    public class AppContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? CashTrackEnv.Development;
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            var settings = new AppSettingsOptions();
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
            config.GetSection(AppSettingsOptions.AppSettings).Bind(settings);

            var connectionString = env.Equals(CashTrackEnv.Production, StringComparison.InvariantCultureIgnoreCase) ? $"Data Source={
                Path.Join(Directory.GetCurrentDirectory(),
                settings.ConnectionStrings[env])}" : $"Data Source={Path.Join(Directory.GetCurrentDirectory(),
                "Data",
                settings.ConnectionStrings[env])}";
            optionsBuilder.UseSqlite(connectionString);

            var arguments = args.Length > 0 ? args[0] : string.Empty;

            return new AppDbContext(optionsBuilder.Options, new FakeWebHostEnvironment(env), arguments);
        }
    }
    public class FakeWebHostEnvironment : IWebHostEnvironment
    {
        public string WebRootPath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IFileProvider WebRootFileProvider { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IFileProvider ContentRootFileProvider { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string EnvironmentName { get; set; }
        public string ContentRootPath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public FakeWebHostEnvironment(string env)
        {
            EnvironmentName = env;
        }
    }
}
