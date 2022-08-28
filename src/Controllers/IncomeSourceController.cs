using CashTrack.Models.IncomeSourceModels;
using CashTrack.Models.MerchantModels;
using CashTrack.Services.IncomeSourceService;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<ActionResult<string[]>> GetIncomeSources([FromQuery] string sourceName)
        {
            try
            {
                var result = await _service.GetMatchingIncomeSourcesAsync(sourceName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("dropdown")]
        public async Task<ActionResult<SourceDropdownSelection[]>> GetAllSourcesForDropDownList()
        {
            try
            {
                var sources = await _service.GetSourceDropdownListAsync();
                return Ok(sources);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
