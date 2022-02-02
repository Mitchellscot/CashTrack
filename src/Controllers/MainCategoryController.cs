using CashTrack.Models.MainCategoryModels;
using CashTrack.Services.MainCategoriesService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using CashTrack.Common.Exceptions;

namespace CashTrack.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MainCategoryController : ControllerBase
    {
        private readonly IMainCategoriesService _service;
        public MainCategoryController(IMainCategoriesService mainCategoryService) => _service = mainCategoryService;

        [HttpGet]
        public async Task<ActionResult<MainCategoryResponse>> GetMainCategories([FromQuery] MainCategoryRequest request)
        {
            try
            {
                var result = await _service.GetMainCategoriesAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        public async Task<ActionResult<AddEditMainCategory>> CreateMainCategory([FromBody] AddEditMainCategory request)
        {
            if (request.Id != null)
                return BadRequest("Cannot have an id in the request to create a new main category.");

            try
            {
                var result = await _service.CreateMainCategoryAsync(request);
                return CreatedAtAction("detail", new { id = result.Id }, result);
            }
            catch (DuplicateNameException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPut]
        public async Task<ActionResult> UpdateMainCategory([FromBody] AddEditMainCategory request)
        {
            if (request.Id == null)
                return BadRequest("Main Category ID must not be null");

            try
            {
                var result = await _service.UpdateMainCategoryAsync(request);
                return Ok();
            }
            catch (DuplicateNameException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (CategoryNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteMainCategory(int id)
        {
            try
            {
                var result = await _service.DeleteMainCategoryAsync(id);
                return Ok();
            }
            catch (CategoryNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
