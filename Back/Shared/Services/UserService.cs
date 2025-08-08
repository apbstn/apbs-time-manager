using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NPOI.SS.Formula.Functions;
using Shared.Context;
using Shared.DTOs.UserDtos;
using Shared.DTOs.UserDtos.Mappers;
using Shared.Models;
using Shared.Models.Mailing;
using Shared.Services.Mailing;
using System.Data;

namespace Shared.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _appContext;
    private readonly TenantDbContext _tenantDbContext;
    private readonly IEncryptionService _encryptionService;
    private readonly IMailService _mailService;
    private readonly IConfiguration _configuration;
    private readonly ICurrentTenantService _currentTenantService;
    private readonly ILogger<UserService>? _logger;

    public UserService(ApplicationDbContext appContext, IEncryptionService encryptionService, TenantDbContext tenantDbContext, IMailService mailService, ICurrentTenantService currentTenantService)
    {
        _appContext = appContext;
        _encryptionService = encryptionService;
        _tenantDbContext = tenantDbContext;
        _mailService = mailService;
        _currentTenantService = currentTenantService;
    }

    public async Task<User> AuthenticateAsync(string email, string password)
    {
        var user = await _tenantDbContext.Users.SingleOrDefaultAsync(u => u.Email == email);

        if (user == null || !VerifyPassword(password, user.PasswordHash))
            return null;

        return user;
    }

    public async Task<User> AuthenticateAsyncNoPass(string email)
    {
        var user = await _tenantDbContext.Users.SingleOrDefaultAsync(u => u.Email == email);

        return user;
    }


    public async Task<User> RegisterAsync(User user, string password)
    {
        if (await _tenantDbContext.Users.AnyAsync(u => u.Email == user.Email))
            return null;

        user.PasswordHash = _encryptionService.Encrypt(password);
        var result = _tenantDbContext.Users.Add(user);
        await _tenantDbContext.SaveChangesAsync();

        return result.Entity;
    }

    private bool VerifyPassword(string password, string storedHash)
    {
        var decryptedPassword = _encryptionService.Decrypt(storedHash);
        return password == decryptedPassword;
    }

    public async Task<IEnumerable<UserNoPassDto>> GetUsers(/*int pageNumber, int pageSize = 10*/)
    {
        var mapper = new UserMapper();

        // Get total count of items
        int totalCount = await _tenantDbContext.Users.CountAsync();

        var items = await _tenantDbContext.Users
            .OrderBy(t => t.Id)
            //.Skip(itemsToSkip)
            //.Take(pageSize)
            .Select(t => mapper.ToUserNoPassDto(t))
            .ToListAsync();
        return items;
    }

    public async Task<User> GetUser(string Email)
    {
        try
        {
            var user = await _tenantDbContext.Users.FirstAsync(x => x.Email == Email);
            return user;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<UserNoPassDto> SetUser(UserDto request)
    {
        var mapper = new UserMapper();
        var user = new User();
        user.Username = request.Username;
        user.Email = request.Email;
        user.PasswordHash = _encryptionService.Encrypt(request.Password);
        user.PhoneNumber = request.PhoneNumber;
        user.Registred = true;

        _tenantDbContext.Users.Add(user);
        await _tenantDbContext.SaveChangesAsync();

        _ = Task.Run(() => SendUserCreationMail(user, request.Password));

        return mapper.ToUserNoPassDto(user);
    }


    public async Task SendUserCreationMail(User user, string pass)
    {
        var applicationUrl = "https://localhost:5197";
        string body = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ color: #2c3e50; border-bottom: 1px solid #eee; padding-bottom: 10px; }}
                    .footer {{ margin-top: 20px; font-size: 0.8em; color: #7f8c8d; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h2>Welcome to Our Platform!</h2>
                    </div>
                    <p>Dear {user.Username},</p>
                    <p>Your account has been successfully created. Here are your login details:</p>
                    <p><strong>Username:</strong> {user.Username}</p>
                    <p><strong>Email:</strong> {user.Email}</p>
                    <p><strong>Temporary Password:</strong> {pass}</p>
                    <p>For security reasons, we recommend you change your password after first login.</p>
                    <p>You can access the system by clicking the button below:</p>
                    <p>
                        <a href='{applicationUrl}' style='background-color: #35D300; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                            Go to Application
                        </a>
                    </p>
                    <div class='footer'>
                        <p>Best regards,<br>The Support Team</p>
                        <p>If you didn't request this account, please ignore this email.</p>
                    </div>
                </div>
            </body>
            </html>";

        var mailParams = new MailParams
        {
            ToEmail = user.Email,
            Subject = "Your Account Has Been Created",
            Body = body,
            IsBodyHtml = true
        };

        await _mailService.SendEmailAsync(mailParams);
    }

    public async Task<UserNoPassDto> ResetPass(UserDto req)
    {
        var mapper = new UserMapper();
        var user = await _tenantDbContext.Users.FirstAsync(s => s.Email == req.Email);

        user.PasswordHash = _encryptionService.Encrypt(req.Password);

        _tenantDbContext.Users.Update(user);
        _tenantDbContext.SaveChanges();

        _ = Task.Run(() => SendUserResetPasswordMail(user, req.Password));

        return mapper.ToUserNoPassDto(user);
    }


    public async Task SendUserResetPasswordMail(User user, string pass)
    {
        var applicationUrl = "https://localhost:5197";
        string body = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ color: #2c3e50; border-bottom: 1px solid #eee; padding-bottom: 10px; }}
                    .footer {{ margin-top: 20px; font-size: 0.8em; color: #7f8c8d; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h2>Welcome to Our Platform!</h2>
                    </div>
                    <p>Dear {user.Username},</p>
                    <p>Your password has been successfully modified. Here are your login details:</p>
                    <p><strong>Username:</strong> {user.Username}</p>
                    <p><strong>Email:</strong> {user.Email}</p>
                    <p><strong>Temporary Password:</strong> {pass}</p>
                    <p>For security reasons, we recommend you change your password after first login.</p>
                    <p>You can access the system by clicking the button below:</p>
                    <p>
                        <a href='{applicationUrl}' style='background-color: #35D300; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                            Go to Application
                        </a>
                    </p>
                    <div class='footer'>
                        <p>Best regards,<br>The Support Team</p>
                        <p>If you didn't request this account, please ignore this email.</p>
                    </div>
                </div>
            </body>
            </html>";

        var mailParams = new MailParams
        {
            ToEmail = user.Email,
            Subject = "Your Account Has Been Created",
            Body = body,
            IsBodyHtml = true
        };

        await _mailService.SendEmailAsync(mailParams);
    }

    public async Task<bool> CheckAccess(Guid UserId, Guid TenantId)
    {
        var ss = await _tenantDbContext.UserTenantRoles.FirstOrDefaultAsync(s => s.UserId == UserId && s.TenantId == TenantId);

        if (ss != null)
            return true;
        return false;
    }

    public async Task<User> DeleteUser(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        }

        try
        {
            using var transaction = await _tenantDbContext.Database.BeginTransactionAsync();

            // Find the user in the tenant context
            var user = await _tenantDbContext.Users.FindAsync(userId);
            if (user == null)
            {
                _logger?.LogWarning("User with ID {UserId} not found.", userId);
                return null; // User not found
            }

            // Find the invitation in the tenant context
            var invitationId = await _tenantDbContext.Invitations
                .Where(a => a.UserId == userId)
                .Select(a => a.Id)
                .FirstOrDefaultAsync();
            
            // Store the old email before deleting
        string oldEmail = user.Email;

            // Retrieve the tenant ID from UserTenantRoles
            var userTenantRole = await _tenantDbContext.UserTenantRoles
                .FirstOrDefaultAsync(utr => utr.UserId == userId);
            if (userTenantRole == null)
            {
                _logger?.LogWarning("No tenant role found for user ID {UserId}.", userId);
                // Instead of throwing, proceed to delete user and invitation
            }
            else
            {
                Guid tenantId = userTenantRole.TenantId;

                // Retrieve the tenant's connection string
                var tenant = await _tenantDbContext.Tenants
                    .FirstOrDefaultAsync(t => t.Id == tenantId);
                if (tenant == null || string.IsNullOrEmpty(tenant.ConnectionString))
                {
                    _logger?.LogWarning("Tenant or connection string not found for tenant ID {TenantId}.", tenantId);
                    throw new InvalidOperationException("Tenant or connection string not found for the user.");
                }

                // Store the original connection string
                var originalConnectionString = _appContext.Database.GetDbConnection().ConnectionString;

                // Ensure the connection is closed before changing the connection string
                if (_appContext.Database.GetDbConnection().State == ConnectionState.Open)
                {
                    await _appContext.Database.CloseConnectionAsync();
                }

                // Set the tenant's connection string
                if (string.IsNullOrWhiteSpace(tenant.ConnectionString))
                {
                    throw new InvalidOperationException("Tenant connection string is empty or invalid.");
                }

                _appContext.Database.GetDbConnection().ConnectionString = tenant.ConnectionString;

                try
                {
                    // Verify and open the connection
                    if (string.IsNullOrWhiteSpace(_appContext.Database.GetDbConnection().ConnectionString))
                    {
                        throw new InvalidOperationException("Failed to set the connection string for _appContext.");
                    }

                    await _appContext.Database.OpenConnectionAsync();

                    // Find the account in T_ACCOUNT using the old email
                    var account = await _appContext.Users
                        .FirstOrDefaultAsync(a => a.Email == oldEmail);
                    Guid accountId = account?.Id ?? Guid.Empty;
                    if (account != null)
                    {
                        _appContext.Users.Remove(account);
                        _logger?.LogInformation("Deleted user account with email {Email} (ID: {UserId}).", oldEmail, accountId);
                    }
                    else
                    {
                        _logger?.LogInformation("No user account found for email {Email}, skipping deletion.", oldEmail);
                    }

                    // Delete LeaveRequests
                    var leaveRequest = await _appContext.LeaveRequests
                        .FirstOrDefaultAsync(a => a.UserId == accountId);
                    if (leaveRequest != null)
                    {
                        _appContext.LeaveRequests.Remove(leaveRequest);
                        _logger?.LogInformation("Deleted LeaveRequest for user ID {UserId}.", accountId);
                    }
                    else
                    {
                        _logger?.LogInformation("No LeaveRequest found for user ID {UserId}, skipping deletion.", accountId);
                    }

                    // Delete LeaveBalances
                    var leaveBalance = await _appContext.LeaveBalances
                        .FirstOrDefaultAsync(a => a.UserId == accountId);
                    if (leaveBalance != null)
                    {
                        _appContext.LeaveBalances.Remove(leaveBalance);
                        _logger?.LogInformation("Deleted LeaveBalance for user ID {UserId}.", accountId);
                    }
                    else
                    {
                        _logger?.LogInformation("No LeaveBalance found for user ID {UserId}, skipping deletion.", accountId);
                    }

                    // Delete TimeLogs
                    var timeLogsCount = await _appContext.TimeLogs
                        .Where(tl => tl.UserId == accountId)
                        .CountAsync();
                    if (timeLogsCount > 0)
                    {
                        await _appContext.TimeLogs
                            .Where(tl => tl.UserId == accountId)
                            .ExecuteDeleteAsync();
                        _logger?.LogInformation("Deleted {Count} TimeLogs for user ID {UserId}.", timeLogsCount, accountId);
                    }
                    else
                    {
                        _logger?.LogInformation("No TimeLogs found for user ID {UserId}, skipping deletion.", accountId);
                    }

                    // Save changes to T_ACCOUNT
                    await _appContext.SaveChangesAsync();
                }
                finally
                {
                    // Close the connection and restore the original connection string
                    if (_appContext.Database.GetDbConnection().State == ConnectionState.Open)
                    {
                        await _appContext.Database.CloseConnectionAsync();
                    }
                    _appContext.Database.GetDbConnection().ConnectionString = originalConnectionString;
                }
            }

            // Delete the user from the tenant context
            _tenantDbContext.Users.Remove(user);

            var invitationToRemove = await _tenantDbContext.Invitations.FindAsync(invitationId);
            if (invitationToRemove != null)
            {
                _tenantDbContext.Invitations.Remove(invitationToRemove);
                _logger?.LogInformation("Deleted invitation with ID {InvitationId} for user ID {UserId}.", invitationId, userId);
            }

            // Save changes to the tenant context
            await _tenantDbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger?.LogInformation("User with ID {UserId} deleted successfully.", userId);
            return user;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger?.LogError(ex, "Concurrency conflict occurred while deleting user with ID {UserId}.", userId);
            throw new InvalidOperationException("Concurrency conflict occurred while deleting the user.", ex);
        }
        catch (DbUpdateException ex)
        {
            _logger?.LogError(ex, "Database error occurred while deleting user with ID {UserId}.", userId);
            throw new InvalidOperationException("An error occurred while deleting the user due to database constraints.", ex);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error occurred while deleting user with ID {UserId}.", userId);
            throw new InvalidOperationException("An unexpected error occurred while deleting the user.", ex);
        }
    }

    public async Task<User> EditUser(Guid userId, User updatedUser)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        }

        try
        {
            using var transaction = await _tenantDbContext.Database.BeginTransactionAsync();

            // Find the user in the tenant context
            var user = await _tenantDbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return null; // User not found
            }

            // Store the old email before updating
            string oldEmail = user.Email;

            // Update user properties in the tenant context (but don't save yet)
            user.Email = updatedUser.Email;
            user.Username = updatedUser.Username;
            user.PhoneNumber = updatedUser.PhoneNumber;

            // Retrieve the tenant ID from UserTenantRoles
            var userTenantRole = await _tenantDbContext.UserTenantRoles
                .FirstOrDefaultAsync(utr => utr.UserId == userId);
            if (userTenantRole == null)
            {
                throw new InvalidOperationException("No tenant role found for the user.");
            }
            Guid tenantId = userTenantRole.TenantId;

            // Retrieve the tenant's connection string using TenantId
            var tenant = await _tenantDbContext.Tenants
                .FirstOrDefaultAsync(t => t.Id == tenantId);
            if (tenant == null || string.IsNullOrEmpty(tenant.ConnectionString))
            {
                throw new InvalidOperationException("Tenant or connection string not found for the user.");
            }

            // Store the original connection string
            var originalConnectionString = _appContext.Database.GetDbConnection().ConnectionString;

            // Ensure the connection is closed before changing the connection string
            if (_appContext.Database.GetDbConnection().State == System.Data.ConnectionState.Open)
            {
                await _appContext.Database.CloseConnectionAsync();
            }

            // Set the tenant's connection string and validate it
            if (string.IsNullOrWhiteSpace(tenant.ConnectionString))
            {
                throw new InvalidOperationException("Tenant connection string is empty or invalid.");
            }
            _appContext.Database.GetDbConnection().ConnectionString = tenant.ConnectionString;

            try
            {
                // Verify the connection string is set
                if (string.IsNullOrWhiteSpace(_appContext.Database.GetDbConnection().ConnectionString))
                {
                    throw new InvalidOperationException("Failed to set the connection string for _appContext.");
                }

                // Open the connection
                await _appContext.Database.OpenConnectionAsync();

                // Find the account in T_ACCOUNT using the old email
                var account = await _appContext.Users
                    .FirstOrDefaultAsync(a => a.Email == oldEmail);
                if (account == null)
                {
                    throw new InvalidOperationException($"No T_ACCOUNT record found for email: {oldEmail}");
                }

                // Update the account details
                account.Email = updatedUser.Email;
                account.Username = updatedUser.Username;
                account.PhoneNumber = updatedUser.PhoneNumber;

                // Save changes to T_ACCOUNT
                await _appContext.SaveChangesAsync();
            }
            finally
            {
                // Close the connection and restore the original connection string
                if (_appContext.Database.GetDbConnection().State == System.Data.ConnectionState.Open)
                {
                    await _appContext.Database.CloseConnectionAsync();
                }
                _appContext.Database.GetDbConnection().ConnectionString = originalConnectionString;
            }

            // Save changes to the tenant context
            await _tenantDbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return user; // Return the updated user
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new InvalidOperationException("Concurrency conflict occurred while editing the user.", ex);
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException("An error occurred while editing the user due to database constraints.", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("An unexpected error occurred while editing the user.", ex);
        }
    }
}

   

