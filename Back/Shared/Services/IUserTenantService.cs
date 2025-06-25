using Shared.DTOs.UserDtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shared.Services;

public interface IUserTenantService
{
    Task<Guid?> GetIdByEmailAsync(string email);
    Task<IEnumerable<object>> GetAllAccountsAsync();
    Task<bool> EditUserTeamAsync(Guid id, string team);
}