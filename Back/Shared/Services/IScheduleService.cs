using Shared.DTOs.PlannerDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services;

public interface IScheduleService
{
    Task<ScheduleDto> CreateAsync(CreateScheduleDto dto);
    Task<ScheduleDto?> GetByIdAsync(int id);
    Task<List<ScheduleDto>> GetAllAsync();
    Task<ScheduleDto?> UpdateAsync(int id, UpdateScheduleDto dto);
    Task<bool> DeleteAsync(int id);
}
