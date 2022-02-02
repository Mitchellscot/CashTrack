using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using CashTrack.Models.IncomeModels;
using System;
using CashTrack.Services.IncomeService;
using Microsoft.AspNetCore.Routing;
using CashTrack.Common.Exceptions;

namespace CashTrack.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class IncomeController : ControllerBase
    {
        private readonly IIncomeService _service;
        public IncomeController(IIncomeService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<IncomeResponse>> GetIncomes([FromQuery] IncomeRequest request)
        {
            try
            {
                var response = await _service.GetIncomeAsync(request);
                return response;
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message.ToString() });
            }
        }
        [HttpGet("detail/{id:int}")]
        public async Task<ActionResult<IncomeListItem>> GetIncomeDetail(int id)
        {
            try
            {
                var result = await _service.GetIncomeByIdAsync(id);
                return Ok(result);
            }
            catch (IncomeNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        public async Task<ActionResult<AddEditIncome>> CreateIncome(AddEditIncome request)
        {
            try
            {
                var result = await _service.CreateIncomeAsync(request);
                return CreatedAtAction("detail", new { id = result.Id.Value }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message + ex.InnerException);
            }
        }
        [HttpPut]
        public async Task<ActionResult<AddEditIncome>> UpdateIncome([FromBody] AddEditIncome request)
        {
            if (request.Id == null)
                return BadRequest("Need an Id to update an income.");

            try
            {
                var result = await _service.UpdateIncomeAsync(request);
                return Ok();
            }
            catch (IncomeNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message + ex.InnerException);
            }
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteIncome(int id)
        {
            try
            {
                var result = await _service.DeleteIncomeAsync(id);
                if (!result)
                    return StatusCode(StatusCodes.Status500InternalServerError, "Unable to delete Income");

                return Ok();
            }
            catch (IncomeNotFoundException ex)
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
