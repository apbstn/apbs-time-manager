using Microsoft.EntityFrameworkCore;
using Shared.Context;
using Shared.Models;
using Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Services
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public LeaveRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LeaveRequest>> GetAllAsync()
        {
            return await _context.LeaveRequests
                .Include(lr => lr.User)
                .ToListAsync();
        }

        public async Task<LeaveRequest> GetByIdAsync(Guid id)
        {
            return await _context.LeaveRequests
                .Include(lr => lr.User)
                .FirstOrDefaultAsync(lr => lr.Id == id);
        }

        public async Task<IEnumerable<LeaveRequest>> GetByUserIdAsync(Guid userId)
        {
            return await _context.LeaveRequests
                .Include(lr => lr.User)
                .Where(lr => lr.UserId == userId)
                .ToListAsync();
        }

        public async Task<LeaveRequest> AddAsync(LeaveRequest leaveRequest)
        {
            // Ensure UTC for DateTime properties
            leaveRequest.StartDate = leaveRequest.StartDate.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(leaveRequest.StartDate, DateTimeKind.Utc)
                : leaveRequest.StartDate.ToUniversalTime();
            leaveRequest.EndDate = leaveRequest.EndDate.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(leaveRequest.EndDate, DateTimeKind.Utc)
                : leaveRequest.EndDate.ToUniversalTime();

            _context.LeaveRequests.Add(leaveRequest);
            await _context.SaveChangesAsync();
            return leaveRequest;
        }

        public async Task<LeaveRequest> UpdateAsync(LeaveRequest leaveRequest)
        {
            // Ensure UTC for DateTime properties
            leaveRequest.StartDate = leaveRequest.StartDate.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(leaveRequest.StartDate, DateTimeKind.Utc)
                : leaveRequest.StartDate.ToUniversalTime();
            leaveRequest.EndDate = leaveRequest.EndDate.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(leaveRequest.EndDate, DateTimeKind.Utc)
                : leaveRequest.EndDate.ToUniversalTime();

            _context.LeaveRequests.Update(leaveRequest);
            await _context.SaveChangesAsync();
            return leaveRequest;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest == null)
            {
                return false;
            }

            _context.LeaveRequests.Remove(leaveRequest);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}