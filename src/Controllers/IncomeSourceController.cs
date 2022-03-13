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
            //this will return a list of strings when I get to it
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
    }
}
