using CashTrack.Common.Exceptions;
using CashTrack.Models.MerchantModels;
using CashTrack.Services.MerchantService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CashTrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MerchantsController : ControllerBase
    {
        private readonly IMerchantService _service;

        public MerchantsController(IMerchantService merchantService) => _service = merchantService;

        [HttpGet]
        public async Task<ActionResult<MerchantResponse>> GetAllMerchants([FromQuery] MerchantRequest request)
        {
            try
            {
                var response = await _service.GetMerchantsAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("match")]
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
        [HttpGet("detail/{id:int}")]
        public async Task<ActionResult<MerchantDetail>> GetMerchantDetail(int id)
        {
            try
            {
                var result = await _service.GetMerchantDetailAsync(id);
                return Ok(result);
            }
            catch (MerchantNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        public async Task<ActionResult<AddEditMerchant>> CreateMerchant([FromBody] AddEditMerchant request)
        {
            if (request.Id != null)
                return BadRequest("Request must not have an id");

            try
            {
                var result = await _service.CreateMerchantAsync(request);
                //this is location on the UI, it's missing /api from the url, which is fine the user doesn't need the api address.
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
        public async Task<ActionResult> UpdateMerchant([FromBody] AddEditMerchant request)
        {
            if (request.Id == null)
                return BadRequest("Need a merchant id to update a merchant.");
            try
            {
                var result = await _service.UpdateMerchantAsync(request);
                return Ok();
            }
            catch (DuplicateNameException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message + ex.InnerException);
            }
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteMerchant(int id)
        {
            try
            {
                var result = await _service.DeleteMerchantAsync(id);
                return Ok();
            }
            catch (MerchantNotFoundException ex)
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
