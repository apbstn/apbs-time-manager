using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using apbs_time_app_admin.Services.TenantService;
using apbs_time_app_admin.Services.TenantService.DTOs;
using Microsoft.AspNetCore.Authorization;
using Shared.Models.Enumerations;

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
    [Authorize(Roles = "Admin")]
    public async Task<Tenant> Post(CreateTenantRequest request)
    {

        var result = await _tenantService.CreateTenant(request);
        return result;
    }
}
