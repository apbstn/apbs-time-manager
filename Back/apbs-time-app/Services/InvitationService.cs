using apbs_time_app.Models;
using apbs_time_app.Services.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NPOI.POIFS.Crypt;
using Shared.Context;
using Shared.DTOs.UserDtos;
using Shared.DTOs.UserDtos.Mappers;
using Shared.Models;
using Shared.Models.Mailing;
using Shared.Services.Mailing;
using System.Security.Cryptography;

namespace apbs_time_app.Services;

public class InvitationService : IInvitationService
{
    private TenantDbContext _tenantDbContext;
    private IMailService _mailService;
    private IEncryption _encryption;
    private ApplicationDbContext _appContext;
    private ILogger<InvitationService> _logger;

    public InvitationService(TenantDbContext tenantDbContext, IMailService mailService, IEncryption encryption, ApplicationDbContext applicationDbContext)
    {
        _appContext = applicationDbContext;
        _tenantDbContext = tenantDbContext;
        _mailService = mailService;
        _encryption = encryption;
    }


    public async Task<Invitation> CreateInviteExist(User user, string tenantId)
    {
        var expiration = DateTime.UtcNow.AddDays(10);
        var token = GenerateToken(64);

        var inv = new Invitation
        {
            Email = user.Email ?? string.Empty,
            Phone_Number = user.PhoneNumber ?? string.Empty,
            UserId = user.Id,
            TenantId = Guid.Parse(tenantId),
            Token = token,
            ExpiresAt = expiration,
            UserExists = true
        };

        await _tenantDbContext.Invitations.AddAsync(inv);
        _tenantDbContext.SaveChanges();

        var tenant = _tenantDbContext.Tenants.FirstAsync(t => t.Id == Guid.Parse(tenantId)).Result;

        _ = Task.Run(() => SendInviteEmail(user, token, inv, tenant));

        return inv;
    }
    public async Task<Invitation> CreateInviteNotExist(UserNoPassDto user, string tenantId)
    {
        var expiration = DateTime.UtcNow.AddDays(10);
        var token = GenerateToken(64);
        var mapper = new UserMapper();

        var inv = new Invitation
        {
            Email = user.Email ?? string.Empty,
            Phone_Number = user.PhoneNumber ?? string.Empty,
            TenantId = Guid.Parse(tenantId),
            Token = token,
            ExpiresAt = expiration,
            UserExists = false
        };

        await _tenantDbContext.Invitations.AddAsync(inv);
        _tenantDbContext.SaveChanges();

        await _appContext.Users.AddAsync(mapper.FromUserNoPassDtoToUserTenant(user));
        _appContext.SaveChanges();

        var tenant = _tenantDbContext.Tenants.FirstAsync(t => t.Id == Guid.Parse(tenantId)).Result;


        _ = Task.Run(() => SendInviteEmail(mapper.ToUserNoId(user), token, inv, tenant));

        return inv;
    }

    public static string GenerateToken(int length = 32)
    {
        byte[] bytes = new byte[length];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes)
                     .Replace("+", "-")
                     .Replace("/", "_")
                     .Replace("=", ""); // make URL-safe
    }



    public async Task SendInviteEmail(User user, string token, Invitation inv, Tenant tenant)
    {
        var combined = $"{user.Email}|{user.Username}|{tenant.Id}";
        var encryptedData = _encryption.EncryptString(combined);
        var urlEncodedData = Uri.EscapeDataString(encryptedData);
        var applicationUrl = inv.UserExists
            ? $"http://localhost:5173/invite/exists?token={token}&data={urlEncodedData}"
            : $"http://localhost:5173/invite/dontExists?token={token}&data={urlEncodedData}";
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
                    <h2>You're Invited!</h2>
                </div>
                <p>Dear {user.Username},</p>
                <p>You have been invited to join {tenant.Name} on our platform. Please click the button below to accept the invitation and complete your registration.</p>
                <p>
                    <a href='{applicationUrl}' style='background-color: #35D300; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                        Accept Invitation
                    </a>
                </p>
                <div class='footer'>
                    <p>Best regards,<br>The Support Team</p>
                    <p>If you did not expect this invitation, you can safely ignore this email.</p>
                </div>
            </div>
        </body>
        </html>";
        var mailParams = new MailParams
        {
            ToEmail = inv.Email,
            Subject = "You're Invited to Join",
            Body = body,
            IsBodyHtml = true
        };
        await _mailService.SendEmailAsync(mailParams);
    }

    public InviteData ExtractInvitationData(string data)
    {
        var inviteData = _encryption.DecryptString(data);

        var inv = inviteData.Split('|');

        return new InviteData
        {
            Email = inv[0],
            Username = inv[1],
            TenantId = inv[2]
        };
    }

    public async Task<bool> CheckInvite(ConfirmInvite confirm)
    {
        var inv = await _tenantDbContext.Invitations.FirstAsync(t => t.Email == confirm.Email && t.TenantId == Guid.Parse(confirm.TenantId));

        if (inv.Token == confirm.Token)
        {
            inv.IsUsed = true; // Fix for CS0120: Accessing the instance property instead of the static property.  
            await _tenantDbContext.SaveChangesAsync(); // Ensure changes are saved to the database.  
            return true;
        }
        else
        {
            return false;
        }
    }
    public async Task<List<Invitation>> GetInvitationsByTenantIdAsync(string tenantId)
    {
        return await _tenantDbContext.Invitations
            .Where(i => i.TenantId == Guid.Parse(tenantId))
            .ToListAsync();
    }

    public async Task<string> DeleteInvitations(string email)
    {
        var invv = await _tenantDbContext.Invitations.FirstOrDefaultAsync(i => i.Email == email);
        if (invv == null)
        {
            return "no invitation was found";
        }
        _tenantDbContext.Remove(invv);
        await _tenantDbContext.SaveChangesAsync();
        return "Deletion Done";
    }
}
