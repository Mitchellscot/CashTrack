using AutoMapper;
using CashTrack.Data.Entities;
using CashTrack.Repositories.UserRepository;
using CashTrack.Services.UserService;
using Moq;
using Xunit;
using Shouldly;
using CashTrack.Models.UserModels;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using CashTrack.Tests.Services.Common;

namespace CashTrack.Tests.Services
{
    public class UserServiceTests
    {
        private readonly IMapper _mapper;
        private readonly UserService _sut;
        private readonly UserRepository _repo;
        private readonly Mock<UserManager<UserEntity>> _userManager;
        private readonly Mock<SignInManager<UserEntity>> _signInManager;

        public UserServiceTests()
        {
            var userMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new UserMapperProfile());
            });
            _mapper = userMapper.CreateMapper();
            var sharedDB = new AppDbContextFactory().CreateDbContext();
            _repo = new UserRepository(sharedDB);

            _userManager = new Mock<UserManager<UserEntity>>(Mock.Of<IUserStore<UserEntity>>(), null, null, null, null, null, null, null, null);
            _signInManager = new Mock<SignInManager<UserEntity>>(_userManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<UserEntity>>(), null, null, null, null);

            _sut = new UserService(_repo, _mapper, _userManager.Object, _signInManager.Object);

        }
        [Fact]
        public async void GetAllUsers()
        {
            var result = await _sut.GetAllUsersAsync();
            result.FirstOrDefault()!.FirstName.ShouldBe("Test");
            result.FirstOrDefault()!.Email.ShouldBe("test@example.com");
        }
        [Fact]
        public async void GetById()
        {
            var result = await _sut.GetUserByIdAsync(1);
            result.FirstName.ShouldBe("Test");
            result.id.ShouldBe(1);
        }
        [Fact]
        public async void CreateUser()
        {
            var user = new UserModels.AddEditUser()
            {
                FirstName = "Arthur",
                LastName = "Scott",
                Email = "arthur@example.com",
                Password = "Chewbaca"
            };

            _userManager.Setup(u => u.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Success));
            _userManager.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new UserEntity()
            {
                Id = 2,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            });

            var result = await _sut.CreateUserAsync(user);
            result.FirstName.ShouldBe(user.FirstName);
            result.id.ShouldBe(2);
            _signInManager.Verify(r => r.RefreshSignInAsync(It.IsAny<UserEntity>()), Times.AtLeastOnce());

        }
    }
}
