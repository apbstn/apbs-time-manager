using Shared.DTOs.UserDtos;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shared.Services;

public interface IUserTenantService
{
    Task<Guid?> GetIdByEmailAsync(string email);
    Task<IEnumerable<object>> GetAllAccountsAsync();
    Task<bool> EditUserTeamAsync(Guid id, string team);
    Task<bool> RemoveUserFromTeamAsync(Guid id);
    Task<UserTenant> DeleteUsers(Guid id);
}