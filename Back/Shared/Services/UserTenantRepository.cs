using Microsoft.EntityFrameworkCore;
using Shared.Context;
using Shared.Models;
using Shared.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shared.Repositories;

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

    public async Task<UserTenant> GetByIdAsync(Guid id)
    {
        try
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user by ID: {Id}", id);
            throw;
        }
    }

    public async Task<bool> UpdateUserAsync(UserTenant userTenant)
    {
        try
        {
            _context.Users.Update(userTenant);
            var rowsAffected = await _context.SaveChangesAsync();
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user: {Id}", userTenant.Id);
            throw;
        }
    }

    public async Task<Team> GetTeamByNameAsync(string name)
    {
        try
        {
            return await _context.Teams
                .FirstOrDefaultAsync(t => t.Name == name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving team by name: {Name}", name);
            throw;
        }
    }

    public async Task AddTeamAsync(Team team)
    {
        try
        {
            await _context.Teams.AddAsync(team);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding team: {Name}", team.Name);
            throw;
        }
    }
}