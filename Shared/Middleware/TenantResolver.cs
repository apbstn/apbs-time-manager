using Microsoft.AspNetCore.Http;
using Shared.Services;
using System.IdentityModel.Tokens.Jwt;

namespace Shared.Middleware;

public class TenantResolver
{
    private readonly RequestDelegate _next;
    public TenantResolver(RequestDelegate next)
    {
    _next = next; 
    }

    public async Task InvokeAsync(HttpContext context, ICurrentTenantService currentTenant)
    {
        context.Request.Headers.TryGetValue("tenant", out var tenantFromHeader);
        context.Request.Headers.TryGetValue("Authorization", out var accessTokenHeader);

        if (context.Request.Path.StartsWithSegments("/api/auth/login") 
            || context.Request.Path.StartsWithSegments("/api/auth/switch"))
        {
            await _next(context);
            return;
        }
        
        if (!string.IsNullOrEmpty(accessTokenHeader))
        {
            var token = accessTokenHeader.ToString().Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Look for the claim that holds the tenant ID
            var tenantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "tenant_id")?.Value;


            if (!string.IsNullOrEmpty(tenantId))
            {
                await currentTenant.SetTenant(tenantId);
                await _next(context);
                return;
            }
            await _next(context);
        }
        else
        {
            throw new Exception("invalid tennant");
        }
    }
}
