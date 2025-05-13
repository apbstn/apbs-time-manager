
using Microsoft.EntityFrameworkCore;
using Shared.Context;

namespace Shared.Services;

public class CurrentTenantService : ICurrentTenantService
{
    private readonly TenantDbContext _context;

    public CurrentTenantService(TenantDbContext context)
    {
        _context = context;
    }

    public Guid? TenantId { get; set; }
    public string? ConnectionString { get; set; }

    public async Task<bool> SetTenant(Guid tenant)
    {
        try { 
            var tenantInfo = await _context.Tenants.FirstOrDefaultAsync(x => x.Id == tenant);
            if (tenantInfo != null)
            {
                TenantId = tenantInfo.Id;
                ConnectionString = tenantInfo.ConnectionString;
                return true;
            }
            else
            {
                throw new Exception("Invalid Tenant");
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }
}
