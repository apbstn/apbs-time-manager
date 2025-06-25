using Riok.Mapperly.Abstractions;
using Shared.Models;
using Shared.DTOs.UserDtos;

namespace Shared.DTOs.UserDtos.Mappers;

[Mapper]
public partial class UserMapper
{
    [MapperIgnoreTarget(nameof(UserDto.Password))]
    public partial UserDto ToUserDto(User user);

    [MapperIgnoreTarget(nameof(UserDto.Password))]
    public partial User ToUser(UserNoPassDto user);

    [MapperIgnoreTarget(nameof(User.PasswordHash))]
    public partial UserNoPassDto ToUserNoPassDto(User user);

    [MapperIgnoreTarget(nameof(UserNoPassDto.Id))]
    public partial UserTenant FromUserNoPassDtoToUserTenant(UserNoPassDto user);

    [MapperIgnoreTarget(nameof(UserNoPassDto.Id))]
    public partial User ToUserNoId(UserNoPassDto user);

    [MapProperty(nameof(UserTenant.Username), nameof(UserTenantDto.Username))]
    [MapProperty(nameof(UserTenant.TeamId), nameof(UserTenantDto.TeamId))] // Ensure Team is mapped
    public partial UserTenantDto ToUserDto(UserTenant account);
}