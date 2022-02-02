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

namespace CashTrack.Tests.Services
{
    public class UserServiceTests
    {
        private readonly IMapper _mapper;
        private readonly UserService _sut;
        private readonly Users[] _data;
        private readonly Mock<IUserRepository> _repo;

        public UserServiceTests()
        {
            _repo = new Mock<IUserRepository>();
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new UserMapperProfile()));
            _mapper = mapperConfig.CreateMapper();
            _sut = new UserService(_repo.Object, _mapper);
            _data = new Users[] {
                new Users()
                {
                    first_name = "Arthur",
                    last_name = "scott",
                    email = "arthur@example.com",
                    id = 1
                },
                new Users()
                {
                    first_name = "Edward",
                    last_name = "scott",
                    email = "edward@example.com",
                    id = 2 }
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

            _repo.Verify(r => r.Find(It.IsAny<Expression<Func<Users, bool>>>()), Times.AtLeastOnce());
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
    }
}
