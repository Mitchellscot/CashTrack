using CashTrack.Models.MerchantModels;
using CashTrack.Models.SubCategoryModels;
using CashTrack.Services.MerchantService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CashTrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MerchantsController : ControllerBase
    {
        private readonly IMerchantService _service;

        public MerchantsController(IMerchantService merchantService) => _service = merchantService;

        [HttpGet("")]
        public async Task<ActionResult<string[]>> GetMatchingMerchants([FromQuery] string merchantName)
        {
            try
            {
                var response = await _service.GetMatchingMerchantsAsync(merchantName);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("dropdown")]
        public async Task<ActionResult<MerchantDropdownSelection[]>> GetAllMerchantsForDropDownList()
        {
            try
            {
                var merchants = await _service.GetMerchantDropdownListAsync();
                return Ok(merchants);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
