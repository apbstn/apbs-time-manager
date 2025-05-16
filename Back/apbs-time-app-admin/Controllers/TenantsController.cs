using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using apbs_time_app_admin.Services.TenantService;
using Microsoft.AspNetCore.Authorization;
using Shared.DTOs.TenantDtos;
using Shared.DTOs.TenantDtos.Mappers;

namespace apbs_time_app_admin.Controllers;

[Route("api/tenant")]
[ApiController]
public class TenantsController : ControllerBase
{
    private readonly ITenantService _tenantService;
    private readonly ILogger<TenantsController> _logger;

    public TenantsController(ITenantService tenantService, ILogger<TenantsController> logger)
    {
        _tenantService = tenantService;
        _logger = logger;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Post(CreateTenantRequest request)
    {
        var result = await _tenantService.CreateTenant(request, request.User);
        return Ok(new TenantMapper().ToResponseTenantDto(result));
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll(PaginationParameters param)
    {
        _logger.LogInformation("ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ");
        var result = await _tenantService.GetAll(param.PageNumber, param.PageSize);
        return Ok(result);
    }

    // === NEW: PUT - Update Tenant ===
    [Authorize]
    [HttpPut("{tenantId}")]
    public async Task<IActionResult> Put(Guid tenantId, [FromBody] UpdateTenantRequest request)
    {
        var result = await _tenantService.UpdateTenant(tenantId, request);
        if (!result)
            return NotFound(new { message = "Tenant not found or update failed." });

        return Ok(new { message = "Tenant updated successfully." });
    }

    // === NEW: DELETE - Delete Tenant ===
    [Authorize]
    [HttpDelete("{tenantId}")]
    public async Task<IActionResult> Delete(Guid tenantId)
    {
        var result = await _tenantService.DeleteTenant(tenantId);
        if (!result)
            return NotFound(new { message = "Tenant not found or delete failed." });

        return Ok(new { message = "Tenant deleted successfully." });
    }

    [Authorize]
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetTenantsByUserId(Guid userId)
    {
        try
        {
            // Optional: Restrict to authenticated user's own tenants
            // var authenticatedUserId = Guid.Parse(User.FindFirstValue("sub") ?? string.Empty);
            // if (userId != authenticatedUserId) return Unauthorized("You can only access your own tenants.");

            var tenants = await _tenantService.GetTenantsByUserIdAsync(userId);
            if (tenants == null || !tenants.Any())
            {
                return NotFound(new { message = $"No tenants found for user ID {userId}" });
            }

            var tenantDtos = tenants.Select(t => new TenantMapper().ToResponseTenantDto(t)).ToList();
            return Ok(tenantDtos);
        }
        catch (Exception ex)
        {
            // In production, use a logging framework like Serilog
            Console.WriteLine($"Error fetching tenants for user ID {userId}: {ex.Message}");
            return StatusCode(500, new { message = "An error occurred while retrieving tenants." });
        }
    }
}
