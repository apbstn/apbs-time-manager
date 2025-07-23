using Shared.DTOs.Leave;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shared.Services;

public interface ILeaveRequestService
{
    Task<IEnumerable<LeaveRequestDto>> GetAllLeaveRequestsAsync();
    Task<LeaveRequestDto> GetLeaveRequestByIdAsync(Guid id);
    Task<IEnumerable<LeaveRequestDto>> GetLeaveRequestsByUserIdAsync(Guid userId);
    Task<LeaveRequestDto> CreateLeaveRequestAsync(CreateLeaveRequestDto createDto);
    Task<LeaveRequestDto> UpdateLeaveRequestAsync(Guid id, UpdateLeaveRequestDto updateDto);
    Task<bool> DeleteLeaveRequestAsync(Guid id);
    Task<LeaveBalanceDto> GetLeaveBalanceByUserIdAsync(Guid userId);
    Task<bool> UpdateLeaveBalanceAsync(Guid userId, decimal newBalance);
    Task<bool> AllocateMonthlyLeaveAsync(Guid userId, decimal monthlyAllocation);
}