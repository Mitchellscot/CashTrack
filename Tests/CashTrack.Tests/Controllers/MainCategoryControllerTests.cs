using CashTrack.Controllers;
using CashTrack.Models.MainCategoryModels;
using CashTrack.Services.MainCategoriesService;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CashTrack.Tests.Controllers
{
    public class MainCategoryControllerTests
    {
        private readonly Mock<IMainCategoriesService> _service;
        private readonly MainCategoryController _sut;

        public MainCategoryControllerTests()
        {
            _service = new Mock<IMainCategoriesService>();
            _sut = new MainCategoryController(_service.Object);
        }
        [Fact]
        public async void GetAll()
        {
            var request = new MainCategoryRequest();
            var result = await _sut.GetMainCategories(request);
            var viewResult = Assert.IsType<ActionResult<MainCategoryResponse>>(result);
            _service.Verify(x => x.GetMainCategoriesAsync(It.IsAny<MainCategoryRequest>()), Times.AtLeastOnce());
        }
        [Fact]
        public async void Create()
        {
            var request = new AddEditMainCategory();
            var result = await _sut.CreateMainCategory(request);
            var viewResult = Assert.IsType<ActionResult<AddEditMainCategory>>(result);
            _service.Verify(x => x.CreateMainCategoryAsync(It.IsAny<AddEditMainCategory>()), Times.AtLeastOnce());
        }
        [Fact]
        public async void Update()
        {
            var request = new AddEditMainCategory() { Id = int.MaxValue };
            var result = await _sut.UpdateMainCategory(request);
            _service.Verify(x => x.UpdateMainCategoryAsync(It.IsAny<AddEditMainCategory>()), Times.AtLeastOnce());
        }
        [Fact]
        public async void Delete()
        {
            var result = await _sut.DeleteMainCategory(int.MaxValue);
            _service.Verify(x => x.DeleteMainCategoryAsync(It.IsAny<int>()), Times.AtLeastOnce());
        }
    }
}
