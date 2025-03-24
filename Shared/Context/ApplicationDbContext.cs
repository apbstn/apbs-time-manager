using Microsoft.EntityFrameworkCore;
using Shared.Services;

namespace Shared.Context
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ICurrentTenantService _tenantService;
        public string CurrentTenantId { get; set; }
        public string CurrentTenantConnectionString { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentTenantService currentTenantService) : base(options) 
        { 
            _tenantService = currentTenantService;
            CurrentTenantId = _tenantService.TenantId;
            CurrentTenantConnectionString = _tenantService.ConnectionString;
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<User>().HasQueryFilter(a => a.TenantId == CurrentTenantId);
        //}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string tenantConnectionString = CurrentTenantConnectionString;
            if (!string.IsNullOrEmpty(tenantConnectionString))
            {
                _ = optionsBuilder.UseSqlServer(tenantConnectionString);
            }
        }
    }
}
