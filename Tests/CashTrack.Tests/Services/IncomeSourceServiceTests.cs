//using AutoMapper;
//using CashTrack.Data.Entities;
//using CashTrack.Repositories.IncomeCategoryRepository;
//using CashTrack.Services.IncomeCategoryService;
//using Moq;
//using System.Linq;
//using Xunit;
//using Shouldly;
//using CashTrack.Models.IncomeCategoryModels;
//using Bogus;
//using CashTrack.Repositories.SubCategoriesRepository;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System.Linq.Expressions;
//using System;

//namespace CashTrack.Tests.Services
//{
//    public class IncomeCategoryServiceTests
//    {
//        private readonly Mock<IIncomeCategoryRepository> _repo;
//        private readonly IMapper _mapper;
//        private readonly IncomeCategoryService _sut;
//        private IncomeCategories[] _data;

//        public IncomeCategoryServiceTests()
//        {
//            _repo = new Mock<IIncomeCategoryRepository>();
//            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new IncomeCategoryProfile()));
//            _mapper = mapperConfig.CreateMapper();
//            _sut = new IncomeCategoryService(_repo.Object, _mapper);
//            _data = GetData();
//        }
//        [Fact]
//        public async Task GetAll()
//        {
//            _repo.Setup(r => r.FindWithPagination(x => true, 1, 25)).ReturnsAsync(_data);
//            _repo.Setup(r => r.GetCount(x => true)).ReturnsAsync(3);
//            var request = new IncomeCategoryRequest();
//            var result = await _sut.GetIncomeCategoriesAsync(request);
//            result.ListItems.Count().ShouldBe(3);
//            result.TotalCount.ShouldBe(3);
//            result.ListItems.FirstOrDefault()!.Name.ShouldBe("Paycheck");
//            result.ListItems.FirstOrDefault()!.Id.ShouldBe(1);
//            result.ListItems.LastOrDefault()!.Name.ShouldBe("government");
//        }
//        [Fact]
//        public async Task Create()
//        {
//            _repo.Setup(x => x.Create(It.IsAny<IncomeCategories>())).ReturnsAsync(true);
//            var request = new AddEditIncomeCategory() { Name = "TEST CREATE" };
//            var result = await _sut.CreateIncomeCategoryAsync(request);
//            result.Name.ShouldBe("TEST CREATE");
//            result.Id.ShouldBe(1);
//        }
//        [Fact]
//        public async Task Update()
//        {
//            _repo.Setup(x => x.Update(It.IsAny<IncomeCategories>())).ReturnsAsync(true);
//            var objectToUpdate = new IncomeCategories() { id = 1, category = "TEST CREATE" };
//            _repo.Setup(x => x.FindById(1)).ReturnsAsync(objectToUpdate);
//            var request = new AddEditIncomeCategory() { Id = 1, Name = "TEST UPDATE" };
//            var result = await _sut.UpdateIncomeCategoryAsync(request);
//            result.ShouldBe(true);
//        }
//        [Fact]
//        public async Task Delete()
//        {
//            _repo.Setup(x => x.Delete(It.IsAny<IncomeCategories>())).ReturnsAsync(true);
//            var objectToUpdate = new IncomeCategories() { id = 1, category = "Paycheck" };
//            _repo.Setup(x => x.FindById(1)).ReturnsAsync(objectToUpdate);
//            var result = await _sut.DeleteIncomeCategoryAsync(1);
//            result.ShouldBe(true);
//        }
//        private static IncomeCategories[] GetData()
//        {
//            return new IncomeCategories[]
//                {
//                    new IncomeCategories()
//                    {
//                        id = 1,
//                        category = "Paycheck",
//                        description = "Money from a paycheck" ,
//                        in_use = true

//                    },
//                    new IncomeCategories()
//                    {
//                        id = 2,
//                        category = "Gifts",
//                        description = "Money from gifts" ,
//                        in_use = true

//                    },
//                    new IncomeCategories()
//                    {
//                        id = 3,
//                        category = "government",
//                        description = "Money from gubmint" ,
//                        in_use = false
//                    },
//                };
//        }
//    }
//}
