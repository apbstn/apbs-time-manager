using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services
{
    public interface ILeaveRequestService
    {
        Task<LeaveRequest> CreateLeaveRequest(LeaveRequestRequest request);
        Task<LeaveRequest> UpdateLeaveRequest(int id, LeaveRequestRequest request);
        Task<bool> DeleteLeaveRequest(int id);
        Task<LeaveRequest> GetLeaveRequest(int id);
        Task<List<LeaveRequest>> GetEmployeeLeaveHistory(int employeeId);
        Task<LeaveRequest> UpdateLeaveStatus(int id, string action);
        Task<List<LeaveRequest>> GetLeaveRequestsByType(string type, int employeeId);
        Task<List<LeaveRequest>> GetAllLeaveRequests();
        Task<decimal> GetEmployeeLeaveBalance(int employeeId);
    }
}
