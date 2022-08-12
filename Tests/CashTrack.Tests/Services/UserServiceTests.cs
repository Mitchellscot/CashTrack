using AutoMapper;
using CashTrack.Data.Entities;
using CashTrack.Repositories.UserRepository;
using CashTrack.Services.UserService;
using Moq;
using System;
using System.Linq.Expressions;
using Xunit;
using Shouldly;
using CashTrack.Models.UserModels;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CashTrack.Tests.Services
{
    public class UserServiceTests
    {
        private readonly IMapper _mapper;
        private readonly UserService _sut;
        private readonly UserEntity[] _data;
        private readonly Mock<IUserRepository> _repo;
        private readonly Mock<UserManager<UserEntity>> _userManager;
        private readonly Mock<SignInManager<UserEntity>> _signInManager;

        public UserServiceTests()
        {
            _repo = new Mock<IUserRepository>();
            _userManager = new Mock<UserManager<UserEntity>>(Mock.Of<IUserStore<UserEntity>>(), null, null, null, null, null, null, null, null);
            _signInManager = new Mock<SignInManager<UserEntity>>(_userManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<UserEntity>>(), null, null, null, null);
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new UserMapperProfile()));
            _mapper = mapperConfig.CreateMapper();
            _sut = new UserService(_repo.Object, _mapper, _userManager.Object, _signInManager.Object);
            _data = new UserEntity[] {
                new UserEntity()
                {
                    FirstName = "Arthur",
                    LastName = "scott",
                    Email = "arthur@example.com",
                    Id = 1
                },
                new UserEntity()
                {
                    FirstName = "Edward",
                    LastName = "scott",
                    Email = "edward@example.com",
                    Id = 2 }
            };
        }
        [Fact]
        public async void GetAllUsers()
        {
            _repo.Setup(r => r.Find(x => true)).ReturnsAsync(_data);

            var result = await _sut.GetAllUsersAsync();
            result.ShouldBeEquivalentTo(_mapper.Map<UserModels.Response[]>(_data));
            result.FirstOrDefault()!.FirstName.ShouldBe("Arthur");
            result.LastOrDefault()!.FirstName.ShouldBe("Edward");

            _repo.Verify(r => r.Find(It.IsAny<Expression<Func<UserEntity, bool>>>()), Times.AtLeastOnce());
        }
        [Fact]
        public async void GetById()
        {
            _repo.Setup(r => r.FindById(1)).ReturnsAsync(_data[0]);

            var result = await _sut.GetUserByIdAsync(1);
            result.ShouldBeEquivalentTo(_mapper.Map<UserModels.Response>(_data[0]));
            result.FirstName.ShouldBe("Arthur");
            _repo.Verify(r => r.FindById(It.IsAny<int>()), Times.AtLeastOnce());
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
            _repo.Setup(r => r.Create(It.IsAny<UserEntity>())).ReturnsAsync(1);
            _userManager.Setup(u => u.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Success));
            _userManager.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new UserEntity()
            {
                Id = 1,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            });

            var result = await _sut.CreateUserAsync(user);
            result.FirstName.ShouldBe(user.FirstName);
            _signInManager.Verify(r => r.RefreshSignInAsync(It.IsAny<UserEntity>()), Times.AtLeastOnce());

        }
    }
}
