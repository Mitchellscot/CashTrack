using CashTrack.Common.Exceptions;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Services.SubCategoryService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
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
        public async Task<ActionResult<SubCategoryResponse>> GetAllSubCategories([FromQuery] SubCategoryRequest request)
        {
            try
            {
                var categories = await _subCategoryService.GetSubCategoriesAsync(request);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        public async Task<ActionResult<AddEditSubCategory>> CreateSubCategory(AddEditSubCategory request)
        {
            if (request.Id != null)
                return BadRequest("Cannot have an ID when creating a sub category");

            try
            {
                var result = await _subCategoryService.CreateSubCategoryAsync(request);
                return CreatedAtAction($"detail", new { id = result.Id }, result);
            }
            catch (DuplicateNameException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message + ex.InnerException);

            }
        }
        [HttpPut]
        public async Task<ActionResult> UpdateSubCategory(AddEditSubCategory request)
        {
            try
            {
                if (!await _subCategoryService.UpdateSubCategoryAsync(request))
                    return StatusCode(StatusCodes.Status500InternalServerError, "Unable to update category");

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
        public async Task<ActionResult> DeleteSubCategory(int id)
        {
            try
            {
                var result = await _subCategoryService.DeleteSubCategoryAsync(id);
                if (!result)
                    return StatusCode(StatusCodes.Status500InternalServerError, "Unable to delete category");

                return Ok();
            }
            catch (CategoryNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message + ex.InnerException);
            }
        }
    }
}
