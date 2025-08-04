using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Context;
using Shared.DTOs.UserDtos;
using Shared.DTOs.UserDtos.Mappers;
using Shared.Models;
using Shared.Repositories;
using Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Services;

public class UserTenantService : IUserTenantService
{
    private readonly IUserTenantRepository _repository;
    private readonly ApplicationDbContext _appContext;
    private readonly TenantDbContext _tenantDbContext;
    private readonly ILogger<UserTenantService>? _logger;

    public UserTenantService(IUserTenantRepository repository, ApplicationDbContext appContext, TenantDbContext tenantDbContext, ILogger<UserTenantService>? logger = null)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
        _tenantDbContext = tenantDbContext ?? throw new ArgumentNullException(nameof(tenantDbContext));
        _logger = logger;
    }

    public async Task<Guid?> GetIdByEmailAsync(string email)
    {
        return await _repository.GetIdByEmailAsync(email);
    }

    public async Task<IEnumerable<object>> GetAllAccountsAsync()
    {
        var accounts = await _repository.GetAllAccountsAsync();
        var results = new List<UserTenantDto>();
        var _mapper = new UserMapper();
        foreach (var account in accounts)
        {
            results.Add(_mapper.ToUserDto(account));
        }
        return results;
    }

    public async Task<bool> EditUserTeamAsync(Guid id, string team)
    {
        var userTenant = await _repository.GetByIdAsync(id);
        if (userTenant == null)
        {
            return false;
        }

        // Find or create a Team
        var existingTeam = await _repository.GetTeamByNameAsync(team);
        if (existingTeam == null)
        {
            var newTeam = new Team { Name = team, Description = $"Team {team} description" }; // Default description
            await _repository.AddTeamAsync(newTeam);
            userTenant.TeamId = newTeam.Id;
        }
        else
        {
            userTenant.TeamId = existingTeam.Id;
        }

        return await _repository.UpdateUserAsync(userTenant);
    }

    public async Task<bool> RemoveUserFromTeamAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            _logger?.LogWarning("User ID cannot be empty.");
            throw new ArgumentException("User ID cannot be empty.", nameof(id));
        }

        using var transaction = await _appContext.Database.BeginTransactionAsync();
        try
        {
            _logger?.LogInformation("Attempting to remove user with ID {UserId} from their team.", id);

            var user = await _repository.GetByIdAsync(id);
            if (user == null)
            {
                _logger?.LogWarning("User with ID {UserId} not found.", id);
                return false;
            }

            if (user.TeamId == null)
            {
                _logger?.LogInformation("User with ID {UserId} is not assigned to a team, no action needed.", id);
                return false;
            }

            user.TeamId = null;
            var result = await _repository.UpdateUserAsync(user);
            if (result)
            {
                _logger?.LogInformation("Removed user with ID {UserId} from team.", id);
                await transaction.CommitAsync();
                return true;
            }
            else
            {
                _logger?.LogWarning("Failed to update user with ID {UserId} to remove team.", id);
                await transaction.RollbackAsync();
                return false;
            }
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger?.LogError(ex, "Failed to remove user with ID {UserId} from team.", id);
            throw new InvalidOperationException($"Failed to remove user with ID {id} from team: {ex.Message}", ex);
        }
    }

    public async Task<UserTenant> DeleteUsers(Guid id)
    {
        if (_appContext == null)
            throw new InvalidOperationException("Database context is not initialized.");

        using var transaction = await _appContext.Database.BeginTransactionAsync();
        try
        {
            _logger?.LogInformation("Attempting to delete user with ID {UserId}.", id);

            var user = await _appContext.Users.FindAsync(id);
            var accountemail = await _appContext.Users
                .Where(a => a.Id == id)
                .Select(a => a.Email)
                .FirstOrDefaultAsync();
            string emaill = accountemail;
            if (user == null)
            {
                _logger?.LogWarning("User with ID {UserId} not found.", id);
                throw new InvalidOperationException($"User with ID {id} not found.");
            }

            var leaveRequest = await _appContext.LeaveRequests
                .FirstOrDefaultAsync(lr => lr.UserId == id);
            if (leaveRequest != null)
            {
                _appContext.LeaveRequests.Remove(leaveRequest);
                _logger?.LogInformation("Deleted LeaveRequest for user ID {UserId}.", id);
            }
            else
            {
                _logger?.LogInformation("No LeaveRequest found for user ID {UserId}, skipping deletion.", id);
            }

            var leaveBalance = await _appContext.LeaveBalances
                .FirstOrDefaultAsync(lb => lb.UserId == id);
            if (leaveBalance != null)
            {
                _appContext.LeaveBalances.Remove(leaveBalance);
                _logger?.LogInformation("Deleted LeaveBalance for user ID {UserId}.", id);
            }
            else
            {
                _logger?.LogInformation("No LeaveBalance found for user ID {UserId}, skipping deletion.", id);
            }

            var timeLogsCount = await _appContext.TimeLogs
                .Where(tl => tl.UserId == id)
                .CountAsync();
            if (timeLogsCount > 0)
            {
                await _appContext.TimeLogs
                    .Where(tl => tl.UserId == id)
                    .ExecuteDeleteAsync();
                _logger?.LogInformation("Deleted {Count} TimeLogs for user ID {UserId}.", timeLogsCount, id);
            }
            else
            {
                _logger?.LogInformation("No TimeLogs found for user ID {UserId}, skipping deletion.", id);
            }

            _appContext.Users.Remove(user);

            _logger?.LogInformation("Deleted user with ID {UserId}.", id);

            await _appContext.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger?.LogInformation("User with ID {UserId} deleted successfully.", id);
            return user;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger?.LogError(ex, "Failed to delete user with ID {UserId}.", id);
            throw new InvalidOperationException($"Failed to delete user with ID {id}: {ex.Message}", ex);
        }
    }
}