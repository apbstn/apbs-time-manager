using Riok.Mapperly.Abstractions;
using Shared.DTOs.UserDtos;
using Shared.Models;

namespace Shared.DTOs.UserDtos.Mappers;

[Mapper]
public partial class UserMapper
{
    [MapperIgnoreTarget(nameof(UserDto.Password))]
    public partial UserDto ToUserDto(User user);

    [MapperIgnoreTarget(nameof(User.PasswordHash))]
    public partial UserNoPassDto ToUserNoPassDto(User user);
}