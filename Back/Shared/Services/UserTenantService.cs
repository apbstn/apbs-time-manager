using Shared.Services;
using Shared.Repositories;
using Shared.DTOs.UserDtos;
using Shared.DTOs.UserDtos.Mappers;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Shared.Services;

public class UserTenantService : IUserTenantService
{
    private readonly IUserTenantRepository _repository;

    public UserTenantService(IUserTenantRepository repository)
    {
        _repository = repository;
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
}