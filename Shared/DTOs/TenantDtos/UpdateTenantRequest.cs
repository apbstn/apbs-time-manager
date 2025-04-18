namespace Shared.DTOs.TenantDtos;

public class UpdateTenantRequest
{
    public string Name { get; set; }         // Tenant Name
    public string Email { get; set; }
    public string Username { get; set; }
    public string PhoneNumber { get; set; }
}

