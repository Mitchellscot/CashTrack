using CashTrack.Common.Exceptions;
using CashTrack.Models.IncomeCategoryModels;
using CashTrack.Services.IncomeCategoryService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CashTrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IncomeCategoryController : ControllerBase
    {
        private readonly IIncomeCategoryService _service;
        public IncomeCategoryController(IIncomeCategoryService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<IncomeCategoryResponse>> GetIncomeCategories([FromQuery] IncomeCategoryRequest request)
        {
            try
            {
                var result = await _service.GetIncomeCategoriesAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        public async Task<ActionResult<AddEditIncomeCategory>> CreateIncomeCategory([FromBody] AddEditIncomeCategory request)
        {
            if (request.Id != null)
                return BadRequest("You cannot include an id when creating an income category");

            try
            {
                var result = await _service.CreateIncomeCategoryAsync(request);
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
        public async Task<ActionResult> UpdateIncomeCategory([FromBody] AddEditIncomeCategory request)
        {
            if (request.Id == null)
                return BadRequest("Id is required to update an income category");

            try
            {
                var result = await _service.UpdateIncomeCategoryAsync(request);
                return Ok();
            }
            catch (CategoryNotFoundException ex)
            {
                return BadRequest(ex.Message);
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
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteIncomeCategory(int id)
        {
            try
            {
                var result = await _service.DeleteIncomeCategoryAsync(id);
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
