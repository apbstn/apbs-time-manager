using Microsoft.EntityFrameworkCore;
using Shared.Context;
using Shared.Models;
using Shared.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Shared.Services;

public class UserTenantRepository : IUserTenantRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserTenantRepository> _logger;

    public UserTenantRepository(ApplicationDbContext context, ILogger<UserTenantRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Guid?> GetIdByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        try
        {
            var userId = await _context.Users
                .Where(u => u.Email == email)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            return userId == default(Guid) ? null : userId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user ID by email: {Email}", email);
            throw;
        }
    }

    public async Task<IEnumerable<UserTenant>> GetAllAccountsAsync()
    {
        try
        {
            return await _context.Users
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all accounts");
            throw;
        }
    }
}