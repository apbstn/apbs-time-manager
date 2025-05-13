using Shared.DTOs.Leave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services
{
    public interface ILeaveRequestService
    {
        Task<IEnumerable<LeaveRequestDto>> GetAllLeaveRequestsAsync();
        Task<LeaveRequestDto> GetLeaveRequestByIdAsync(Guid id);
        Task<IEnumerable<LeaveRequestDto>> GetLeaveRequestsByUserIdAsync(Guid userId);
        Task<LeaveRequestDto> CreateLeaveRequestAsync(Guid userId, CreateLeaveRequestDto createDto);
        Task<LeaveRequestDto> UpdateLeaveRequestAsync(Guid id, UpdateLeaveRequestDto updateDto);
        Task<bool> DeleteLeaveRequestAsync(Guid id);
    }
}
