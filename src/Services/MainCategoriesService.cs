using CashTrack.Data.Entities;
using CashTrack.Common.Exceptions;
using CashTrack.Models.MainCategoryModels;
using CashTrack.Repositories.MainCategoriesRepository;
using CashTrack.Repositories.SubCategoriesRepository;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CashTrack.Services.MainCategoriesService
{
    public interface IMainCategoriesService
    {
        Task<MainCategoryResponse> GetMainCategoriesAsync(MainCategoryRequest request);
        Task<MainCategoryDetail> GetMainCategoryDetailAsync(int id);
        Task<string> GetMainCategoryNameBySubCategoryIdAsync(int id);
        Task<int> CreateMainCategoryAsync(AddEditMainCategory request);
        Task<MainCategoryDropdownSelection[]> GetMainCategoriesForDropdownListAsync();
        Task<int> UpdateMainCategoryAsync(AddEditMainCategory request);
        Task<bool> DeleteMainCategoryAsync(int id);
    }
    public class MainCategoriesService : IMainCategoriesService
    {
        private readonly IMainCategoriesRepository _mainCategoryRepo;
        private readonly ISubCategoryRepository _subCategoryRepository;

        public MainCategoriesService(IMainCategoriesRepository mainCategoryRepository, ISubCategoryRepository subCategoryRepository)
        {
            _mainCategoryRepo = mainCategoryRepository;
            _subCategoryRepository = subCategoryRepository;
        }

        public async Task<int> CreateMainCategoryAsync(AddEditMainCategory request)
        {
            var categories = await _mainCategoryRepo.Find(x => true);
            if (categories.Any(x => x.Name == request.Name))
                throw new DuplicateNameException(nameof(MainCategoryEntity), request.Name);

            var categoryEntity = new MainCategoryEntity()
            {
                Name = request.Name,
            };

            return await _mainCategoryRepo.Create(categoryEntity);
        }

        public async Task<bool> DeleteMainCategoryAsync(int id)
        {
            var category = await _mainCategoryRepo.FindById(id);
            if (category == null)
                throw new CategoryNotFoundException(id.ToString());

            if (category.Name == "Other")
                throw new Exception("You cannot delete this category. It's kind of important.");
            var otherCategory = (await _mainCategoryRepo.Find(x => x.Name == "Other")).FirstOrDefault();
            if (otherCategory == null)
            {
                throw new CategoryNotFoundException("You need to create a new category called 'Other' before you can delete a sub category. This will assigned all exepenses associated with this main category to the 'Other' category.");
            }
            var subCategories = await _subCategoryRepository.Find(x => x.MainCategoryId == id);
            foreach (var subCategory in subCategories)
            {
                subCategory.MainCategoryId = otherCategory.Id;
            }

            return await _mainCategoryRepo.Delete(category);
        }

        public async Task<MainCategoryResponse> GetMainCategoriesAsync(MainCategoryRequest request)
        {
            var categories = await _mainCategoryRepo.Find(x => true);

            var listItems = await _mainCategoryRepo.GetMainCategoryListItems();

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
            var categories = await _mainCategoryRepo.Find(x => x.Name == request.Name);
            if (categories.Any(x => x.Id != request.Id))
                throw new DuplicateNameException(request.Name, nameof(MainCategoryEntity));

            var category = await _mainCategoryRepo.FindById(request.Id.Value);
            if (category == null)
                throw new CategoryNotFoundException(request.Id.Value.ToString());

            category.Name = request.Name;
            return await _mainCategoryRepo.Update(category);
        }
    }
}
