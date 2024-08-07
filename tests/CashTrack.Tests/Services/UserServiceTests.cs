﻿using AutoMapper;
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
using Bogus.DataSets;
using System;

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

            _userManager = new Mock<UserManager<UserEntity>>(Mock.Of<IUserStore<UserEntity>>(), null!, null!, null!, null!, null!, null!, null!, null!);
            _signInManager = new Mock<SignInManager<UserEntity>>(_userManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<UserEntity>>(), null!, null!, null!, null!);

            _sut = new UserService(_repo, _mapper, _userManager.Object, _signInManager.Object);

        }
        [Fact]
        public async Task GetAllUsers()
        {
            var result = await _sut.GetAllUsersAsync();
            result.FirstOrDefault()!.FirstName.ShouldBe("Test");
            result.FirstOrDefault()!.Email.ShouldBe("test@example.com");
        }
        [Fact]
        public async Task GetById()
        {
            var result = await _sut.GetUserByIdAsync(1);
            result.FirstName.ShouldBe("Test");
            result.Id.ShouldBe(1);
        }
        [Fact]
        public async Task CreateUser()
        {
            var user = new AddEditUser()
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
            result.Id.ShouldBe(2);
            _signInManager.Verify(r => r.RefreshSignInAsync(It.IsAny<UserEntity>()), Times.AtLeastOnce());

        }
        [Fact]
        public async Task Can_Get_Default_Tax()
        {
            var defaultTax = 0.07875M;
            _userManager.Setup(u => u.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new UserEntity()
            {
                Id = 1,
                UserName = "test",
                DefaultTax = defaultTax
            });
            var tax = await _sut.GetDefaultTax("test");
            tax.ShouldBe(defaultTax);
        }
        [Fact]
        public async Task Can_Update_Default_Tax()
        {
            var defaultTax = 0.07875M;
            _userManager.Setup(u => u.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new UserEntity()
            {
                Id = 1,
                UserName = "test",
                DefaultTax = defaultTax
            });
            _userManager.Setup(u => u.UpdateAsync(It.IsAny<UserEntity>())).ReturnsAsync(IdentityResult.Success);
            var newTax = 0.10M;
            var result = await _sut.UpdateDefaultTax("test", newTax);
            result.ShouldBeTrue();
            var updatedTax = await _sut.GetDefaultTax("test");
            updatedTax.ShouldBe(newTax);
        }
        [Fact]
        public async Task Can_Update_UserName()
        {
            var newUser = new AddEditUser()
            {
                FirstName = "Arthur",
                LastName = "Scott",
                Email = "arthur@example.com",
                Password = "Chewbaca"
            };
            _userManager.Setup(u => u.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Success));
            _userManager.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new UserEntity()
            {
                Id = 1,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Email = newUser.Email
            });
            _userManager.Setup(u => u.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new UserEntity()
            {
                Id = 1,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Email = newUser.Email
            });
            _userManager.Setup(u => u.UpdateAsync(It.IsAny<UserEntity>())).ReturnsAsync(IdentityResult.Success);
            _userManager.Setup(u => u.CheckPasswordAsync(It.IsAny<UserEntity>(), It.IsAny<string>())).ReturnsAsync(true);
            var request = new ChangeUsername()
            {
                Username = newUser.FirstName,
                Password = newUser.Password,
                NewUsername = "Edward",
                ConfirmUsername = "Edward"
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var repo = new UserRepository(db);
                var service = new UserService(repo, _mapper, _userManager.Object, _signInManager.Object);
                var createUserResult = await service.CreateUserAsync(newUser);
                var getUserEntity = await service.GetUserByIdAsync(createUserResult.Id);
                var result = await service.UpdateUsernameAsync(request);
                result.ShouldBeTrue();
            }
        }
        [Fact]
        public async Task Can_Update_Password()
        {
            var newUser = new AddEditUser()
            {
                FirstName = "Arthur",
                LastName = "Scott",
                Email = "arthur@example.com",
                Password = "Chewbaca"
            };
            _userManager.Setup(u => u.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Success));
            _userManager.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new UserEntity()
            {
                Id = 1,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Email = newUser.Email
            });
            _userManager.Setup(u => u.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new UserEntity()
            {
                Id = 1,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Email = newUser.Email
            });
            _userManager.Setup(u => u.ChangePasswordAsync(It.IsAny<UserEntity>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            var request = new ChangePassword()
            {
                Username = newUser.FirstName,
                OldPassword = newUser.Password,
                NewPassword = "Han-Solo",
                ConfirmPassword = "Han-Solo"
            };
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var repo = new UserRepository(db);
                var service = new UserService(repo, _mapper, _userManager.Object, _signInManager.Object);
                var createUserResult = await service.CreateUserAsync(newUser);
                var getUserEntity = await service.GetUserByIdAsync(createUserResult.Id);
                var result = await service.UpdatePasswordAsync(request);
                result.ShouldBeTrue();
            }
        }
        [Fact]
        public async Task Can_Update_Last_Import_Date()
        {
            var newUser = new AddEditUser()
            {
                FirstName = "Arthur",
                LastName = "Scott",
                Email = "arthur@example.com",
                Password = "Chewbaca"
            };
            _userManager.Setup(u => u.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Success));
            _userManager.Setup(u => u.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new UserEntity()
            {
                Id = 1,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Email = newUser.Email
            });
            _userManager.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new UserEntity()
            {
                Id = 1,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Email = newUser.Email
            });
            _userManager.Setup(u => u.UpdateAsync(It.IsAny<UserEntity>())).ReturnsAsync(IdentityResult.Success);
            using (var db = new AppDbContextFactory().CreateDbContext())
            {
                var repo = new UserRepository(db);
                var service = new UserService(repo, _mapper, _userManager.Object, _signInManager.Object);
                var createUserResult = await service.CreateUserAsync(newUser);
                var result = await service.UpdateLastImportDate(createUserResult.Id);
                result.ShouldBeTrue();
            }
        }
        [Fact]
        public async Task Throws_When_Passwords_Dont_Match()
        {
            var request = new ChangePassword()
            {
                Username = "Arthur",
                OldPassword = "Chewbaca",
                NewPassword = "Han-Solo",
                ConfirmPassword = "HanSolo"
            };
            _userManager.Setup(u => u.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new UserEntity()
            {
                Id = 1,
                FirstName = request.Username,
                LastName = "Scott",
                Email = "arthur@example.com"
            });
            await Task.Run(() => Should.Throw<ArgumentException>(async () => await _sut.UpdatePasswordAsync(request)));
        }
        [Fact]
        public async Task Throws_When_Password_Is_Null()
        {
            var request = new ChangePassword()
            {
                Username = "Arthur",
                OldPassword = "Chewbaca",
                NewPassword = "Han-Solo",
                ConfirmPassword = ""
            };
            _userManager.Setup(u => u.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new UserEntity()
            {
                Id = 1,
                FirstName = request.Username,
                LastName = "Scott",
                Email = "arthur@example.com"
            });
            await Task.Run(() => Should.Throw<ArgumentNullException>(async () => await _sut.UpdatePasswordAsync(request)));
        }
    }
}