using Shared.DTOs.Leave;
using Shared.DTOs.Leave.Mapper;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ILeaveRequestRepository _repository;

        public LeaveRequestService(ILeaveRequestRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<LeaveRequestDto>> GetAllLeaveRequestsAsync()
        {
            var leaveRequests = await _repository.GetAllAsync();
            var result = new List<LeaveRequestDto>();
            
             var _mapper = new LeaveMapper();
            foreach (var lr in leaveRequests)
            {
                result.Add(_mapper.ToLeaveRequestDto(lr));
            }

            return result;
        }

        public async Task<LeaveRequestDto> GetLeaveRequestByIdAsync(Guid id)
        {
            var leaveRequest = await _repository.GetByIdAsync(id);
            return leaveRequest == null ? null : MapToDto(leaveRequest);
        }

        public async Task<IEnumerable<LeaveRequestDto>> GetLeaveRequestsByUserIdAsync(Guid userId)
        {
            var leaveRequests = await _repository.GetByUserIdAsync(userId);
            var result = new List<LeaveRequestDto>();

            foreach (var lr in leaveRequests)
            {
                result.Add(MapToDto(lr));
            }

            return result;
        }

        public async Task<LeaveRequestDto> CreateLeaveRequestAsync(CreateLeaveRequestDto createDto)
        {
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
            if (existingRequest == null) return null;

            // Update only provided fields
            if (updateDto.StartDate.HasValue)
                existingRequest.StartDate = updateDto.StartDate.Value;

            if (updateDto.EndDate.HasValue)
                existingRequest.EndDate = updateDto.EndDate.Value;

            if (updateDto.Status.HasValue)
                existingRequest.Status = updateDto.Status.Value;

            if (!string.IsNullOrEmpty(updateDto.Type))
                existingRequest.Type = updateDto.Type;

            if (!string.IsNullOrEmpty(updateDto.Reason))
                existingRequest.Reason = updateDto.Reason;

            // NumberOfDays is automatically recalculated by the model
            existingRequest = await _repository.UpdateAsync(existingRequest);

            return MapToDto(existingRequest);
        }

        public async Task<bool> DeleteLeaveRequestAsync(Guid id)
        {
            return await _repository.DeleteAsync(id);
        }

        private LeaveRequestDto MapToDto(LeaveRequest leaveRequest)
        {
            return new LeaveRequestDto
            {
                Id = leaveRequest.Id,
                UserId = leaveRequest.UserId,
                StartDate = leaveRequest.StartDate,
                EndDate = leaveRequest.EndDate,
                NumberOfDays = leaveRequest.NumberOfDays,
                Status = leaveRequest.Status,
                Type = leaveRequest.Type,
                Reason = leaveRequest.Reason
            };
        }
    }
}
