using Riok.Mapperly.Abstractions;
using Shared.DTOs.TenantDtos;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Leave.Mapper;

[Mapper]
public partial class LeaveMapper
{
    [MapProperty(nameof(LeaveRequest.User.Username), nameof(LeaveRequestDto.Username))]
    public partial LeaveRequestDto ToLeaveRequestDto(LeaveRequest leave);
}
