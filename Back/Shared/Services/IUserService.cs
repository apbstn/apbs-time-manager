using Shared.DTOs.UserDtos;
using Shared.Models;

namespace Shared.Services;

public interface IUserService
{
    public Task<User> AuthenticateAsync(string email, string password);
    public Task<User> RegisterAsync(User user, string password);
    public Task<IEnumerable<UserNoPassDto>> GetUsers(/*int pageNumber, int pageSize = 10*/);
    public Task<UserNoPassDto> SetUser(UserDto request);
    public Task<User> GetUser(string Email);
    public Task<UserNoPassDto> ResetPass(UserDto user);
    public Task<bool> CheckAccess(Guid UserId, Guid TenantId);
}
