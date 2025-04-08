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
    private readonly IUserService _userService;

    public TenantsController(ITenantService tenantService, IUserService userService)
    {
        _tenantService = tenantService;
        _userService = userService;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Post(CreateTenantRequest request)
    {
        var user = new UserDto
        {
            Email = request.Email,
            Username = request.Username,
            PhoneNumber = request.PhoneNumber
        };
        var resultUser = await _userService.SetUser(user);
        var result = await _tenantService.CreateTenant(request, resultUser);
        return Ok(new { result });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _tenantService.GetAll();
        return Ok(result);
    }

    // === NEW: PUT - Update Tenant ===
    [Authorize(Roles = "Admin")]
    [HttpPut("{tenantId}")]
    public async Task<IActionResult> Put(Guid tenantId, [FromBody] UpdateTenantRequest request)
    {
        var result = await _tenantService.UpdateTenant(tenantId, request);
        if (!result)
            return NotFound(new { message = "Tenant not found or update failed." });

        return Ok(new { message = "Tenant updated successfully." });
    }

    // === NEW: DELETE - Delete Tenant ===
    [Authorize(Roles = "Admin")]
    [HttpDelete("{tenantId}")]
    public async Task<IActionResult> Delete(Guid tenantId)
    {
        var result = await _tenantService.DeleteTenant(tenantId);
        if (!result)
            return NotFound(new { message = "Tenant not found or delete failed." });

        return Ok(new { message = "Tenant deleted successfully." });
    }

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
