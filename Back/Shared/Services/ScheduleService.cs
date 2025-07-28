using Shared.DTOs.PlannerDtos;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services;

public class ScheduleService : IScheduleService
{
    private readonly IScheduleRepository _repository;

    public ScheduleService(IScheduleRepository repository)
    {
        _repository = repository;
    }

    public async Task<ScheduleDto> CreateAsync(CreateScheduleDto dto)
    {
        var schedule = new Schedule
        {
            Name = dto.Name,
            WorkType = dto.WorkType,
            SelectedDays = string.Join(",", dto.SelectedDays),
            FlexibleHoursJson = System.Text.Json.JsonSerializer.Serialize(dto.FlexibleHours),
            FixedHoursJson = System.Text.Json.JsonSerializer.Serialize(dto.FixedHours),
            WeeklyHours = dto.WeeklyHours,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(schedule);
        return MapToDto(created);
    }

    public async Task<ScheduleDto?> GetByIdAsync(int id)
    {
        var schedule = await _repository.GetByIdAsync(id);
        return schedule != null ? MapToDto(schedule) : null;
    }

    public async Task<List<ScheduleDto>> GetAllAsync()
    {
        var schedules = await _repository.GetAllAsync();
        return schedules.Select(MapToDto).ToList();
    }

    public async Task<ScheduleDto?> UpdateAsync(int id, UpdateScheduleDto dto)
    {
        var schedule = await _repository.GetByIdAsync(id);
        if (schedule == null) return null;

        schedule.Name = dto.Name;
        schedule.WorkType = dto.WorkType;
        schedule.SelectedDays = string.Join(",", dto.SelectedDays);
        schedule.FlexibleHoursJson = System.Text.Json.JsonSerializer.Serialize(dto.FlexibleHours);
        schedule.FixedHoursJson = System.Text.Json.JsonSerializer.Serialize(dto.FixedHours);
        schedule.WeeklyHours = dto.WeeklyHours;

        var updated = await _repository.UpdateAsync(schedule);
        return MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    private ScheduleDto MapToDto(Schedule schedule)
    {
        return new ScheduleDto(
            schedule.Id,
            schedule.Name,
            schedule.WorkType,
            schedule.SelectedDays.Split(',', StringSplitOptions.RemoveEmptyEntries),
            System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, FlexibleHoursDto>>(schedule.FlexibleHoursJson) ?? new(),
            System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, FixedHoursDto>>(schedule.FixedHoursJson) ?? new(),
            schedule.WeeklyHours,
            schedule.CreatedAt
        );
    }
}
