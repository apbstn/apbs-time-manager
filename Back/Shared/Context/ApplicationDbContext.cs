using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Models;
using Shared.Services;

namespace Shared.Context
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ICurrentTenantService _tenantService;
        public Guid? CurrentTenantId { get; set; }
        public string CurrentTenantConnectionString { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentTenantService currentTenantService, ILogger<ApplicationDbContext> logger) : base(options)
        {
            _tenantService = currentTenantService;
            CurrentTenantId = _tenantService.TenantId;
            CurrentTenantConnectionString = _tenantService.ConnectionString;
            logger.LogInformation("Tenant Connection String: {ConnectionString}", CurrentTenantConnectionString);
        }

        public DbSet<UserTenant> Users { get; set; }
        public DbSet<TimeLog> TimeLogs { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Planner> Planners { get; set; }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<User>().HasQueryFilter(a => a.TenantId == CurrentTenantId);
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string tenantConnectionString = CurrentTenantConnectionString;
            if (!string.IsNullOrEmpty(tenantConnectionString))
            {
                _ = optionsBuilder.UseNpgsql(tenantConnectionString);
            }
        }

        public void ForceReload()
        {
            CurrentTenantId = _tenantService.TenantId;
            CurrentTenantConnectionString = _tenantService.ConnectionString;
        }

        

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Team>().HasQueryFilter(t => t.TenantId == CurrentTenantId);
        //    // ... other configurations ...
        //}
        
        //public DbSet<Notification> Notifications { get; set; }
    }


}
