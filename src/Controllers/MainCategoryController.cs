using CashTrack.Models.MainCategoryModels;
using CashTrack.Services.MainCategoriesService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace CashTrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MainCategoryController : ControllerBase
    {
        private readonly IMainCategoriesService _service;
        public MainCategoryController(IMainCategoriesService mainCategoryService) => _service = mainCategoryService;

        [HttpGet]
        public async Task<ActionResult<MainCategoryDropdownSelection[]>> GetMainCategoriesForDropdownList()
        {
            try
            {
                var result = await _service.GetMainCategoriesForDropdownListAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("sub-category/{Id:int}")]
        public async Task<ActionResult<string>> GetMainCategoryNameBySubCategoryId(int id)
        {
            try
            {
                var result = await _service.GetMainCategoryNameBySubCategoryIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
