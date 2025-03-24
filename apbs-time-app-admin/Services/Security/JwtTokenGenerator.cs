using apbs_time_app_admin.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Shared.Models;
using System.Security.Claims;

namespace apbs_time_app_admin.Services.Security;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    public Task<string> Generate(User user)
    {
        var hours = 30;
        var newExpire = 0;
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
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim("role", user.Role.ToString())
                ])
        };

        var handler = new JsonWebTokenHandler();
        handler.SetDefaultTimesOnTokenCreation = false;
        return Task.FromResult(handler.CreateToken(tokenDescriptor));
    }
}
