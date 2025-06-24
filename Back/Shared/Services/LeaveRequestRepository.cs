using Microsoft.EntityFrameworkCore;
using Shared.Context;
using Shared.Models;
using Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Services;

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
        leaveRequest.StartDate = DateTime.SpecifyKind(leaveRequest.StartDate, DateTimeKind.Utc);
        leaveRequest.EndDate = DateTime.SpecifyKind(leaveRequest.EndDate, DateTimeKind.Utc);
        
        if (leaveRequest.UserId == Guid.Empty)
        {
            if (leaveRequest.UserId == null)
            {
                throw new ArgumentException("L_USER_ID is required and must be a valid user ID.");
            }
            else
            {
                throw new ArgumentException("IDK");
            }
            
        }

        Console.WriteLine($"Validating UserId: {leaveRequest.UserId}");
        var userExists = await _context.Users.AnyAsync(a => a.Id == leaveRequest.UserId); // a.Id is A_ID
        Console.WriteLine($"Validating UserId from request: {leaveRequest.UserId}");
        var user = await _context.Users.FirstOrDefaultAsync(a => a.Id == leaveRequest.UserId);
        Console.WriteLine($"User found: {user != null}, User: {user?.Email}");
        if (!userExists)
        {
            throw new ArgumentException($"Invalid L_USER_ID. User does not exist in T_ACCOUNT. Checked: {leaveRequest.UserId}");
        }

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