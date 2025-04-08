using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using apbs_time_app_admin.Services.TenantService;
using apbs_time_app_admin.Services.TenantService.DTOs;
using Microsoft.AspNetCore.Authorization;
using Shared.Services;
using Shared.DTOs;
using System.Text;

namespace apbs_time_app_admin.Controllers;

[Route("api/tenant")]
[ApiController]
public class TenantsController : ControllerBase
{
    private readonly ITenantService _tenantService;

    public TenantsController(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Post(CreateTenantRequest request)
    {
        var result = await _tenantService.CreateTenant(request, request.User);
        return Ok(new { result });
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll(PaginationParameters param)
    {
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
}
