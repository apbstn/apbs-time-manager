using apbs_time_app.Models;
using apbs_time_app.Services;
using apbs_time_app.Services.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.UserDtos;
using Shared.DTOs.UserDtos.Mappers;
using Shared.Models;
using Shared.Services;

namespace apbs_time_app.Controllers;

[ApiController]
[Route("api/auth/invite")]
public class InvitationController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IInvitationService _invitationService;
    private readonly IJwtTokenGenerator _jwtGen;
    private readonly ITenantService _tenantService;

    public InvitationController(IUserService userService, IInvitationService invitationService, ITenantService tenantService, IJwtTokenGenerator jwtToken)
    {
        _userService = userService;
        _invitationService = invitationService;
        _jwtGen = jwtToken;
        _tenantService = tenantService;
    }



    [Authorize(Roles = "Owner")]
    [HttpPost]
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

    [Authorize]
    [HttpPost("confirmation")]
    public async Task<IActionResult> ConfirmInvitation([FromBody] ConfirmInvite confirmation)
    {
        if (confirmation == null || string.IsNullOrEmpty(confirmation.Token))
            return Unauthorized("Token is missing or invalid.");

        var checkInvite = await _invitationService.CheckInvite(confirmation);
        if (!checkInvite)
            return Unauthorized();

        await _tenantService.AddUserToTenant(confirmation.Email, Guid.Parse(confirmation.TenantId));

        var user = await _userService.GetUser(confirmation.Email);

        (var accessToken, var role) = await _jwtGen.GenerateAccessToken(user);

        return Ok(new
        {
            accessToken,
            role
        });
    }

    [HttpGet("{tenantId}")]
    public async Task<IActionResult> GetInvitations(string tenantId)
    {
        var invitations = await _invitationService.GetInvitationsByTenantIdAsync(tenantId);
        if (invitations == null || !invitations.Any())
        {
            return Ok("No Invitation were found!!");
        }
        return Ok(invitations);
    }

}
