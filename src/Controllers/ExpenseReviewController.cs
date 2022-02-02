using CashTrack.Common.Exceptions;
using CashTrack.Models.ExpenseReviewModels;
using CashTrack.Services.ExpenseReviewService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CashTrack.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/expense-reviews")]
    public class ExpenseReviewController : ControllerBase
    {
        private readonly IExpenseReviewService _service;

        public ExpenseReviewController(IExpenseReviewService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<ExpenseReviewResponse>> GetAllExpenseReviews([FromQuery] ExpenseReviewRequest request)
        {
            try
            {
                var response = await _service.GetExpenseReviewsAsync(request);
                return response;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("detail/{id:int}")]
        public async Task<ActionResult<ExpenseReviewListItem>> GetExpenseDetail(int id)
        {
            try
            {
                var result = await _service.GetExpenseReviewByIdAsync(id);
                return Ok(result);
            }
            catch (ExpenseNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateExpense(int id)
        {
            try
            {
                var result = await _service.UpdateExpenseReviewAsync(id);
                return Ok();
            }
            catch (ExpenseNotFoundException ex)
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
