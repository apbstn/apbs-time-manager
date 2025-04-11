using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.UserDtos;
using Shared.Models;
using Shared.Services;
using Shared.Services.Generator;

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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll(PaginationParameters param) 
        { 
            var list = await _userService.GetUsers(param.PageNumber, param.PageSize);
            return Ok(list);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post(UserDto request) 
        {
            request.Password = PassGenerator.GeneratePass(20);

            var result = await _userService.SetUser(request);
            return Ok(result);
        }

        [Authorize]
        [HttpPatch]
        [Route("resetPass")]
        public async Task<IActionResult> Patch(UserDto request)
        {
            request.Password = PassGenerator.GeneratePass(20);

            var result = await _userService.ResetPass(request);

            return Ok(result);
        }
    }
}
