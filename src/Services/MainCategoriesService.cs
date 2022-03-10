using AutoMapper;
using CashTrack.Data.Entities;
using CashTrack.Common.Exceptions;
using CashTrack.Models.MainCategoryModels;
using CashTrack.Repositories.MainCategoriesRepository;
using CashTrack.Repositories.SubCategoriesRepository;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CashTrack.Services.MainCategoriesService
{
    public interface IMainCategoriesService
    {
        Task<MainCategoryResponse> GetMainCategoriesAsync(MainCategoryRequest request);
        Task<MainCategoryDetail> GetMainCategoryDetailAsync(int id);
        Task<string> GetMainCategoryNameBySubCategoryIdAsync(int id);
        Task<AddEditMainCategory> CreateMainCategoryAsync(AddEditMainCategory request);
        Task<bool> UpdateMainCategoryAsync(AddEditMainCategory request);
        Task<bool> DeleteMainCategoryAsync(int id);
    }
    public class MainCategoriesService : IMainCategoriesService
    {
        private readonly IMainCategoriesRepository _mainCategoryRepo;
        private readonly ISubCategoryRepository _subCategoryRepository;
        private readonly IMapper _mapper;

        public MainCategoriesService(IMainCategoriesRepository mainCategoryRepository, IMapper mapper, ISubCategoryRepository subCategoryRepository)
        {
            _mainCategoryRepo = mainCategoryRepository;
            _subCategoryRepository = subCategoryRepository;
            _mapper = mapper;
        }

        public async Task<AddEditMainCategory> CreateMainCategoryAsync(AddEditMainCategory request)
        {
            var categories = await _mainCategoryRepo.Find(x => true);
            if (categories.Any(x => x.main_category_name == request.Name))
                throw new DuplicateNameException(nameof(MainCategories), request.Name);

            var category = _mapper.Map<MainCategories>(request);

            if (!await _mainCategoryRepo.Create(category))
                throw new Exception("Unable to save category to the database");

            request.Id = category.Id;

            return request;
        }

        public async Task<bool> DeleteMainCategoryAsync(int id)
        {
            var category = await _mainCategoryRepo.FindById(id);
            if (category == null)
                throw new CategoryNotFoundException(id.ToString());

            return await _mainCategoryRepo.Delete(category);
        }

        public async Task<MainCategoryResponse> GetMainCategoriesAsync(MainCategoryRequest request)
        {
            Expression<Func<MainCategories, bool>> search = (MainCategories x) => x.main_category_name.ToLower().Contains(request.Query);
            Expression<Func<MainCategories, bool>> returnAll = (MainCategories x) => true;

            var predicate = request.Query == null ? returnAll : search;
            var categories = await _mainCategoryRepo.Find(predicate);
            var listItems = categories.Select(mc => new MainCategoryListItem()
            {
                Id = mc.Id,
                Name = mc.main_category_name,
                NumberOfSubCategories = (int)_subCategoryRepository.GetCount(c => c.main_categoryid == mc.Id).Result
            }).ToArray();

            var response = new MainCategoryResponse()
            {
                TotalMainCategories = categories.Count(),
                MainCategories = listItems
            };
            return response;
        }


        public Task<MainCategoryDetail> GetMainCategoryDetailAsync(int id)
        {
            //think on this one
            throw new NotImplementedException();
        }

        public async Task<string> GetMainCategoryNameBySubCategoryIdAsync(int id)
        {
            var subCategory = await _subCategoryRepository.FindById(id);
            if (subCategory == null)
                throw new CategoryNotFoundException(id.ToString());
            var mainCategory = await _mainCategoryRepo.FindById(subCategory.main_categoryid);
            if (mainCategory == null)
                throw new CategoryNotFoundException(id.ToString());
            return mainCategory.main_category_name;
        }

        public async Task<bool> UpdateMainCategoryAsync(AddEditMainCategory request)
        {
            var checkId = await _mainCategoryRepo.FindById(request.Id.Value);
            if (checkId == null)
                throw new CategoryNotFoundException(request.Id.Value.ToString());

            var nameCheck = await _mainCategoryRepo.Find(x => x.main_category_name == request.Name);
            if (nameCheck.Any())
                throw new DuplicateNameException(nameof(MainCategories), request.Name);

            var category = _mapper.Map<MainCategories>(request);
            return await _mainCategoryRepo.Update(category);
        }
    }
}
public class MainCategoryProfile : Profile
{
    public MainCategoryProfile()
    {
        CreateMap<AddEditMainCategory, MainCategories>()
            .ForMember(c => c.Id, o => o.MapFrom(src => src.Id))
            .ForMember(c => c.main_category_name, o => o.MapFrom(src => src.Name));
    }
}
