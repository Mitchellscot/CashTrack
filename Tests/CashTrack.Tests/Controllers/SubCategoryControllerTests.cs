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
        public async void GetAll()
        {
            var request = new SubCategoryRequest();
            var result = await _sut.GetAllSubCategories(request);
            var viewResult = Assert.IsType<ActionResult<SubCategoryResponse>>(result);
            _service.Verify(s => s.GetSubCategoriesAsync(It.IsAny<SubCategoryRequest>()), Times.AtLeastOnce());
        }
        [Fact]
        public async void Create()
        {
            var request = new AddEditSubCategory();
            var result = await _sut.CreateSubCategory(request);
            var viewResult = Assert.IsType<ActionResult<AddEditSubCategory>>(result);
            _service.Verify(s => s.CreateSubCategoryAsync(It.IsAny<AddEditSubCategory>()), Times.AtLeastOnce());
        }
        [Fact]
        public async void Update()
        {
            var request = new AddEditSubCategory() { Id = int.MaxValue };
            var result = await _sut.UpdateSubCategory(request);
            _service.Verify(s => s.UpdateSubCategoryAsync(It.IsAny<AddEditSubCategory>()), Times.AtLeastOnce());
        }
        [Fact]
        public async void Delete()
        {
            var result = await _sut.DeleteSubCategory(int.MaxValue);
            _service.Verify(s => s.DeleteSubCategoryAsync(It.IsAny<int>()), Times.AtLeastOnce());
        }
    }
}
