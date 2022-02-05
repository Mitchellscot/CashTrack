using CashTrack.Common.Exceptions;
using CashTrack.Models.IncomeSourceModels;
using CashTrack.Services.IncomeSourceService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CashTrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IncomeSourceController : ControllerBase
    {
        private readonly IIncomeSourceService _service;

        public IncomeSourceController(IIncomeSourceService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<IncomeSourceResponse>> GetIncomeSources([FromQuery] IncomeSourceRequest request)
        {
            try
            {
                var result = await _service.GetIncomeSourcesAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        public async Task<ActionResult<AddEditIncomeSource>> CreateIncomeSource([FromBody] AddEditIncomeSource request)
        {
            if (request.Id != null)
                return BadRequest("You cannot include an id when creating an income source");

            try
            {
                var result = await _service.CreateIncomeSourceAsync(request);
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
        public async Task<ActionResult> UpdateIncomeSource([FromBody] AddEditIncomeSource request)
        {
            if (request.Id == null)
                return BadRequest("Id is required to update an income category");

            try
            {
                var result = await _service.UpdateIncomeSourceAsync(request);
                return Ok();
            }
            catch (IncomeSourceNotFoundException ex)
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
        public async Task<ActionResult> DeleteIncomeSource(int id)
        {
            try
            {
                var result = await _service.DeleteIncomeSourceAsync(id);
                return Ok();
            }
            catch (IncomeSourceNotFoundException ex)
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
