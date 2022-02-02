using CashTrack.Common.Exceptions;
using CashTrack.Models.IncomeReviewModels;
using CashTrack.Services.IncomeReviewService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CashTrack.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/income-reviews")]
    public class IncomeReviewController : ControllerBase
    {
        private readonly IIncomeReviewService _service;

        public IncomeReviewController(IIncomeReviewService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<IncomeReviewResponse>> GetAllIncomeReviews([FromQuery] IncomeReviewRequest request)
        {
            try
            {
                var response = await _service.GetIncomeReviewsAsync(request);
                return response;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("detail/{id:int}")]
        public async Task<ActionResult<IncomeReviewListItem>> GetIncomeDetail(int id)
        {
            try
            {
                var result = await _service.GetIncomeReviewByIdAsync(id);
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
        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateIncome(int id)
        {
            try
            {
                var result = await _service.UpdateIncomeReviewAsync(id);
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
