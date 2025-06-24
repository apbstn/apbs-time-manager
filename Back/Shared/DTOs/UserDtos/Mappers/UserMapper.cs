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

    [MapProperty(nameof(UserTenant.Username), nameof(UserDto.Username))]
    [MapProperty(nameof(UserTenant.Team), nameof(UserDto.Team))] // Ensure Team is mapped
    public partial UserDto ToUserDto(UserTenant account);
}