using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services
{
    public interface ILeaveRequestRepository
    {
        Task<IEnumerable<LeaveRequest>> GetAllAsync();
        Task<LeaveRequest> GetByIdAsync(Guid id);
        Task<IEnumerable<LeaveRequest>> GetByUserIdAsync(Guid userId);
        Task<LeaveRequest> AddAsync(LeaveRequest leaveRequest);
        Task<LeaveRequest> UpdateAsync(LeaveRequest leaveRequest);
        Task<bool> DeleteAsync(Guid id);
    }
}
