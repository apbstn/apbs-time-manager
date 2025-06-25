using Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shared.Repositories;

public interface IUserTenantRepository
{
    Task<Guid?> GetIdByEmailAsync(string email);
    Task<IEnumerable<UserTenant>> GetAllAccountsAsync();
    Task<UserTenant> GetByIdAsync(Guid id);
    Task<bool> UpdateUserAsync(UserTenant userTenant);
    Task<Team> GetTeamByNameAsync(string name);
    Task AddTeamAsync(Team team);
}