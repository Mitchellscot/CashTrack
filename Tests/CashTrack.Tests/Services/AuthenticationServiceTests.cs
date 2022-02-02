using AutoMapper;
using CashTrack.Common;
using CashTrack.Data;
using CashTrack.Data.Entities;
using CashTrack.IntegrationTests;
using CashTrack.IntegrationTests.Common;
using CashTrack.Models.AuthenticationModels;
using CashTrack.Services.AuthenticationService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using System.IO;
using Xunit;

namespace CashTrack.Tests.Services
{
    public class AuthenticationServiceTests
    {
        private readonly AuthenticationService _sut;
        private readonly TestSettings _userCreds;

        public AuthenticationServiceTests()
        {
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new AuthorizationProfile()));
            var mapper = mapperConfig.CreateMapper();
            var logger = Mock.Of<ILogger<AuthenticationService>>();
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Test.json")
                .Build();
            var appSettings = config.GetSection("AppSettings").Get<AppSettings>();
            var settings = new TestOptionsSnapshot<AppSettings>(appSettings);
            _userCreds = config.GetSection("TestSettings").Get<TestSettings>();
            var dbOptions = new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(config.GetConnectionString("TestDb")).Options;
            var context = new AppDbContext(dbOptions, config);
            _sut = new AuthenticationService(context, mapper, logger, settings);
        }
        [Fact]
        public async void AuthenticateUser()
        {
            var userRequest = new AuthenticationModels.Request(_userCreds.Username, _userCreds.Password);
            var result = await _sut.AuthenticateAsync(userRequest);
            Assert.NotNull(result);
            result.FirstName.ShouldBe(_userCreds.Username);
        }
        [Fact]
        public async void ReturnNullWithWrongCreds()
        {
            var userRequest = new AuthenticationModels.Request("hacker", "password123");
            var result = await _sut.AuthenticateAsync(userRequest);
            Assert.Null(result);
        }
        [Fact]
        public void GenerateJWTToken()
        {
            var henry = new Users()
            {
                id = 1,
                first_name = "Henry",
                last_name = "Scott",
                email = "Henry@example.com",
                password_hash = "blahblahblah"
            };
            var lydia = new Users()
            {
                id = 2,
                first_name = "Lydia",
                last_name = "Scott",
                email = "Lydia@example.com",
                password_hash = "blahblahblah"
            };
            var henryToken = _sut.GenerateJwtToken(henry);
            var henryTokenAgain = _sut.GenerateJwtToken(henry);
            var lydiaToken = _sut.GenerateJwtToken(lydia);
            Assert.NotNull(henryToken);
            Assert.NotNull(henryTokenAgain);
            Assert.NotNull(lydiaToken);
            //this might fail if the tests are taking a long time... that's alright just rerun it. Or delete it, whatever.
            Assert.Equal(henryToken, henryTokenAgain);
            Assert.NotEqual(henryToken, lydiaToken);
        }
    }
}
