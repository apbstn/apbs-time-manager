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
    private readonly ILogger<UserTenantsController>? _logger;

    public UserTenantsController(IUserTenantService userTenantService, ILogger<UserTenantsController>? logger = null)
    {
        _userTenantService = userTenantService;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpPost("get-id-by-email")]
    public async Task<ActionResult<Guid>> GetIdByEmail([FromBody] string email)
    {
        _logger?.LogInformation("Received POST request for get-id-by-email with email {Email}", email);
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
        _logger?.LogInformation("Received GET request for accounts");
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
        _logger?.LogInformation("Received PUT request to edit team for user ID {UserId} with team {Team}", id, team);
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

    [HttpPut("remove-team/{id}")]
    public async Task<IActionResult> RemoveUserFromTeam(Guid id)
    {
        _logger?.LogInformation("Entered RemoveUserFromTeam with ID {UserId}", id);
        if (id == Guid.Empty)
        {
            _logger?.LogWarning("User ID is empty");
            return BadRequest(new { Message = "User ID is required." });
        }

        try
        {
            _logger?.LogInformation("Calling RemoveUserFromTeamAsync for user ID {UserId}", id);
            var result = await _userTenantService.RemoveUserFromTeamAsync(id);
            if (!result)
            {
                _logger?.LogWarning("RemoveUserFromTeamAsync returned false for user ID {UserId}", id);
                return NotFound(new { Message = "User not found or not assigned to a team." });
            }
            _logger?.LogInformation("User with ID {UserId} removed from team successfully", id);
            return Ok(new { Message = $"User with ID {id} removed from team successfully." });
        }
        catch (InvalidOperationException ex)
        {
            _logger?.LogWarning("InvalidOperationException: {Message}", ex.Message);
            return StatusCode(500, new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error removing user with ID {UserId} from team", id);
            return StatusCode(500, new { Message = "An error occurred while removing the user from the team." });
        }
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        _logger?.LogInformation("Received DELETE request for user ID {UserId}", id);
        try
        {
            var deletedUser = await _userTenantService.DeleteUsers(id);
            return Ok(new { Message = $"User with ID {id} deleted successfully.", User = deletedUser });
        }
        catch (InvalidOperationException ex)
        {
            _logger?.LogWarning("InvalidOperationException: {Message}", ex.Message);
            return NotFound(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error deleting user with ID {UserId}", id);
            return StatusCode(500, new { Message = "An error occurred while deleting the user." });
        }
    }

    [AllowAnonymous]
    [HttpGet("test")]
    public IActionResult Test()
    {
        _logger?.LogInformation("Test endpoint hit");
        return Ok("Test endpoint reached");
    }
}