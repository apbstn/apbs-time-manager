using apbs_time_app.Models;
using apbs_time_app.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.UserDtos;
using Shared.DTOs.UserDtos.Mappers;
using Shared.Services;

namespace apbs_time_app.Controllers;

[ApiController]
[Route("api/auth")]
public class InvitationController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IInvitationService _invitationService;
    private readonly ITenantService _tenantService;

    public InvitationController(IUserService userService, IInvitationService invitationService, ITenantService tenantService)
    {
        _userService = userService;
        _invitationService = invitationService;
    }

    [Authorize(Roles = "Owner")]
    [HttpPost("invite")]
    public async Task<IActionResult> InviteUser(UserNoPassDto user)
    {
        if (user.Email == null)
            return Unauthorized();

        string? tenantId = Request.HttpContext.User.FindFirst("tenant_id")?.Value;

        var mapper = new UserMapper();
        var ss = await _userService.GetUser(user.Email);
        if (ss == null)
        {
            await _invitationService.CreateInviteNotExist(user, tenantId);
        }
        else
        {
            await _invitationService.CreateInviteExist(ss, tenantId);
        }
        return Ok();
    }

    [HttpGet("check/{data}")]
    public async Task<IActionResult> CheckInviteData(string data)
    {
        return Ok(_invitationService.ExtractInvitationData(data));
    }


    [HttpPost("confirm")]
    public async Task<IActionResult> ConfirmInvite(ConfirmInvite confirm)
    {
        var checkInvite = await _invitationService.CheckInvite(confirm);
        if (!checkInvite)
            return Unauthorized();

        await _tenantService.AddUserToTenant(confirm.Email, confirm.TenantId);

        return Ok();
    }
}
