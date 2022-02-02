using AutoMapper;
using CashTrack.Data.Entities;
using CashTrack.Repositories.IncomeSourceRepository;
using CashTrack.Services.IncomeSourceService;
using Moq;
using System.Linq;
using Xunit;
using Shouldly;
using CashTrack.Models.IncomeSourceModels;
using System.Threading.Tasks;

namespace CashTrack.Tests.Services
{
    public class IncomeSourceServiceTests
    {
        private readonly Mock<IIncomeSourceRepository> _repo;
        private readonly IMapper _mapper;
        private readonly IncomeSourceService _sut;
        private IncomeSources[] _data;

        public IncomeSourceServiceTests()
        {
            _repo = new Mock<IIncomeSourceRepository>();
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new IncomeSourcesProfile()));
            _mapper = mapperConfig.CreateMapper();
            _sut = new IncomeSourceService(_repo.Object, _mapper);
            _data = GetData();
        }
        [Fact]
        public async Task GetAll()
        {
            _repo.Setup(r => r.FindWithPagination(x => true, 1, 25)).ReturnsAsync(_data);
            _repo.Setup(r => r.GetCount(x => true)).ReturnsAsync(3);
            var request = new IncomeSourceRequest();
            var result = await _sut.GetIncomeSourcesAsync(request);
            result.ListItems.Count().ShouldBe(3);
            result.TotalCount.ShouldBe(3);
            result.ListItems.FirstOrDefault()!.Name.ShouldBe("Persuade");
            result.ListItems.FirstOrDefault()!.Id.ShouldBe(1);
            result.ListItems.LastOrDefault()!.Name.ShouldBe("GrowthZone");
        }
        [Fact]
        public async Task Create()
        {
            _repo.Setup(x => x.Create(It.IsAny<IncomeSources>())).ReturnsAsync(true);
            var request = new AddEditIncomeSource() { Name = "TEST CREATE" };
            var result = await _sut.CreateIncomeSourceAsync(request);
            result.Name.ShouldBe("TEST CREATE");
            result.Id.ShouldBe(1);
        }
        [Fact]
        public async Task Update()
        {
            _repo.Setup(x => x.Update(It.IsAny<IncomeSources>())).ReturnsAsync(true);
            var objectToUpdate = new IncomeSources() { id = 1, source = "TEST CREATE" };
            _repo.Setup(x => x.FindById(1)).ReturnsAsync(objectToUpdate);
            var request = new AddEditIncomeSource() { Id = 1, Name = "TEST UPDATE" };
            var result = await _sut.UpdateIncomeSourceAsync(request);
            result.ShouldBe(true);
        }
        [Fact]
        public async Task Delete()
        {
            _repo.Setup(x => x.Delete(It.IsAny<IncomeSources>())).ReturnsAsync(true);
            var objectToUpdate = new IncomeSources() { id = 1, source = "Persuade" };
            _repo.Setup(x => x.FindById(1)).ReturnsAsync(objectToUpdate);
            var result = await _sut.DeleteIncomeSourceAsync(1);
            result.ShouldBe(true);
        }
        private static IncomeSources[] GetData()
        {
            return new IncomeSources[]
                {
                    new IncomeSources()
                    {
                        id = 1,
                        source = "Persuade",
                        description = "Money from a persuade" ,
                        in_use = true

                    },
                    new IncomeSources()
                    {
                        id = 2,
                        source = "Capillary",
                        description = "Money from capillary" ,
                        in_use = true

                    },
                    new IncomeSources()
                    {
                        id = 3,
                        source = "GrowthZone",
                        description = "Money from growth zone" ,
                        in_use = false
                    },
                };
        }
    }
}
