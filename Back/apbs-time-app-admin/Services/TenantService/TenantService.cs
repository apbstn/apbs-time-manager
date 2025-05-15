using Microsoft.EntityFrameworkCore;
using Shared.Context;
using Shared.DTOs.TenantDtos;
using Shared.DTOs.TenantDtos.Mappers;
using Shared.Models;
using Shared.Models.Enumerations;
using Shared.Models.Join;
using Shared.Services;
using Shared.Services.Pagination;

namespace apbs_time_app_admin.Services.TenantService;

public class TenantService : ITenantService
{
    private readonly TenantDbContext _context;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private readonly ICurrentTenantService _currentTenantService;
    public TenantService(TenantDbContext context, 
        IConfiguration configuration, 
        IServiceProvider serviceProvider, 
        ICurrentTenantService currentTenantService,
        ApplicationDbContext applicationDbContext)
    {
        _context = context;
        _configuration = configuration;
        _serviceProvider = serviceProvider;
        _currentTenantService = currentTenantService;
        _applicationDbContext = applicationDbContext;
    }

    public async Task<Tenant> CreateTenant(CreateTenantRequest request, string user)
    {
        string newConnectionString = null;



        var us = await _context.Users.FirstAsync(x => x.Id == Guid.Parse(user));

        Tenant tenant = new()
        {
            Name = request.TenantName,
            Code = request.Code,
            ConnectionString = newConnectionString,
            UserId = us.Id
        };

        _context.Tenants.Add(tenant);
        _context.SaveChanges();

        string dbName = "multiTenantAppDb-" + tenant.Id;
        string defaultConnectionString = _configuration.GetConnectionString("DefaultConnection");
        newConnectionString = defaultConnectionString.Replace("TenantDB", dbName);

        tenant.ConnectionString = newConnectionString;

        _context.Update(tenant);
        _context.SaveChanges();


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


        us.Roles.Add(new UserTenantRole
        {
            TenantId = tenant.Id,
            UserId = us.Id,
            Role = RoleEnum.Owner
        });

        _context.Users.Update(us);
        _context.SaveChanges();

        await _currentTenantService.SetTenant(tenant.Id);
        _applicationDbContext.ForceReload();


        _applicationDbContext.Users.Add(new UserTenant
        {
            Id = us.Id,
            Username = us.Username,
            Email = us.Email,
            PhoneNumber = us.PhoneNumber,
            Role = RoleEnum.Owner
        });

        _applicationDbContext.SaveChanges();

        return tenant;
    }

    public async Task<PaginatedResponse<ResponseTenantDto>> GetAll(int pageNumber, int pageSize = 10)
    {
        int itemsToSkip = (pageNumber - 1) * pageSize;

        // Get total count of items
        int totalCount = await _context.Tenants.CountAsync();

        var mapper = new TenantMapper();

        var items = await _context.Tenants
            .Include(t=> t.Owner)
            .OrderBy(t => t.Id)
            .Skip(itemsToSkip)
            .Take(pageSize)
            .Select(t => mapper.ToResponseTenantDto(t))
            .ToListAsync();

        return new PaginatedResponse<ResponseTenantDto>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = items
        };
    }

    public async Task<bool> UpdateTenant(Guid tenantId, UpdateTenantRequest request)
    {
        var tenant = await _context.Tenants
            .Include(t => t.Owner) // ⬅️ important!
            .FirstOrDefaultAsync(t => t.Id == tenantId);

        if (tenant == null || tenant.Owner == null)
            return false;

        tenant.Name = request.Name;
        tenant.Owner.Email = request.Email;
        tenant.Owner.Username = request.Username;
        tenant.Owner.PhoneNumber = request.PhoneNumber;

        await _context.SaveChangesAsync();
        return true;
    }


    public async Task<bool> DeleteTenant(Guid tenantId)
    {
        var tenant = await _context.Tenants.FindAsync(tenantId);
        if (tenant == null)
            return false;

        _context.Tenants.Remove(tenant);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Tenant>> GetTenantsByUserIdAsync(Guid userId)
    {
        return await _context.Tenants
            .Where(ut => ut.UserId == userId)
            .Join(
                _context.Tenants,
                ut => ut.Id,
                t => t.Id,
                (ut, t) => t)
            .ToListAsync();
    }

}
