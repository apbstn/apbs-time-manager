using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.Models;
using Shared.Services;
using System.Text;

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
            request.Password = generatePass(20);

            var result = await _userService.SetUser(request);
            return Ok(result);
        }

        //[Authorize]
        //[HttpPatch]
        //[Route("resetPass")]
        //public async Task<IActionResult> Patch(UserDto user)
        //{

        //}

        private string generatePass(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
    }
}
