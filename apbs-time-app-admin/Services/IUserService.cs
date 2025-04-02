using Shared.DTOs;
using Shared.Models;

namespace Shared.Services;

public interface IUserService
{
    public Task<User> AuthenticateAsync(string email, string password);
    public Task<bool> RegisterAsync(User user, string password);
    public Task<IEnumerable<User>> GetUsers();
    public Task<User> SetUser(UserDto request);
    public Task<User> GetUser(string Email);
}
