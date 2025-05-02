using Apbs_Time_App.Client.Shared.Services.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Shared.Context;
using Shared.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace apbs_time_app.Services.Security;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    public readonly TenantDbContext _context;

    public JwtTokenGenerator(TenantDbContext context)
    {
        _context = context;
    }

    public Task<string> GenerateAuthToken(User user)
    {
        var hours = 30;
        var jwtOptions = new JwtIssuerOptions("Audience");

        var tokenExpire = jwtOptions.IssuedAt.AddHours(hours);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = jwtOptions.Issuer,
            Audience = jwtOptions.Audience,
            IssuedAt = jwtOptions.IssuedAt,
            Expires = tokenExpire,
            NotBefore = jwtOptions.NotBefore,
            SigningCredentials = jwtOptions.GetSigningCredentials(),
            Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Name, user.Email!),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                ])
        };

        var handler = new JsonWebTokenHandler();
        handler.SetDefaultTimesOnTokenCreation = false;
        return Task.FromResult(handler.CreateToken(tokenDescriptor));
    }

    public async Task<string> GenerateAccessToken(User user)
    {
        var hours = 20;
        var jwtOptions = new JwtIssuerOptions("Audience");

        var ten = await _context.UserTenantRoles.FirstOrDefaultAsync(s => s.UserId == user.Id);

        if (ten == null)
            return "";


        var tokenExpire = jwtOptions.IssuedAt.AddHours(hours);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email ?? ""),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("tenant_id", ten.TenantId.ToString()),
            new Claim(ClaimTypes.Role, ten.Role.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = jwtOptions.Issuer,
            Audience = jwtOptions.Audience,
            IssuedAt = jwtOptions.IssuedAt,
            Expires = tokenExpire,
            NotBefore = jwtOptions.NotBefore,
            SigningCredentials = jwtOptions.GetSigningCredentials(),
            Subject = new ClaimsIdentity(claims)
        };

        var handler = new JsonWebTokenHandler();
        handler.SetDefaultTimesOnTokenCreation = false;
        return handler.CreateToken(tokenDescriptor);
    }

    public async Task<string> GenerateAccessToken(User user, string tenantId, string authToken)
    {
        var tokenString = authToken.Substring("Bearer ".Length).Trim();

        var ten = await _context.UserTenantRoles.FirstOrDefaultAsync(s => s.UserId == user.Id);

        if (ten == null)
            return "";

        var tokenHandler = new JwtSecurityTokenHandler();

        JwtSecurityToken jwtToken;
        try
        {
            jwtToken = tokenHandler.ReadJwtToken(tokenString);
        }
        catch (Exception)
        {
            throw;
        }
        var authIssuer = jwtToken.Issuer;
        var authAudience = jwtToken.Audiences.FirstOrDefault();
        var authExpires = jwtToken.ValidTo;
        var jwtOptions = new JwtIssuerOptions("Audience");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email ?? ""),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("tenant_id", tenantId),
            new Claim(ClaimTypes.Role, ten.Role.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = authIssuer,
            Audience = authAudience,
            IssuedAt = jwtOptions.IssuedAt,
            Expires = authExpires,
            NotBefore = jwtOptions.NotBefore,
            SigningCredentials = jwtOptions.GetSigningCredentials(),
            Subject = new ClaimsIdentity(claims)
        };

        var handler = new JsonWebTokenHandler();
        handler.SetDefaultTimesOnTokenCreation = false;
        return handler.CreateToken(tokenDescriptor);

    }
}
