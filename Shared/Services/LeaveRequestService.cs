using Shared.Context;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Shared.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ApplicationDbContext _context;

        public LeaveRequestService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LeaveRequest> CreateLeaveRequest(LeaveRequestRequest request)
        {
            var employee = await _context.Employees.FindAsync(request.EmployeeId);
            if (employee == null)
                throw new Exception("Employee not found.");

            var daysRequested = (request.EndDate - request.StartDate).Days + 1;
            if (employee.LeaveBalance < daysRequested)
                throw new Exception("Insufficient leave balance.");

            var lastRequest = await _context.LeaveRequests
                .Where(lr => lr.EmployeeId == request.EmployeeId)
                .OrderByDescending(lr => lr.Sequence)
                .FirstOrDefaultAsync();

            var leaveRequest = new LeaveRequest
            {
                EmployeeId = request.EmployeeId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                LeaveType = request.LeaveType,
                Reason = request.Reason,
                Status = LeaveStatus.Pending,
                Sequence = lastRequest?.Sequence + 1 ?? 1
            };

            _context.LeaveRequests.Add(leaveRequest);
            await _context.SaveChangesAsync();

            return leaveRequest;
        }

        public async Task<LeaveRequest> UpdateLeaveRequest(int id, LeaveRequestRequest request)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest == null)
                throw new Exception("Leave request not found.");

            var employee = await _context.Employees.FindAsync(request.EmployeeId);
            if (employee == null)
                throw new Exception("Employee not found.");

            var daysRequested = (request.EndDate - request.StartDate).Days + 1;
            if (employee.LeaveBalance < daysRequested)
                throw new Exception("Insufficient leave balance.");

            leaveRequest.StartDate = request.StartDate;
            leaveRequest.EndDate = request.EndDate;
            leaveRequest.LeaveType = request.LeaveType;
            leaveRequest.Reason = request.Reason;

            await _context.SaveChangesAsync();

            return leaveRequest;
        }

        public async Task<bool> DeleteLeaveRequest(int id)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest == null)
                return false;

            _context.LeaveRequests.Remove(leaveRequest);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<LeaveRequest> GetLeaveRequest(int id)
        {
            return await _context.LeaveRequests
                .Include(lr => lr.Employee)
                .FirstOrDefaultAsync(lr => lr.Id == id);
        }

        public async Task<List<LeaveRequest>> GetEmployeeLeaveHistory(int employeeId)
        {
            return await _context.LeaveRequests
                .Where(lr => lr.EmployeeId == employeeId)
                .OrderByDescending(lr => lr.StartDate)
                .ToListAsync();
        }

        public async Task<LeaveRequest> UpdateLeaveStatus(int id, string action)
        {
            var leaveRequest = await _context.LeaveRequests
                .Include(lr => lr.Employee)
                .FirstOrDefaultAsync(lr => lr.Id == id);

            if (leaveRequest == null)
                throw new Exception("Leave request not found.");

            if (leaveRequest.Status != LeaveStatus.Pending)
                throw new Exception("Leave request has already been processed.");

            switch (action.ToLower())
            {
                case "approve":
                    if (leaveRequest.Employee.LeaveBalance < leaveRequest.DaysRequested)
                        throw new Exception("Insufficient leave balance.");

                    leaveRequest.Employee.LeaveBalance -= leaveRequest.DaysRequested;
                    leaveRequest.Status = LeaveStatus.Approved;
                    break;

                case "reject":
                    leaveRequest.Status = LeaveStatus.Rejected;
                    break;

                default:
                    throw new Exception("Invalid action. Use 'approve' or 'reject'.");
            }

            await _context.SaveChangesAsync();

            return leaveRequest;
        }

        public async Task<List<LeaveRequest>> GetLeaveRequestsByType(string type, int employeeId)
        {
            return await _context.LeaveRequests
                .Where(lr => lr.EmployeeId == employeeId && lr.LeaveType.ToLower() == type.ToLower())
                .ToListAsync();
        }

        public async Task<List<LeaveRequest>> GetAllLeaveRequests()
        {
            return await _context.LeaveRequests
                .Include(lr => lr.Employee)
                .OrderByDescending(lr => lr.StartDate)
                .ToListAsync();
        }

        public async Task<decimal> GetEmployeeLeaveBalance(int employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            return employee?.LeaveBalance ?? 0;
        }
    }
}