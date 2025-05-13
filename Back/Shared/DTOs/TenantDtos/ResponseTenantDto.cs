using Shared.Models;

namespace Shared.DTOs.TenantDtos;

public class ResponseTenantDto
{
    public string? Id { get; set; }
    public string? Code { get; set; }
    public string? TenantName { get; set; }
    public string? ConnectionString { get; set; }
    public string? UserId { get; set; }
    public string? Email { get; set; }
    public string? Username { get; set; }
    public string? PhoneNumber { get; set; }




}
