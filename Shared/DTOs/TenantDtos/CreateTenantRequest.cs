namespace Shared.DTOs.TenantDtos;

public class CreateTenantRequest
{
    public string Code { get; set; } 
    public string TenantName { get; set; }
    public string UserId { get; set; }

}
