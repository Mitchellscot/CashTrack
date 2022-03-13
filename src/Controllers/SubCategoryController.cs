using CashTrack.Models.SubCategoryModels;
using CashTrack.Services.SubCategoryService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CashTrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubCategoryController : ControllerBase
    {
        private readonly ISubCategoryService _subCategoryService;

        public SubCategoryController(ISubCategoryService subCategoryService) => _subCategoryService = subCategoryService;

        [HttpGet]
        public async Task<ActionResult<Dictionary<int, string>>> GetAllSubCategoriesForDropDownList()
        {
            try
            {
                var categories = await _subCategoryService.GetAllSubCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
