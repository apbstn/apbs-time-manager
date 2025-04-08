using Shared.Models;
using apbs_time_app_admin.Services.TenantService.DTOs;
using Shared.DTOs;
using Shared.Services.Pagination;

namespace apbs_time_app_admin.Services.TenantService;

public interface ITenantService
{
    Task<Tenant> CreateTenant(CreateTenantRequest request, UserDto user);
    Task<PaginatedResponse<ResponseTenantDto>> GetAll(int pageNumber, int pageSize = 10);
    Task<bool> UpdateTenant(Guid tenantId, UpdateTenantRequest request);
    Task<bool> DeleteTenant(Guid tenantId);

}
