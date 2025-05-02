using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.UserDtos;
using Shared.Services;

namespace apbs_time_app.Controllers;

[ApiController]
[Route("api/auth")]
public class InvitationController : ControllerBase
{
    private IUserService _userService;

    public InvitationController(IUserService userService)
    {
        _userService = userService;
    }

    [Authorize(Roles = "Owner")]
    [HttpGet("invite")]
    public async Task<IActionResult> InviteUser(UserNoPassDto user)
    {
        if (user.Email == null)
            return Unauthorized();

        string? tenantId = Request.HttpContext.User.FindFirst("tenant_id")?.Value;

        var ss = await _userService.GetUser(user.Email);
        
        if (ss == null)
        {

        }
        else
        {

        }
        return Ok();
    }
}
