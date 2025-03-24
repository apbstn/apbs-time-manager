namespace apbs_time_app_admin.Services.TenantService.DTOs;

public class CreateTenantRequest
{
    public string Code { get; set; } 
    public string TenantName { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string PhoneNumber { get; set; }
}
