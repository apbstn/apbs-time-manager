﻿using Microsoft.EntityFrameworkCore;
using Shared.Context;
using Shared.DTOs.TenantDtos;
using Shared.DTOs.TenantDtos.Mappers;
using Shared.Models.Join;
using Shared.Models.Enumerations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Shared.Services;

namespace apbs_time_app.Services;

public class TenantService : ITenantService
{
    private readonly TenantDbContext _context;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ICurrentTenantService _currentTenant;

    public TenantService(TenantDbContext context, ApplicationDbContext applicationDbContext, ICurrentTenantService currentTenant)
    {
        _context = context;
        _applicationDbContext = applicationDbContext;
        _currentTenant = currentTenant;
    }

    public async Task<List<SimpleTenantDto>> GetListTenantOfUser(Guid userId)
    {
        var mapper = new TenantMapper();
        return _context.UserTenantRoles
            .Where(t => t.UserId == userId)
            .Select(t => mapper.ToSimpleTenantDto(t.Tenant))
            .ToListAsync()
            .Result;
    }

    public async Task AddUserToTenant(string email, Guid tenantId) 
    {
        var user = await _context.Users.FirstAsync(u => u.Email == email);

        await _context.UserTenantRoles.AddAsync(new UserTenantRole
        {
            UserId = user.Id,
            TenantId = tenantId,
            Role = RoleEnum.User
        });

        _context.SaveChanges();

    }
}
