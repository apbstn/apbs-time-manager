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
            throw new ArgumentException("L_USER_ID is required and must be a valid user ID.");
        }

        var userExists = await _context.Users.AnyAsync(a => a.Id == leaveRequest.UserId);
        if (!userExists)
        {
            throw new ArgumentException($"Invalid L_USER_ID. User does not exist in T_ACCOUNT. Checked: {leaveRequest.UserId}");
        }

        // Validate and deduct leave balance
        var balance = await GetLeaveBalanceByUserIdAsync(leaveRequest.UserId);
        var requestedDays = leaveRequest.NumberOfDays;
        if (balance == null || balance.Balance < requestedDays)
        {
            throw new InvalidOperationException("Insufficient leave balance.");
        }

        Console.WriteLine($"Deducting {requestedDays} days from balance {balance.Balance} for UserId {leaveRequest.UserId}");
        await UpdateLeaveBalanceAsync(leaveRequest.UserId, balance.Balance - requestedDays);

        _context.LeaveRequests.Add(leaveRequest);
        await _context.SaveChangesAsync();
        return leaveRequest;
    }

    public async Task<LeaveRequest> UpdateAsync(LeaveRequest leaveRequest)
    {
        leaveRequest.StartDate = DateTime.SpecifyKind(leaveRequest.StartDate, DateTimeKind.Utc);
        leaveRequest.EndDate = DateTime.SpecifyKind(leaveRequest.EndDate, DateTimeKind.Utc);

        var existingRequest = await GetByIdAsync(leaveRequest.Id);
        if (existingRequest == null)
        {
            Console.WriteLine($"Leave request with ID {leaveRequest.Id} not found.");
            throw new InvalidOperationException("Leave request not found.");
        }

        // Handle balance adjustments
        var balance = await GetLeaveBalanceByUserIdAsync(leaveRequest.UserId);
        Console.WriteLine($"Current balance: {balance.Balance}, Existing request days: {existingRequest.NumberOfDays}, New request days: {leaveRequest.NumberOfDays}");

        if (leaveRequest.Status == LeaveRequestStatus.Pending && existingRequest.Status == LeaveRequestStatus.Rejected)
        {
            // Re-creating a pending request from rejected: deduct new days
            var requestedDays = leaveRequest.NumberOfDays;
            if (balance == null || balance.Balance < requestedDays)
            {
                throw new InvalidOperationException("Insufficient leave balance.");
            }
            Console.WriteLine($"Deducting {requestedDays} days from balance {balance.Balance} for status change to Pending");
            await UpdateLeaveBalanceAsync(leaveRequest.UserId, balance.Balance - requestedDays);
        }
        else if (leaveRequest.Status == LeaveRequestStatus.Rejected && existingRequest.Status == LeaveRequestStatus.Pending)
        {
            // Rejecting a pending request: restore original days
            Console.WriteLine($"Restoring {existingRequest.NumberOfDays} days to balance {balance.Balance} for status change to Rejected");
            await UpdateLeaveBalanceAsync(leaveRequest.UserId, balance.Balance + existingRequest.NumberOfDays);
        }
        else if (leaveRequest.Status == LeaveRequestStatus.Pending && leaveRequest.NumberOfDays != existingRequest.NumberOfDays)
        {
            // Date change for pending request: restore original days, deduct new days
            var newBalance = balance.Balance + existingRequest.NumberOfDays;
            var newRequestedDays = leaveRequest.NumberOfDays;
            if (newBalance < newRequestedDays)
            {
                throw new InvalidOperationException("Insufficient leave balance for updated dates.");
            }
            Console.WriteLine($"Adjusting balance: Restoring {existingRequest.NumberOfDays} days, deducting {newRequestedDays} days, new balance: {newBalance - newRequestedDays}");
            await UpdateLeaveBalanceAsync(leaveRequest.UserId, newBalance - newRequestedDays);
        }

        // Update the leave request
        existingRequest.StartDate = leaveRequest.StartDate;
        existingRequest.EndDate = leaveRequest.EndDate;
        existingRequest.Type = leaveRequest.Type;
        existingRequest.Reason = leaveRequest.Reason;
        existingRequest.Status = leaveRequest.Status;
        existingRequest.GetType().GetProperty("NumberOfDays")?.SetValue(existingRequest, leaveRequest.NumberOfDays);

        _context.LeaveRequests.Update(existingRequest);
        await _context.SaveChangesAsync();
        return existingRequest;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var leaveRequest = await _context.LeaveRequests.FindAsync(id);
        if (leaveRequest == null)
        {
            return false;
        }

        if (leaveRequest.Status == LeaveRequestStatus.Pending)
        {
            var balance = await GetLeaveBalanceByUserIdAsync(leaveRequest.UserId);
            Console.WriteLine($"Restoring {leaveRequest.NumberOfDays} days to balance {balance.Balance} on deletion");
            await UpdateLeaveBalanceAsync(leaveRequest.UserId, balance.Balance + leaveRequest.NumberOfDays);
        }

        _context.LeaveRequests.Remove(leaveRequest);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<LeaveBalance> GetLeaveBalanceByUserIdAsync(Guid userId)
    {
        var balance = await _context.Set<LeaveBalance>()
            .FirstOrDefaultAsync(lb => lb.UserId == userId);

        if (balance == null)
        {
            balance = new LeaveBalance
            {
                UserId = userId,
                Balance = 0.0m,
                LastUpdated = DateTime.UtcNow,
                LastAllocationMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc)
            };
            await _context.Set<LeaveBalance>().AddAsync(balance);
            await _context.SaveChangesAsync();
        }

        return balance;
    }

    public async Task<LeaveBalance> CreateOrUpdateLeaveBalanceAsync(LeaveBalance balance)
    {
        var existing = await _context.Set<LeaveBalance>()
            .FirstOrDefaultAsync(lb => lb.UserId == balance.UserId);

        if (existing == null)
        {
            balance.Id = Guid.NewGuid();
            balance.LastUpdated = DateTime.UtcNow;
            _context.Set<LeaveBalance>().Add(balance);
        }
        else
        {
            existing.Balance = balance.Balance;
            existing.LastUpdated = DateTime.UtcNow;
            existing.LastAllocationMonth = balance.LastAllocationMonth;
            _context.Set<LeaveBalance>().Update(existing);
            balance = existing;
        }

        await _context.SaveChangesAsync();
        return balance;
    }

    public async Task<bool> UpdateLeaveBalanceAsync(Guid userId, decimal newBalance)
    {
        var balance = await _context.Set<LeaveBalance>()
            .FirstOrDefaultAsync(lb => lb.UserId == userId);

        if (balance == null)
        {
            balance = new LeaveBalance
            {
                UserId = userId,
                Balance = newBalance,
                LastUpdated = DateTime.UtcNow,
                LastAllocationMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc)
            };
            _context.Set<LeaveBalance>().Add(balance);
        }
        else
        {
            balance.Balance = newBalance;
            balance.LastUpdated = DateTime.UtcNow;
            _context.Set<LeaveBalance>().Update(balance);
        }

        Console.WriteLine($"Updating balance for UserId {userId} to {newBalance}");
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AllocateMonthlyLeaveAsync(Guid userId, decimal monthlyAllocation)
    {
        var balance = await _context.Set<LeaveBalance>()
            .FirstOrDefaultAsync(lb => lb.UserId == userId);

        var currentMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        if (balance == null)
        {
            balance = new LeaveBalance
            {
                UserId = userId,
                Balance = monthlyAllocation,
                LastUpdated = DateTime.UtcNow,
                LastAllocationMonth = currentMonth
            };
            _context.Set<LeaveBalance>().Add(balance);
        }
        else if (balance.LastAllocationMonth < currentMonth)
        {
            balance.Balance += monthlyAllocation;
            balance.LastUpdated = DateTime.UtcNow;
            balance.LastAllocationMonth = currentMonth;
            _context.Set<LeaveBalance>().Update(balance);
        }
        else
        {
            return false; // No allocation needed
        }

        await _context.SaveChangesAsync();
        return true;
    }
}