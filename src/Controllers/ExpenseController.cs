using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using CashTrack.Models.ExpenseModels;
using System;
using CashTrack.Common.Exceptions;
using CashTrack.Services.ExpenseService;
using Microsoft.AspNetCore.Routing;

namespace CashTrack.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpenseController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        [HttpGet]
        public async Task<ActionResult<ExpenseResponse>> GetAllExpenses([FromQuery] ExpenseRequest request)
        {
            try
            {
                var response = await _expenseService.GetExpensesAsync(request);
                return response;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("detail/{id:int}")]
        public async Task<ActionResult<ExpenseListItem>> GetExpenseDetail(int id)
        {
            try
            {
                var result = await _expenseService.GetExpenseByIdAsync(id);
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

        [HttpGet("notes")]
        public async Task<ActionResult<ExpenseResponse>> GetExpensesByNotes([FromQuery] ExpenseRequest request)
        {
            if (string.IsNullOrEmpty(request.Query))
            {
                return BadRequest("This endpoint requres a search term");
            }

            try
            {
                var response = await _expenseService.GetExpensesByNotesAsync(request);
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("amount")]
        public async Task<ActionResult<ExpenseResponse>> GetExpensesByAmount([FromQuery] AmountSearchRequest request)
        {
            if (string.IsNullOrEmpty(request.Query.ToString()))
            {
                return BadRequest("This endpoint requres a search term");
            }

            try
            {
                var response = await _expenseService.GetExpensesByAmountAsync(request);
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<AddEditExpense>> CreateExpense(AddEditExpense request)
        {
            try
            {
                var result = await _expenseService.CreateExpenseAsync(request);
                return CreatedAtAction("detail", new { id = result.Id.Value }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message + ex.InnerException);
            }
        }
        [HttpPut]
        public async Task<ActionResult<AddEditExpense>> UpdateExpense([FromBody] AddEditExpense request)
        {
            if (request.Id == null)
                return BadRequest("Need an Id to update an expense.");

            try
            {
                var result = await _expenseService.UpdateExpenseAsync(request);
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
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteExpense(int id)
        {
            try
            {
                var result = await _expenseService.DeleteExpenseAsync(id);
                if (!result)
                    return StatusCode(StatusCodes.Status500InternalServerError, "Unable to delete expense");

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

        //    //api/expense/merchant/{merchantId}
        //    //accepts an int indicating the merchant Id
        //    //returns all transactions for that merchant, sorted by date
        //    [HttpGet("/merchant/{merchantId}")]
        //    //might need a custom model for this response instead of the standard ExpenseModel
        //    public async Task<ActionResult<ExpeseModel[]>> GetExpensesByMerchant(int merchantId)
        //    {
        //        try
        //        {
        //            //int.TryParse(amount)...
        //            //logic goes here
        //            return Content("This is the data you were looking for.");
        //        }
        //        catch (System.Exception ex)
        //        {
        //            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //        }
        //    }

        //    //api/expense/category
        //    //returns transactions from a given category
        //    [HttpGet("/category/{categoryId}")]
        //    public async Task<ActionResult<ExpenseModel[]>> GetcategoryExpenses(int categoryId)
        //    {
        //        try
        //        {
        //            //logic goes here
        //            return Content("This is the data you were looking for.");
        //        }
        //        catch (System.Exception ex)
        //        {
        //            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //        }
        //    }


        //    //api/expense/tags/{tagId}
        //    //Accepts a tag id
        //    //returns all transactions that contain that tag Id
        //    [HttpGet("/tags/[tagId]")]
        //    //create a Tag Request model that has a tag Id
        //    public async Task<ActionResult<ExpenseModel[]>> GetExpensesByTag(int tagId)
        //    {
        //        try
        //        {
        //            //logic goes here
        //            return Content("This is the data you were looking for.");
        //        }
        //        catch (System.Exception ex)
        //        {
        //            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //        }
        //    }

    }
}
