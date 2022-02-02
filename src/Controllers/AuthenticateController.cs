using Microsoft.AspNetCore.Mvc;
using CashTrack.Models.AuthenticationModels;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System;
using CashTrack.Services.AuthenticationService;
using Microsoft.AspNetCore.Http;

namespace CashTrack.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticateController : Controller
    {
        private readonly IAuthenticationService _authService;

        public AuthenticateController(IAuthenticationService authService)
        {
            _authService = authService;
        }
        [HttpPost]
        public async Task<ActionResult<AuthenticationModels.Response>> Authenticate(AuthenticationModels.Request model)
        {
            try
            {
                var response = await _authService.AuthenticateAsync(model);
                if (response == null)
                {
                    return Unauthorized(new { message = "YOU DIDN'T SAY THE MAGIC WORD!" });
                }
                return Ok(response);
            }
            catch (ArgumentNullException)
            {
                return BadRequest(new { message = "Ah ah ah... you didn't say the magic words!" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
