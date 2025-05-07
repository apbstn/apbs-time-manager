using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Context;
using Shared.Models;

namespace Shared.Extensions;

public static class MultipleDatabaseExtensions
{

    public static async Task<IServiceCollection> AddMigrateTenantDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        using IServiceScope scopeTenant = services.BuildServiceProvider().CreateScope();
        TenantDbContext tenantDbContext = scopeTenant.ServiceProvider.GetRequiredService<TenantDbContext>();

        if (tenantDbContext.Database.GetPendingMigrations().Any())
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Applying BaseDb Migrations.");
            Console.ResetColor();
            tenantDbContext.Database.Migrate();
        }

        List<Tenant> tenantsInDb = tenantDbContext.Tenants.ToList();
        string defaultConnectionString = configuration.GetConnectionString("DefaultConnection");

        foreach (Tenant tenant in tenantsInDb)
        {
            string connectionString = string.IsNullOrEmpty(tenant.ConnectionString) ? defaultConnectionString : tenant.ConnectionString;

            using IServiceScope scopeApplication = services.BuildServiceProvider().CreateScope();
            ApplicationDbContext dbContext = scopeApplication.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.SetConnectionString(connectionString);

            if (dbContext.Database.GetPendingMigrations().Any())
            {
                try { 
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"Applying Migrations for '{tenant.Id}' tenant.");
                Console.ResetColor();
                dbContext.Database.Migrate();
                }
                catch(Exception ex)
                {
                    
                }
            }
        }

        return services;
    }
}
