using CashTrack.Services.BudgetService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using CashTrack.Models.BudgetModels;

namespace CashTrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BudgetController : ControllerBase
    {
        private readonly IBudgetService _budgetService;
        public BudgetController(IBudgetService budgetService) => (_budgetService) = (budgetService);
        [HttpGet("averages-and-totals/{subCategoryId:int}")]
        public async Task<ActionResult<CategoryAveragesAndTotals>> GetAmountsForBudgetAllocationModal(int subCategoryId)
        {
            try
            {
                var result = await _budgetService.GetCategoryAveragesAndTotals(subCategoryId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
