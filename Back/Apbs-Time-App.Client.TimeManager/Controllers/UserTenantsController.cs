using Microsoft.AspNetCore.Mvc;
using Shared.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Apbs_Time_App.Client.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserTenantsController : ControllerBase
{
    private readonly IUserTenantService _userTenantService;

    public UserTenantsController(IUserTenantService userTenantService)
    {
        _userTenantService = userTenantService;
    }

    [AllowAnonymous]
    [HttpPost("get-id-by-email")]
    public async Task<ActionResult<Guid>> GetIdByEmail([FromBody] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest("Email is required.");
        }

        var userId = await _userTenantService.GetIdByEmailAsync(email);
        if (userId == null)
        {
            return NotFound("Account with the specified email does not exist.");
        }

        return Ok(userId);
    }

    [HttpGet("accounts")]
    public async Task<ActionResult<IEnumerable<object>>> GetAllAccounts()
    {
        var accounts = await _userTenantService.GetAllAccountsAsync();
        if (accounts == null || !accounts.Any())
        {
            return NotFound("No accounts found.");
        }

        return Ok(accounts);
    }

    [HttpPut("{id}/team")]
    public async Task<IActionResult> EditUserTeam(Guid id, [FromBody] string team)
    {
        if (string.IsNullOrWhiteSpace(team))
        {
            return BadRequest("Team is required.");
        }

        var result = await _userTenantService.EditUserTeamAsync(id, team);
        if (!result)
        {
            return NotFound("User not found or update failed.");
        }

        return Ok("Team updated successfully.");
    }
}