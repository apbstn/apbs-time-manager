using Shared.Models;
using apbs_time_app_admin.Services.TenantService.DTOs;
using Shared.DTOs;

namespace apbs_time_app_admin.Services.TenantService;

public interface ITenantService
{
    Task<Tenant> CreateTenant(CreateTenantRequest request, User user);
    Task<IEnumerable<ResponseTenantDto>> GetAll();
    Task<bool> UpdateTenant(Guid tenantId, UpdateTenantRequest request);
    Task<bool> DeleteTenant(Guid tenantId);

}
