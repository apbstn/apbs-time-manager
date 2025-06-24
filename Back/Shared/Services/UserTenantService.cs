using Shared.Services;
using Shared.DTOs.UserDtos;
using Shared.DTOs.UserDtos.Mappers;
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
        var results = new List<UserDto>();
        var _mapper = new UserMapper();
        foreach (var account in accounts)
        {
            results.Add(_mapper.ToUserDto(account));
        }
        return results;
    }
}