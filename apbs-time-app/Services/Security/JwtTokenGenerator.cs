using Apbs_Time_App.Client.Shared.Services.Security;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Shared.Context;
using Shared.Models;
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

    public Task<string> GenerateAccessToken(User user)
    {
        var hours = 20;
        var jwtOptions = new JwtIssuerOptions("Audience");

        var ten = _context.UserTenantRoles.FirstOrDefault(s => s.UserId == user.Id);

        if (ten == null)
            return Task.FromResult("");


        var tokenExpire = jwtOptions.IssuedAt.AddHours(hours);

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Email ?? ""),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim("tenant_id", ten.TenantId),
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
        return Task.FromResult(handler.CreateToken(tokenDescriptor));
    }
}
