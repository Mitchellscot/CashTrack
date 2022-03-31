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
        Task<MainCategoryDropdownSelection[]> GetMainCategoriesForDropdownListAsync();
        Task<int> UpdateMainCategoryAsync(AddEditMainCategory request);
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
            if (categories.Any(x => x.Name == request.Name))
                throw new DuplicateNameException(nameof(MainCategoryEntity), request.Name);

            var category = _mapper.Map<MainCategoryEntity>(request);

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
            Expression<Func<MainCategoryEntity, bool>> search = (MainCategoryEntity x) => x.Name.ToLower().Contains(request.Query);
            Expression<Func<MainCategoryEntity, bool>> returnAll = (MainCategoryEntity x) => true;

            var predicate = request.Query == null ? returnAll : search;
            var categories = await _mainCategoryRepo.Find(predicate);
            var listItems = categories.Select(mc => new MainCategoryListItem()
            {
                Id = mc.Id,
                Name = mc.Name,
                NumberOfSubCategories = (int)_subCategoryRepository.GetCount(c => c.MainCategoryId == mc.Id).Result
            }).ToArray();

            var response = new MainCategoryResponse()
            {
                TotalMainCategories = categories.Count(),
                MainCategories = listItems
            };
            return response;
        }

        public async Task<MainCategoryDropdownSelection[]> GetMainCategoriesForDropdownListAsync()
        {
            return (await _mainCategoryRepo.Find(x => true)).Select(x => new MainCategoryDropdownSelection()
            {
                Id = x.Id,
                Category = x.Name
            }).ToArray();
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
            var mainCategory = await _mainCategoryRepo.FindById(subCategory.MainCategoryId);
            if (mainCategory == null)
                throw new CategoryNotFoundException(id.ToString());
            return mainCategory.Name;
        }

        public async Task<int> UpdateMainCategoryAsync(AddEditMainCategory request)
        {
            var checkId = await _mainCategoryRepo.FindById(request.Id.Value);
            if (checkId == null)
                throw new CategoryNotFoundException(request.Id.Value.ToString());

            var nameCheck = await _mainCategoryRepo.Find(x => x.Name == request.Name);
            if (nameCheck.Any())
                throw new DuplicateNameException(nameof(MainCategoryEntity), request.Name);

            var category = _mapper.Map<MainCategoryEntity>(request);
            return await _mainCategoryRepo.Update(category);
        }
    }
}
public class MainCategoryProfile : Profile
{
    public MainCategoryProfile()
    {
        CreateMap<AddEditMainCategory, MainCategoryEntity>()
            .ForMember(c => c.Id, o => o.MapFrom(src => src.Id))
            .ForMember(c => c.Name, o => o.MapFrom(src => src.Name));
    }
}
