using Microsoft.AspNetCore.Http;
using Shared.Services;

namespace Shared.Middleware;

public class TenantResolver
{
    private readonly RequestDelegate _next;
    public TenantResolver(RequestDelegate next)
    {
    _next = next; 
    }

    public async Task InvokeAsync(HttpContext context, ICurrentTenantService currentTennant)
    {
        context.Request.Headers.TryGetValue("tenant", out var tenantFromHeader);
        if (context.Request.Path.StartsWithSegments("/api/tenant")
            || context.Request.Path.StartsWithSegments("/api/auth/login")
            || context.Request.Path.StartsWithSegments("/api/auth/register"))
        {
            await _next(context);
            return;
        }
        
        if (!string.IsNullOrEmpty(tenantFromHeader))
        {
            await currentTennant.SetTenant(tenantFromHeader);
            await _next(context);
        }
        else
        {
            throw new Exception("invalid tennant");
        }
    }
}
