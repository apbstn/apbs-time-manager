using Shared.DTOs.TenantDtos;

namespace apbs_time_app.Services;

public interface ITenantService
{
    public Task<List<SimpleTenantDto>> GetListTenantOfUser(Guid userId);
    public Task AddUserToTenant(string email, Guid tenantId);
}
