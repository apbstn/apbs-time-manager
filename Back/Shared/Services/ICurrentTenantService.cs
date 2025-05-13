namespace Shared.Services;

public interface ICurrentTenantService
{
    Guid? TenantId { get; set; }
    string? ConnectionString { get; set; }
    public Task<bool> SetTenant(Guid tenant);
}
