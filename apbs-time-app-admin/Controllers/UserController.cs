using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.Services;

namespace apbs_time_app_admin.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Get() 
        { 
            var list = await _userService.GetUsers();
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Post(UserDto request) 
        { 
            var result = await _userService.SetUser(request);
            return Ok(result);
        }
    }
}
