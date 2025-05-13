using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Apbs_Time_App.Client.Shared.Services.Security;

public static class JwtExtensions
{
    public static TokenValidationParameters GetTokenValidationParameters(string audience = "Audience")
    {
        var _options = new JwtIssuerOptions(audience);
        return new TokenValidationParameters
        {
            // the signing key must match!
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _options.GetSigningCredentials().Key,
            // validate the JWT Issuer (iss) claim
            ValidateIssuer = true,
            ValidIssuer = _options.Issuer,
            // validate the JWT Audience (aud) claim
            ValidateAudience = true,
            ValidAudience = _options.Audience,
            // validate the token expiry
            ValidateLifetime = true,
            // if you want to allow a certain amount of clock drift, set that here:
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = ClaimTypes.Role
        };
    }
}
