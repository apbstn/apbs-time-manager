using Microsoft.EntityFrameworkCore;
using Shared.Context;
using Shared.Models;
using apbs_time_app_admin.Services.TenantService.DTOs;

namespace apbs_time_app_admin.Services.TenantService;

public class TenantService : ITenantService
{
    private readonly TenantDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    public TenantService(TenantDbContext context, IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _context = context;
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    public async Task<Tenant> CreateTenant(CreateTenantRequest request)
    {
        string newConnectionString = null;


        string dbName = "multiTenantAppDb-" + request.Code;
        string defaultConnectionString = _configuration.GetConnectionString("DefaultConnection");
        newConnectionString = defaultConnectionString.Replace("mtaDb", dbName);

        try
        {
            using IServiceScope scopeTenant = _serviceProvider.CreateScope();
            ApplicationDbContext dbContext = scopeTenant.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.SetConnectionString(newConnectionString);
            if (dbContext.Database.GetPendingMigrations().Any())
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"Applying ApplicationDB Migrations for New '{request.Code}' tenant.");
                Console.ResetColor();
                dbContext.Database.Migrate();
            }
        }
        catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }
        

        Tenant tenant = new()
        {
            Name = request.TenantName,
            Code = request.Code,
            ConnectionString = newConnectionString
        };

        _context.Add(tenant);
        _context.SaveChanges();

        return tenant;
    }
}
