using Microsoft.AspNetCore.Mvc;
using Shared.Services;
using System.Threading.Tasks;
using System.Collections.Generic;

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
}