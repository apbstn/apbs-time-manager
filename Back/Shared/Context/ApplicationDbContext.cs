using Microsoft.EntityFrameworkCore;
using Shared.Models;
using Shared.Services;
using Shared.Models.Planners;

namespace Shared.Context
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ICurrentTenantService _tenantService;
        public Guid? CurrentTenantId { get; set; }
        public string CurrentTenantConnectionString { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentTenantService currentTenantService) : base(options)
        {
            _tenantService = currentTenantService;
            CurrentTenantId = _tenantService.TenantId;
            CurrentTenantConnectionString = _tenantService.ConnectionString;
        }

        public DbSet<UserTenant> Users { get; set; }
        public DbSet<TimeLog> TimeLogs { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<PlannerBase> Planners { get; set; }
        public DbSet<FixedPlanner> FixedPlanners { get; set; }
        public DbSet<FlexiblePlanner> FlexiblePlanners { get; set; }
        public DbSet<WeeklyPlanner> WeeklyPlanners { get; set; }
        public DbSet<FixedDayPlanner> fixedDayPlanners { get; set; }
        public DbSet<FlexibleDayPlanner> flexibleDayPlanners { get; set; }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<User>().HasQueryFilter(a => a.TenantId == CurrentTenantId);
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlannerBase>().UseTptMappingStrategy();
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
