using Microsoft.Extensions.Configuration;
using Shared.DTOs.Leave;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Services;

public class LeaveRequestService : ILeaveRequestService
{
    private readonly ILeaveRequestRepository _repository;
    private readonly decimal _monthlyAllocation;

    public LeaveRequestService(ILeaveRequestRepository repository, IConfiguration configuration)
    {
        _repository = repository;
        _monthlyAllocation = configuration.GetValue<decimal>("LeaveSettings:MonthlyLeaveAllocation", 10.0m);
    }

    public async Task<IEnumerable<LeaveRequestDto>> GetAllLeaveRequestsAsync()
    {
        var leaveRequests = await _repository.GetAllAsync();
        return leaveRequests.Select(MapToDto);
    }

    public async Task<LeaveRequestDto> GetLeaveRequestByIdAsync(Guid id)
    {
        var leaveRequest = await _repository.GetByIdAsync(id);
        return leaveRequest == null ? null : MapToDto(leaveRequest);
    }

    public async Task<IEnumerable<LeaveRequestDto>> GetLeaveRequestsByUserIdAsync(Guid userId)
    {
        var leaveRequests = await _repository.GetByUserIdAsync(userId);
        return leaveRequests.Select(MapToDto);
    }

    public async Task<LeaveRequestDto> CreateLeaveRequestAsync(CreateLeaveRequestDto createDto)
    {
        await _repository.AllocateMonthlyLeaveAsync(createDto.UserId, _monthlyAllocation);

        var leaveRequest = new LeaveRequest
        {
            UserId = createDto.UserId,
            StartDate = createDto.StartDate,
            EndDate = createDto.EndDate,
            Type = createDto.Type,
            Reason = createDto.Reason,
            Status = LeaveRequestStatus.Pending
        };

        leaveRequest = await _repository.AddAsync(leaveRequest);
        return MapToDto(leaveRequest);
    }

    public async Task<LeaveRequestDto> UpdateLeaveRequestAsync(Guid id, UpdateLeaveRequestDto updateDto)
    {
        var existingRequest = await _repository.GetByIdAsync(id);
        if (existingRequest == null)
        {
            Console.WriteLine($"Leave request with ID {id} not found.");
            return null;
        }

        Console.WriteLine($"Updating leave request {id}. Old StartDate: {existingRequest.StartDate}, Old EndDate: {existingRequest.EndDate}, Old NumberOfDays: {existingRequest.NumberOfDays}");

        var updatedRequest = new LeaveRequest
        {
            Id = existingRequest.Id,
            UserId = existingRequest.UserId,
            StartDate = updateDto.StartDate.HasValue ? updateDto.StartDate.Value : existingRequest.StartDate,
            EndDate = updateDto.EndDate.HasValue ? updateDto.EndDate.Value : existingRequest.EndDate,
            Type = updateDto.Type ?? existingRequest.Type,
            Reason = updateDto.Reason ?? existingRequest.Reason,
            Status = updateDto.Status.HasValue ? updateDto.Status.Value : existingRequest.Status
        };

        if (updateDto.StartDate.HasValue || updateDto.EndDate.HasValue)
        {
            var newNumberOfDays = (updatedRequest.EndDate.Date - updatedRequest.StartDate.Date).Days + 1;
            Console.WriteLine($"Recalculated NumberOfDays: {newNumberOfDays}");
            updatedRequest.GetType().GetProperty("NumberOfDays")?.SetValue(updatedRequest, newNumberOfDays);
        }
        else
        {
            updatedRequest.GetType().GetProperty("NumberOfDays")?.SetValue(updatedRequest, existingRequest.NumberOfDays);
        }

        Console.WriteLine($"New StartDate: {updatedRequest.StartDate}, New EndDate: {updatedRequest.EndDate}, New NumberOfDays: {updatedRequest.NumberOfDays}, New Status: {updatedRequest.Status}");

        updatedRequest = await _repository.UpdateAsync(updatedRequest);
        return MapToDto(updatedRequest);
    }

    public async Task<bool> DeleteLeaveRequestAsync(Guid id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<LeaveBalanceDto> GetLeaveBalanceByUserIdAsync(Guid userId)
    {
        await _repository.AllocateMonthlyLeaveAsync(userId, _monthlyAllocation);

        var balance = await _repository.GetLeaveBalanceByUserIdAsync(userId);
        return new LeaveBalanceDto
        {
            Id = balance.Id,
            UserId = balance.UserId,
            Balance = balance.Balance,
            LastUpdated = balance.LastUpdated,
            LastAllocationMonth = balance.LastAllocationMonth
        };
    }

    public async Task<bool> UpdateLeaveBalanceAsync(Guid userId, decimal newBalance)
    {
        return await _repository.UpdateLeaveBalanceAsync(userId, newBalance);
    }

    public async Task<bool> AllocateMonthlyLeaveAsync(Guid userId, decimal monthlyAllocation)
    {
        return await _repository.AllocateMonthlyLeaveAsync(userId, monthlyAllocation);
    }

    public async Task<int?> GetLastLeaveRequestStatusAsync(Guid userId)
    {
        var leaveRequests = await _repository.GetByUserIdAsync(userId);
        if (leaveRequests == null || !leaveRequests.Any())
        {
            return null;
        }
        var latestRequest = leaveRequests.OrderByDescending(lr => lr.Id).FirstOrDefault();
        return (int?)(latestRequest?.Status ?? null);
    }

    private LeaveRequestDto MapToDto(LeaveRequest leaveRequest)
    {
        return new LeaveRequestDto
        {
            Id = leaveRequest.Id,
            UserId = leaveRequest.UserId,
            Username = leaveRequest.User?.Username,
            StartDate = leaveRequest.StartDate,
            EndDate = leaveRequest.EndDate,
            NumberOfDays = leaveRequest.NumberOfDays,
            Status = leaveRequest.Status,
            Type = leaveRequest.Type,
            Reason = leaveRequest.Reason
        };
    }
}