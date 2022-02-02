using AutoMapper;
using CashTrack.Data.Entities;
using CashTrack.Repositories.MainCategoriesRepository;
using CashTrack.Services.MainCategoriesService;
using Moq;
using System.Linq;
using Xunit;
using Shouldly;
using CashTrack.Models.MainCategoryModels;
using Bogus;
using CashTrack.Repositories.SubCategoriesRepository;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System;

namespace CashTrack.Tests.Services
{
    public class MainCategoryServiceTests
    {
        private readonly Mock<IMainCategoriesRepository> _repo;
        private readonly Mock<ISubCategoryRepository> _subRepo;
        private readonly IMapper _mapper;
        private readonly MainCategoriesService _sut;
        private readonly Faker _faker;
        private MainCategories[] _data;

        public MainCategoryServiceTests()
        {
            _repo = new Mock<IMainCategoriesRepository>();
            _subRepo = new Mock<ISubCategoryRepository>();
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new MainCategoryProfile()));
            _mapper = mapperConfig.CreateMapper();
            _sut = new MainCategoriesService(_repo.Object, _mapper, _subRepo.Object);
            _faker = new Faker();
            _data = GetData();
        }
        [Fact]
        public async Task GetAll()
        {
            _subRepo.Setup(s => s.GetCount(It.IsAny<Expression<Func<SubCategories, bool>>>())).ReturnsAsync(1);
            _repo.Setup(r => r.Find(x => true)).ReturnsAsync(_data);
            var request = new MainCategoryRequest();
            var result = await _sut.GetMainCategoriesAsync(request);
            result.MainCategories.Count().ShouldBe(2);
            result.TotalMainCategories.ShouldBe(2);
            result.MainCategories.FirstOrDefault()!.Name.ShouldBe("FOOD");
            result.MainCategories.FirstOrDefault()!.NumberOfSubCategories.ShouldBe(1);
            result.MainCategories.LastOrDefault()!.Name.ShouldBe("Housing");
        }
        [Fact]
        public async Task Create()
        {
            _repo.Setup(x => x.Create(It.IsAny<MainCategories>())).ReturnsAsync(true);
            var request = new AddEditMainCategory() { Name = "TEST CREATE" };
            var result = await _sut.CreateMainCategoryAsync(request);
            result.Name.ShouldBe("TEST CREATE");
            result.Id.ShouldBe(1);
        }
        [Fact]
        public async Task Update()
        {
            _repo.Setup(x => x.Update(It.IsAny<MainCategories>())).ReturnsAsync(true);
            var objectToUpdate = new MainCategories() { id = 1, main_category_name = "TEST CREATE" };
            _repo.Setup(x => x.FindById(1)).ReturnsAsync(objectToUpdate);
            var request = new AddEditMainCategory() { Id = 1, Name = "TEST UPDATE" };
            var result = await _sut.UpdateMainCategoryAsync(request);
            result.ShouldBe(true);
        }
        [Fact]
        public async Task Delete()
        {
            _repo.Setup(x => x.Delete(It.IsAny<MainCategories>())).ReturnsAsync(true);
            var objectToUpdate = new MainCategories() { id = 1, main_category_name = "TEST UPDATE" };
            _repo.Setup(x => x.FindById(1)).ReturnsAsync(objectToUpdate);
            var result = await _sut.DeleteMainCategoryAsync(1);
            result.ShouldBe(true);
        }
        private static MainCategories[] GetData()
        {
            return new MainCategories[]
                {
                    new MainCategories()
                    {
                        id = 1,
                        main_category_name = "FOOD",
                        sub_categories = new List<SubCategories>() { new SubCategories(){
                            id = 1,
                            sub_category_name="Groceries",
                            main_categoryid=1,

                        } }
                    },
                    new MainCategories()
                    {
                        id = 2,
                        main_category_name = "Housing",
                        sub_categories= new List<SubCategories>()
                    },
                };
        }
    }
}
