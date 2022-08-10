using CashTrack.Controllers;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Services.SubCategoryService;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CashTrack.Tests.Controllers
{
    public class SubCategoryControllerTests
    {
        private readonly Mock<ISubCategoryService> _service;
        private readonly SubCategoryController _sut;

        public SubCategoryControllerTests()
        {
            _service = new Mock<ISubCategoryService>();
            _sut = new SubCategoryController(_service.Object);
        }
        [Fact]
        public async void Should_GetAllSubCategoriesForDropDownList()
        {
            var result = await _sut.GetAllSubCategoriesForDropDownList();
            var viewResult = Assert.IsType<ActionResult<SubCategoryResponse>>(result);
            _service.Verify(s => s.GetSubCategoriesAsync(It.IsAny<SubCategoryRequest>()), Times.AtLeastOnce());
        }
        [Fact]
        public async void Should_GetMatchingSubCategoryNames()
        {
            var result = await _sut.GetMatchingSubCategoryNames("car");
            var viewResult = Assert.IsType<ActionResult<SubCategoryResponse>>(result);
            _service.Verify(s => s.GetSubCategoriesAsync(It.IsAny<SubCategoryRequest>()), Times.AtLeastOnce());
        }
    }
}
