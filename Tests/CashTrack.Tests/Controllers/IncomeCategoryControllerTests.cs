using CashTrack.Controllers;
using CashTrack.Models.IncomeCategoryModels;
using CashTrack.Services.IncomeCategoryService;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CashTrack.Tests.Controllers
{
    public class IncomeCategoryControllerTests
    {
        private readonly Mock<IIncomeCategoryService> _service;
        private readonly IncomeCategoryController _sut;

        public IncomeCategoryControllerTests()
        {
            _service = new Mock<IIncomeCategoryService>();
            _sut = new IncomeCategoryController(_service.Object);
        }
        [Fact]
        public async void GetAll()
        {
            var request = new IncomeCategoryRequest();
            var result = await _sut.GetIncomeCategories(request);
            var viewResult = Assert.IsType<ActionResult<IncomeCategoryResponse>>(result);
            _service.Verify(s => s.GetIncomeCategoriesAsync(It.IsAny<IncomeCategoryRequest>()), Times.AtLeastOnce());
        }
        [Fact]
        public async void Create()
        {
            var request = new AddEditIncomeCategory();
            var result = await _sut.CreateIncomeCategory(request);
            var viewResult = Assert.IsType<ActionResult<AddEditIncomeCategory>>(result);
            _service.Verify(s => s.CreateIncomeCategoryAsync(It.IsAny<AddEditIncomeCategory>()), Times.AtLeastOnce());
        }
        [Fact]
        public async void Update()
        {
            var request = new AddEditIncomeCategory() { Id = int.MaxValue };
            var result = await _sut.UpdateIncomeCategory(request);
            _service.Verify(s => s.UpdateIncomeCategoryAsync(It.IsAny<AddEditIncomeCategory>()), Times.AtLeastOnce());
        }
        [Fact]
        public async void Delete()
        {
            var result = await _sut.DeleteIncomeCategory(int.MaxValue);
            _service.Verify(s => s.DeleteIncomeCategoryAsync(It.IsAny<int>()), Times.AtLeastOnce());
        }
    }
}
