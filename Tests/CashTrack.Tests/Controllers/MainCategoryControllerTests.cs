//using CashTrack.Controllers;
//using CashTrack.Models.MainCategoryModels;
//using CashTrack.Services.MainCategoriesService;
//using Microsoft.AspNetCore.Mvc;
//using Moq;
//using Xunit;

//namespace CashTrack.Tests.Controllers
//{
//    public class MainCategoryControllerTests
//    {
//        private readonly Mock<IMainCategoriesService> _service;
//        private readonly MainCategoryController _sut;

//        public MainCategoryControllerTests()
//        {
//            _service = new Mock<IMainCategoriesService>();
//            _sut = new MainCategoryController(_service.Object);
//        }
//        [Fact]
//        public async void Should_GetMainCategoriesForDropdownList()
//        {
//            var result = await _sut.GetMainCategoriesForDropdownList();
//            var viewResult = Assert.IsType<ActionResult<MainCategoryResponse>>(result);
//            _service.Verify(x => x.GetMainCategoriesAsync(It.IsAny<MainCategoryRequest>()), Times.AtLeastOnce());
//        }
//        [Fact]
//        public async void Should_GetMainCategoryNameBySubCategoryId()
//        {
//            var result = await _sut.GetMainCategoryNameBySubCategoryId(1);
//            var viewResult = Assert.IsType<ActionResult<MainCategoryResponse>>(result);
//            _service.Verify(x => x.GetMainCategoriesAsync(It.IsAny<MainCategoryRequest>()), Times.AtLeastOnce());
//        }
//    }
//}
