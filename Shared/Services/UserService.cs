using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using Shared.Context;
using Shared.DTOs.UserDtos;
using Shared.DTOs.UserDtos.Mappers;
using Shared.Models;
using Shared.Models.Mailing;
using Shared.Services.Mailing;

namespace Shared.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _appContext;
    private readonly TenantDbContext _tenantDbContext;
    private readonly IEncryptionService _encryptionService;
    private readonly IMailService _mailService;

    public UserService(ApplicationDbContext appContext, IEncryptionService encryptionService, TenantDbContext tenantDbContext, IMailService mailService)
    {
        _appContext = appContext;
        _encryptionService = encryptionService;
        _tenantDbContext = tenantDbContext;
        _mailService = mailService;
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

    public async Task<IEnumerable<UserNoPassDto>> GetUsers(int pageNumber, int pageSize = 10)
    {
        var mapper = new UserMapper();
        int itemsToSkip = (pageNumber - 1) * pageSize;

        // Get total count of items
        int totalCount = await _tenantDbContext.Users.CountAsync();

        var items = await _tenantDbContext.Users
            .OrderBy(t => t.Id)
            .Skip(itemsToSkip)
            .Take(pageSize)
            .Select(t => mapper.ToUserNoPassDto(t))
            .ToListAsync();
        return items;
    }

    public async Task<UserNoPassDto> GetUser(string Email)
    {
        throw new NotImplementedException();
    }

    public async Task<UserNoPassDto> SetUser(UserDto request)
    {
        var mapper = new UserMapper();
        var user = new User();
        user.Username = request.Username;
        user.Email = request.Email;
        user.PasswordHash = _encryptionService.Encrypt(request.Password);
        user.PhoneNumber = request.PhoneNumber;

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
                        <a href='{applicationUrl}' style='background-color: #3498db; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block;'>
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
                        <a href='{applicationUrl}' style='background-color: #3498db; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block;'>
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




}
