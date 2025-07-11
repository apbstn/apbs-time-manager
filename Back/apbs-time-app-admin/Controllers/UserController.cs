using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.UserDtos;
using Shared.Models;
using Shared.Services;
using Shared.Services.Generator;

namespace apbs_time_app_admin.Controllers;

[Authorize]
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
    public async Task<IActionResult> GetAll(/*PaginationParameters param*/)
    {
        var list = await _userService.GetUsers(/*param.PageNumber, param.PageSize*/);
        return Ok(list);
    }


    [HttpPost]
    public async Task<IActionResult> Post(UserDto request)
    {
        request.Password = PassGenerator.GeneratePass(20);

        var result = await _userService.SetUser(request);
        return Ok(result);
    }


    [HttpPatch]
    [Route("resetPass")]
    public async Task<IActionResult> Patch(UserDto request)
    {
        request.Password = PassGenerator.GeneratePass(20);

        var result = await _userService.ResetPass(request);

        return Ok(new { result, request.Password });
    }
    [HttpDelete]
    [Route("delete/{userId}")] // Use route parameter instead of query parameter
    public async Task<IActionResult> Delete(Guid userId)
    {
        var result = await _userService.DeleteUser(userId);
        if (result == null)
        {
            return NotFound("tried to find that user with the given id but didn't found it");
        }
        return Ok("User Has Been Deleted with success");
    }

    [HttpPut]
    [Route("edit/{userId}")]
    public async Task<IActionResult> Edit(Guid userId, [FromBody] User updatedUser)
    {
        if (updatedUser == null)
        {
            return BadRequest("User data is required in the request body.");
        }

        var result = await _userService.EditUser(userId, updatedUser);
        if (result == null)
        {
            return NotFound("tried to find that user with the given id but didn't found it");
        }

        // Map to DTO to avoid serialization issues
        var userDto = new UserDto
        {
            Email = result.Email,
            Username = result.Username,
            PhoneNumber = result.PhoneNumber
        };

        return Ok(userDto);
    }
}
