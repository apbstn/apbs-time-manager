using Microsoft.EntityFrameworkCore;
using Shared.Models;
using Shared.Models.Join;

namespace Shared.Context;

public class TenantDbContext : DbContext
{
    public TenantDbContext(DbContextOptions<TenantDbContext> options) : base(options)
    {
    }
    public DbSet<Tenant> Tenants{ get; set;}
    public DbSet<User> Users { get; set; }
    public DbSet<UserTenantRole> UserTenantRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserTenantRole>()
            .HasOne(utr => utr.User)
            .WithMany(u => u.Roles)
            .HasForeignKey(utr => utr.UserId)
            .OnDelete(DeleteBehavior.Cascade); // keep this cascade

        modelBuilder.Entity<UserTenantRole>()
            .HasOne(utr => utr.Tenant)
            .WithMany() // If Tenant has a Roles collection, use `.WithMany(t => t.Roles)`
            .HasForeignKey(utr => utr.TenantId)
            .OnDelete(DeleteBehavior.Restrict); // <- change cascade to restrict or no action
    }


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
