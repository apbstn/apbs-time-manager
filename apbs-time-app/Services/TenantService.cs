using Microsoft.EntityFrameworkCore;
using Shared.Context;
using Shared.DTOs.TenantDtos;
using Shared.DTOs.TenantDtos.Mappers;

namespace apbs_time_app.Services;

public class TenantService : ITenantService
{
    private readonly TenantDbContext _context;

    public TenantService(TenantDbContext context)
    {
        _context = context;
    }

    public async Task<List<SimpleTenantDto>> GetListTenantOfUser(string userId)
    {
        var mapper = new TenantMapper();
        return _context.UserTenantRoles
            .Where(t => t.UserId == userId)
            .Select(t => mapper.ToSimpleTenantDto(t.Tenant))
            .ToListAsync()
            .Result;
    }
}
