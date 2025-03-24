using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Shared.Context;

public class TenantDbContext : DbContext
{
    public TenantDbContext(DbContextOptions<TenantDbContext> options) : base(options)
    {
    }
    public DbSet<Tenant> Tenants{ get; set;}
    public DbSet<User> Users { get; set; }

    //public override int SaveChanges()
    //{
    //    foreach (var entry in ChangeTracker.Entries<IMustHaveTenant>().ToList()) 
    //    { 
    //        switch (entry.State)
    //        {
    //            case EntityState.Added:
    //                entry.Entity.TenantId = CurrentTenantId;
    //                break;

    //            case EntityState.Modified:
    //                entry.Entity.TenantId = CurrentTenantId;
    //                break;
    //        }
    //    }
    //    var result = base.SaveChanges();
    //    return result;
    //}
}
