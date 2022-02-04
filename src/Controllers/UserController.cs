//using Microsoft.AspNetCore.Mvc;
//using CashTrack.Models.UserModels;
//using System.Threading.Tasks;
//using System;
//using Microsoft.AspNetCore.Http;
//using CashTrack.Common.Exceptions;
//using CashTrack.Services.UserService;

//namespace CashTrack.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class UserController : ControllerBase
//    {
//        private readonly IUserService _userService;

//        public UserController(IUserService userService)
//        {
//            this._userService = userService;
//        }
//        [HttpGet]
//        public async Task<ActionResult<UserModels.Response>> GetUserById(int id)
//        {
//            try
//            {
//                var user = await _userService.GetUserByIdAsync(id);
//                return Ok(user);
//            }
//            catch (UserNotFoundException ex)
//            {
//                return BadRequest(ex.Message);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
//            }
//        }
//        [HttpGet("all")]
//        public async Task<ActionResult<UserModels.Response[]>> GetAllUsers()
//        {
//            try
//            {
//                var users = await _userService.GetAllUsersAsync();
//                return Ok(users);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
//            }
//        }
//    }
//}