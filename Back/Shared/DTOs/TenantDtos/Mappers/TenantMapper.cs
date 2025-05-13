using Riok.Mapperly.Abstractions;
using Shared.Models;

namespace Shared.DTOs.TenantDtos.Mappers;

[Mapper]
public partial class TenantMapper
{
    [MapProperty(nameof(Tenant.Owner.Email), nameof(ResponseTenantDto.Email))]
    [MapProperty(nameof(Tenant.Owner.Username), nameof(ResponseTenantDto.Username))]
    [MapProperty(nameof(Tenant.Owner.PhoneNumber), nameof(ResponseTenantDto.PhoneNumber))]
    public partial ResponseTenantDto ToResponseTenantDto(Tenant tenant);
    
    public partial SimpleTenantDto ToSimpleTenantDto(Tenant tenant);
}
