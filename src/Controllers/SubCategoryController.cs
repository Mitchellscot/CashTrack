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
        public async Task<ActionResult<SubCategoryDropdownSelection[]>> GetAllSubCategoriesForDropDownList()
        {
            try
            {
                var categories = await _subCategoryService.GetSubCategoryDropdownListAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("autocomplete")]
        public async Task<ActionResult<string[]>> GetMatchingSubCategoryNames([FromQuery] string categoryName)
        {
            try
            {
                var response = await _subCategoryService.GetMatchingSubCategoryNamesAsync(categoryName);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}

