using Microsoft.EntityFrameworkCore;
using Shared.Context;
using Shared.DTOs;
using Shared.Models;

namespace Shared.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _appContext;
    private readonly TenantDbContext _tenantDbContext;
    private readonly IEncryptionService _encryptionService;

    public UserService(ApplicationDbContext appContext, IEncryptionService encryptionService, TenantDbContext tenantDbContext)
    {
        _appContext = appContext;
        _encryptionService = encryptionService;
        _tenantDbContext = tenantDbContext;
    }

    public async Task<User> AuthenticateAsync(string email, string password)
    {
        var user = await _tenantDbContext.Users.SingleOrDefaultAsync(u => u.Email == email);

        if (user == null || !VerifyPassword(password, user.PasswordHash))
            return null;

        return user;
    }

    public async Task<bool> RegisterAsync(User user, string password)
    {
        if (await _tenantDbContext.Users.AnyAsync(u => u.Username == user.Username))
            return false;

        user.PasswordHash = _encryptionService.Encrypt(password);
        _tenantDbContext.Users.Add(user);
        await _tenantDbContext.SaveChangesAsync();

        return true;
    }

    private bool VerifyPassword(string password, string storedHash)
    {
        var decryptedPassword = _encryptionService.Decrypt(storedHash);
        return password == decryptedPassword;
    }

    public async Task<IEnumerable<User>> GetUsers()
    {
        var users = _tenantDbContext.Users.ToList();
        return users;
    }

    public async Task<User> GetUser(string Email)
    {
        throw new NotImplementedException();
    }

    public async Task<User> SetUser(UserDto request)
    {
        var user = new User();
        user.Username = request.Username;
        user.Email = request.Email;
        user.PasswordHash = request.Password;

        _tenantDbContext.Users.Add(user);
        _tenantDbContext.SaveChanges();
        
        return user;
    }
}
