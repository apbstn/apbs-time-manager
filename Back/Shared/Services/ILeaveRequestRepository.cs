using Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shared.Services;

public interface ILeaveRequestRepository
{
    Task<IEnumerable<LeaveRequest>> GetAllAsync();
    Task<LeaveRequest> GetByIdAsync(Guid id);
    Task<IEnumerable<LeaveRequest>> GetByUserIdAsync(Guid userId);
    Task<LeaveRequest> AddAsync(LeaveRequest leaveRequest);
    Task<LeaveRequest> UpdateAsync(LeaveRequest leaveRequest);
    Task<bool> DeleteAsync(Guid id);
    Task<LeaveBalance> GetLeaveBalanceByUserIdAsync(Guid userId);
    Task<LeaveBalance> CreateOrUpdateLeaveBalanceAsync(LeaveBalance balance);
    Task<bool> UpdateLeaveBalanceAsync(Guid userId, decimal newBalance);
    Task<bool> AllocateMonthlyLeaveAsync(Guid userId, decimal monthlyAllocation);
}