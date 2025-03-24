using Shared.Models;
using apbs_time_app_admin.Services.TenantService.DTOs;

namespace apbs_time_app_admin.Services.TenantService;

public interface ITenantService
{
    Task<Tenant> CreateTenant(CreateTenantRequest request);
}
