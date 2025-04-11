using Shared.DTOs.UserDtos;

namespace apbs_time_app_admin.Services.TenantService.DTOs;

public class CreateTenantRequest
{
    public string Code { get; set; } 
    public string TenantName { get; set; }
    public string User { get; set; }

}
