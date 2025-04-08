using Shared.DTOs;

namespace apbs_time_app_admin.Services.TenantService.DTOs;

public class CreateTenantRequest
{
    public string Code { get; set; } 
    public string TenantName { get; set; }
    public UserDto User { get; set; }

}
