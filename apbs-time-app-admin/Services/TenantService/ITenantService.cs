using Shared.Models;
using Shared.Services.Pagination;
using Shared.DTOs.UserDtos;
using Shared.DTOs.TenantDtos;

namespace apbs_time_app_admin.Services.TenantService;

public interface ITenantService
{
    Task<Tenant> CreateTenant(CreateTenantRequest request, string userId);
    Task<PaginatedResponse<ResponseTenantDto>> GetAll(int pageNumber, int pageSize = 10);
    Task<bool> UpdateTenant(Guid tenantId, UpdateTenantRequest request);
    Task<bool> DeleteTenant(Guid tenantId);

}
